from src.backend.DeckManagement.InputIdentifier import Input
from src.backend.PluginManager.ActionBase import ActionBase
from ..Enums import Enums
from ..Utils import Ui
from collections import deque
import time


class DialAction(ActionBase):
    selectedDialKey = "SelectedDial"
    prevDialDeltasCount = 4

    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)

    def on_ready(self):
        self.plugin_base.RegisterDial(self)
        self.selectedDialIndex = None
        self.selectedDialCode = None

        self.dialState = 0
        self.dialSpeed = 1
        self.prevDialSpeedSlow = True
        self.lastDialTime = 0
        self.prevDialDeltas = deque(maxlen=DialAction.prevDialDeltasCount)

        settings = self.get_settings()
        if self.selectedDialKey in settings:
            self.SetSelectedDial(settings[self.selectedDialKey])
            self.UpdateVisuals()
        else:
            self.set_top_label("New")

        self.plugin_base.SendBufferedDatagram(b"DataRequest")

    def get_config_rows(self):
        self.prefRow = Ui.GetConfigRow("DialAction", Enums.DialActions, self.OnPrefInputChanged)
        return [self.prefRow]

    def event_callback(self, event, data):
        if self.selectedDialCode != "VS" and (event == Input.Dial.Events.TURN_CW or event == Input.Dial.Events.TURN_CCW):
            self.AdjustDialSpeed()

        match self.selectedDialCode:
            case "ALT":
                self.HandleAltitude(event)

            case "HDG":
                self.HandleHeading(event)

            case "SPD":
                self.HandleAtSpeed(event)

            case "VS":
                self.dialSpeed = 1
                self.HandleVerticalSpeed(event)

            case _:
                self.show_error()
                return

        self.UpdateVisuals()

    def OnPrefInputChanged(self, rowInput):
        self.SetSelectedDial(rowInput.get_active())
        self.UpdateSetting(self.selectedDialKey, self.selectedDialIndex)

    def SetSelectedDial(self, index):
        self.selectedDialIndex = index
        self.selectedDialCode = Enums.DialActions[index][0]
        self.set_top_label(self.selectedDialCode)
        self.UpdateVisuals()

    def UpdateSetting(self, key, value):
        settings = self.get_settings()
        settings[key] = value
        self.set_settings(settings)

    def UpdateVisuals(self):
        match self.selectedDialCode:
            case "ALT":
                if self.dialState >= 100:
                    text = f"FL{self.dialState}"
                else:
                    text = f"{self.dialState * 100}ft"

            case "HDG":
                text = f"{self.dialState}°"

            case "SPD":
                text = f"{self.dialState}kt"

            case "VS":
                text = f"{self.dialState * 100}ft"
                if self.dialState > 0:
                    text = "+" + text

            case _:
                text = "ERROR"
            
        self.set_bottom_label(text)

    def AdjustDialSpeed(self):
        now = time.monotonic()
        self.prevDialDeltas.append(now - self.lastDialTime)
        self.lastDialTime = now

        if (sum(self.prevDialDeltas) / DialAction.prevDialDeltasCount) < 0.065:
            self.dialSpeed = 10 - DialAction.prevDialDeltasCount if self.prevDialSpeedSlow else 10
            self.prevDialSpeedSlow = False
        else:
            self.dialSpeed = 1
            self.prevDialSpeedSlow = True

    def HandleHeading(self, event):
        match event:
            case Input.Dial.Events.TURN_CW:
                self.dialState += self.dialSpeed

            case Input.Dial.Events.TURN_CCW:
                self.dialState -= self.dialSpeed

        self.dialState %= 360

    def HandleAltitude(self, event):
        match event:
            case Input.Dial.Events.TURN_CW:
                self.dialState += self.dialSpeed

            case Input.Dial.Events.TURN_CCW:
                self.dialState -= self.dialSpeed

        if self.dialState < 0:
            self.dialState = 0
        elif self.dialState > 510:
            self.dialState = 510

    def HandleVerticalSpeed(self, event):
        match event:
            case Input.Dial.Events.DOWN:
                self.dialState = 0
                return

            case Input.Dial.Events.TURN_CW:
                self.dialState += self.dialSpeed

            case Input.Dial.Events.TURN_CCW:
                self.dialState -= self.dialSpeed

        if self.dialState < -100:
            self.dialState = -100
        elif self.dialState > 100:
            self.dialState = 100

    def HandleAtSpeed(self, event):
        match event:
            case Input.Dial.Events.TURN_CW:
                self.dialState += self.dialSpeed

            case Input.Dial.Events.TURN_CCW:
                self.dialState -= self.dialSpeed

        if self.dialState < 0:
            self.dialState = 0
        elif self.dialState > 999:
            self.dialState = 999
