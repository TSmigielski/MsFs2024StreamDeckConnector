import json
from src.backend.PluginManager.ActionBase import ActionBase
from ..Enums import Enums
from ..Utils import Ui


class ToggleAction(ActionBase):
    selectedToggleKey = "SelectedToggle"

    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)

    def on_ready(self):
        self.plugin_base.RegisterToggle(self)
        self.selectedToggleIndex = None
        self.selectedToggleCode = None
        self.toggleState = False

        settings = self.get_settings()
        if self.selectedToggleKey in settings:
            self.SetSelectedToggle(settings[self.selectedToggleKey])
        else:
            self.set_top_label("New")

        self.set_bottom_label("OFF")
        self.plugin_base.SendBufferedDatagram({})

    def get_config_rows(self):
        self.prefRow = Ui.GetConfigRow("ToggleAction", Enums.ToggleActions, self.OnPrefInputChanged)

        if self.selectedToggleIndex is not None:
            self.prefRow.input.set_active(self.selectedToggleIndex)

        return [self.prefRow]

    def on_key_down(self) -> None:
        if self.selectedToggleIndex is None or not self.selectedToggleIndex >= 0:
            return

        self.toggleState = not self.toggleState

        self.plugin_base.SendDatagram({
            "Toggle": self.selectedToggleCode,
            "DesiredState": self.toggleState
        })

    def OnPrefInputChanged(self, rowInput):
        self.SetSelectedToggle(rowInput.get_active())
        self.UpdateSetting(self.selectedToggleKey, self.selectedToggleIndex)

    def SetSelectedToggle(self, index):
        self.selectedToggleIndex = index
        self.selectedToggleCode = Enums.ToggleActions[index][0]
        self.set_top_label(self.selectedToggleCode)

    def UpdateSetting(self, key, value):
        settings = self.get_settings()
        settings[key] = value
        self.set_settings(settings)

    def UpdateVisuals(self):
        self.set_bottom_label("ON" if self.toggleState else "OFF")
