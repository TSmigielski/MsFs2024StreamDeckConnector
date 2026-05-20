from src.backend.DeckManagement.InputIdentifier import Input
from src.backend.PluginManager.ActionBase import ActionBase
from ..Enums import Enums
from ..Utils import Ui


class ToggleAction(ActionBase):
    selectedToggleKey = "SelectedToggle"

    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        self.selectedToggleIndex = None

    def on_ready(self):
        self.plugin_base.RegisterToggle(self)
        self.selectedToggleIndex = None
        self.selectedToggleCode = None
        self.toggleState = False
        self.holdSinceDown = False

        settings = self.get_settings()
        if self.selectedToggleKey in settings:
            self.SetSelectedToggle(settings[self.selectedToggleKey])
        else:
            self.set_top_label("New")

        self.set_bottom_label("")
        self.plugin_base.SendBufferedDatagram({})

    def get_config_rows(self):
        self.prefRow = Ui.GetConfigRow("ToggleAction", Enums.ToggleActions, self.OnPrefInputChanged)

        if self.selectedToggleIndex is not None:
            self.prefRow.input.set_active(self.selectedToggleIndex)

        return [self.prefRow]

    def event_callback(self, event, data):
        if self.selectedToggleIndex is None or not self.selectedToggleIndex >= 0:
            return

        match self.selectedToggleCode:
            case "AT":
                self.HandleAutoThrottle(event)
                return

        if event == Input.Key.Events.DOWN:
            self.DefaultClick()

    def DefaultClick(self) -> None:
        self.toggleState = not self.toggleState

        self.plugin_base.SendDatagram({
            "Toggle": self.selectedToggleCode,
            "DesiredState": self.toggleState
        })

        if self.selectedToggleCode == "VS":
            dial = self.plugin_base.GetDial("VS")
            if dial is not None:
                dial.resendOwn = True

    def HandleAutoThrottle(self, event):
        match event:
            case Input.Key.Events.UP:
                if self.holdSinceDown:
                    self.holdSinceDown = False
                    return

                self.DefaultClick()

            case Input.Key.Events.HOLD_START:
                self.holdSinceDown = True
                self.manuallySet = not self.manuallySet

                self.plugin_base.SendDatagram({
                    "Toggle": "ATMAN",
                    "DesiredState": self.manuallySet
                })

    def OnPrefInputChanged(self, rowInput):
        self.SetSelectedToggle(rowInput.get_active())
        self.UpdateSetting(self.selectedToggleKey, self.selectedToggleIndex)
        self.plugin_base.SendDatagram({})

    def SetSelectedToggle(self, index):
        self.selectedToggleIndex = index
        self.selectedToggleCode = Enums.ToggleActions[index][0]
        self.set_top_label(self.selectedToggleCode)

    def UpdateSetting(self, key, value):
        settings = self.get_settings()
        settings[key] = value
        self.set_settings(settings)

    def SetState(self, newState):
        self.toggleState = newState

    def UpdateVisuals(self):
        match self.selectedToggleCode:
            case "AT":
                self.set_top_label("AT MAN" if self.manuallySet else "AT FMS")

        self.set_bottom_label("*" if self.toggleState else "")
