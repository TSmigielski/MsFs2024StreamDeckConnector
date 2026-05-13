import gi
import json
from gi.repository import Gtk, Adw
from src.backend.PluginManager.ActionBase import ActionBase
from ..Enums import Enums

gi.require_version("Gtk", "4.0")
gi.require_version("Adw", "1")


class ToggleAction(ActionBase):
    selectedToggleKey = "SelectedToggle"

    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)

    def on_ready(self) -> None:
        # icon_path = os.path.join(self.plugin_base.PATH, "Assets", "info.png")
        # self.set_media(media_path=icon_path, size=0.75)
        self.selectedToggle = None
        self.toggleState = False

        settings = self.get_settings()
        if self.selectedToggleKey in settings:
            self.selectedToggle = settings[self.selectedToggleKey]
            self.set_top_label(Enums.ToggleActions[self.selectedToggle][0])
        else:
            self.set_top_label("New")

        self.set_bottom_label("OFF")

    def get_config_rows(self):
        self.storeModel = Gtk.ListStore.new([str, str])
        for action in sorted(Enums.ToggleActions, key=lambda x: x[1]):
            self.storeModel.append([action[1], action[0]])

        self.prefRow = Adw.PreferencesRow(title="ToggleAction")
        self.prefRowBox = Gtk.Box(
            orientation=Gtk.Orientation.HORIZONTAL,
            margin_start=10,
            margin_end=10,
            margin_top=10,
            margin_bottom=10
        )

        self.cellRenderer = Gtk.CellRendererText()
        self.prefRowInput = Gtk.ComboBox.new_with_model(self.storeModel)
        self.prefRowInput.pack_start(self.cellRenderer, True)
        self.prefRowInput.add_attribute(self.cellRenderer, "text", 0)

        if self.selectedToggle is not None:
            self.prefRowInput.set_active(self.selectedToggle)

        self.prefRowInput.connect("changed", self.OnPrefInputChanged)

        self.prefRow.set_child(self.prefRowBox)
        self.prefRowBox.append(Gtk.Label(
            label="ToggleAction:",
            hexpand=True,
            xalign=0
        ))

        self.prefRowBox.append(self.prefRowInput)

        return [self.prefRow]

    def on_key_down(self) -> None:
        if self.selectedToggle is None or not self.selectedToggle >= 0:
            return

        self.toggleState = not self.toggleState
        self.set_bottom_label("ON" if self.toggleState else "OFF")

        payload = {
            "Toggle": Enums.ToggleActions[self.selectedToggle][0],
            "State": self.toggleState
        }

        self.plugin_base.udpTransport.sendto(json.dumps(payload).encode(encoding="utf-8"))

    def OnPrefInputChanged(self, rowInput):
        settings = self.get_settings()
        settings[self.selectedToggleKey] = self.selectedToggle = rowInput.get_active()
        self.set_settings(settings)
