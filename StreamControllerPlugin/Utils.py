import gi
from gi.repository import Gtk, Adw

gi.require_version("Gtk", "4.0")
gi.require_version("Adw", "1")


class Ui():
    def GetConfigRow(title: str, actionsEnum: list[list[str]], onInputChanged: callable):
        prefRow = Adw.PreferencesRow(title=title)
        prefRow.storeModel = Gtk.ListStore.new([str, str])

        for action in actionsEnum:
            prefRow.storeModel.append([action[1], action[0]])

        prefRow.box = Gtk.Box(
            orientation=Gtk.Orientation.HORIZONTAL,
            margin_start=10,
            margin_end=10,
            margin_top=10,
            margin_bottom=10
        )

        prefRow.cellRenderer = Gtk.CellRendererText()
        prefRow.input = Gtk.ComboBox.new_with_model(prefRow.storeModel)
        prefRow.input.pack_start(prefRow.cellRenderer, True)
        prefRow.input.add_attribute(prefRow.cellRenderer, "text", 0)

        if onInputChanged is not None:
            prefRow.input.connect("changed", onInputChanged)

        prefRow.set_child(prefRow.box)
        prefRow.box.append(Gtk.Label(
            label="ToggleAction:",
            hexpand=True,
            xalign=0
        ))

        prefRow.box.append(prefRow.input)

        return prefRow
