from src.backend.DeckManagement.InputIdentifier import Input
from src.backend.PluginManager.ActionBase import ActionBase
from ..Enums import Enums
from ..Utils import TrailingThrottle, Ui
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
        
        self.udpThrottle = TrailingThrottle(self.SendDatagram, 100)

        settings = self.get_settings()
        if self.selectedDialKey in settings:
            self.SetSelectedDial(settings[self.selectedDialKey])
            self.UpdateVisuals()
        else:
            self.set_top_label("New")

        self.plugin_base.SendBufferedDatagram({})

    def get_config_rows(self):
        self.prefRow = Ui.GetConfigRow("DialAction", Enums.DialActions, self.OnPrefInputChanged)
        return [self.prefRow]

    def event_callback(self, event, data):
        if self.selectedDialCode != "VS" and (event == Input.Dial.Events.TURN_CW or event == Input.Dial.Events.TURN_CCW):
            self.AdjustDialSpeed()

        sendDatagram = False

        match self.selectedDialCode:
            case "ALT":
                sendDatagram = self.HandleAltitude(event)

            case "HDG":
                sendDatagram = self.HandleHeading(event)

            case "SPD":
                sendDatagram = self.HandleSpeed(event)

            case "VS":
                self.dialSpeed = 1
                sendDatagram = self.HandleVerticalSpeed(event)

            case _:
                self.show_error()
                return

        self.UpdateVisuals()

        if sendDatagram:
            self.udpThrottle()

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

    def SendDatagram(self, data: object = None):
        if data is None:
            data = {
                "Dial": self.selectedDialCode,
                "DialValue": self.dialState
            }

        self.plugin_base.SendDatagram(data)

    def SetDialState(self, newState, now):
        if now - self.lastDialTime > 0.5:
            self.dialState = newState

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

    def HandleHeading(self, event) -> bool:
        match event:
            case Input.Dial.Events.DOWN:
                self.SendDatagram({
                    "Dial": self.selectedDialCode,
                    "DesiredState": True
                })
                return False

            case Input.Dial.Events.TURN_CW:
                self.dialState += self.dialSpeed

            case Input.Dial.Events.TURN_CCW:
                self.dialState -= self.dialSpeed

            case _:
                return False

        self.dialState %= 360
        return True

    def HandleAltitude(self, event) -> bool:
        match event:
            case Input.Dial.Events.DOWN:
                self.SendDatagram({
                    "Dial": self.selectedDialCode,
                    "DesiredState": True
                })
                return False

            case Input.Dial.Events.TURN_CW:
                self.dialState += self.dialSpeed

            case Input.Dial.Events.TURN_CCW:
                self.dialState -= self.dialSpeed

            case _:
                return False

        if self.dialState < 0:
            self.dialState = 0
        elif self.dialState > 510:
            self.dialState = 510

        return True

    def HandleVerticalSpeed(self, event) -> bool:
        match event:
            case Input.Dial.Events.DOWN:
                self.dialState = 0
                return True

            case Input.Dial.Events.TURN_CW:
                self.dialState += self.dialSpeed

            case Input.Dial.Events.TURN_CCW:
                self.dialState -= self.dialSpeed

            case _:
                return False

        if self.dialState < -99:
            self.dialState = -99
        elif self.dialState > 99:
            self.dialState = 99

        self.lastDialTime = time.monotonic()
        return True

    def HandleSpeed(self, event) -> bool:
        match event:
            case Input.Dial.Events.DOWN:
                self.SendDatagram({
                    "Dial": self.selectedDialCode,
                    "DesiredState": True
                })
                return False

            case Input.Dial.Events.TURN_CW:
                self.dialState += self.dialSpeed

            case Input.Dial.Events.TURN_CCW:
                self.dialState -= self.dialSpeed

            case _:
                return False

        if self.dialState < 0:
            self.dialState = 0
        elif self.dialState > 999:
            self.dialState = 999

        return True
