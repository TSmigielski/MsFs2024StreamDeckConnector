# Import libs
import json
import socket
import threading
import time

# Import StreamController modules
from src.backend.DeckManagement.InputIdentifier import Input
from src.backend.PluginManager.ActionHolder import ActionHolder
from src.backend.PluginManager.ActionInputSupport import ActionInputSupport
from src.backend.PluginManager.PluginBase import PluginBase

# Import actions
from .Actions.ToggleAction import ToggleAction
from .Actions.DialAction import DialAction

PluginId = "com_TomaszSmigielski_MsFs2024Connector"
UdpAddress = ("127.0.0.1", 13337)
Debug = False


class MsFsConnector(PluginBase):
    def __init__(self):
        super().__init__()
        self.toggles = set()
        self.dials = set()
        self.lastBufferedDataGramTime = 999

        # Register actions
        self.add_action_holder(ActionHolder(
            plugin_base=self,
            action_base=ToggleAction,
            action_id=PluginId + "::ToggleAction",
            action_name="Toggle",
            action_support={
                Input.Key: ActionInputSupport.SUPPORTED,
                Input.Dial: ActionInputSupport.UNSUPPORTED,
                Input.Touchscreen: ActionInputSupport.UNSUPPORTED
            }
        ))

        self.add_action_holder(ActionHolder(
            plugin_base=self,
            action_base=DialAction,
            action_id=PluginId + "::DialAction",
            action_name="Dial",
            action_support={
                Input.Key: ActionInputSupport.UNSUPPORTED,
                Input.Dial: ActionInputSupport.SUPPORTED,
                Input.Touchscreen: ActionInputSupport.UNSUPPORTED
            }
        ))

        # Register plugin
        self.register(
            plugin_name="Microsoft Flight Simulator 2024 Connector",
            github_repo="https://github.com/StreamController/PluginTemplate",
            app_version="1.1.1-alpha"
        )

        # Start Udp client
        self.udpClient = UdpClient(self)
        self.udpClient.start()

    def RegisterToggle(self, toggle):
        self.toggles.add(toggle)

    def RegisterDial(self, dial):
        self.dials.add(dial)

    def SendDatagram(self, data: object):
        self.udpClient.sock.sendto(json.dumps(data).encode(encoding="utf-8"), UdpAddress)

    def SendBufferedDatagram(self, data: object):
        now = time.perf_counter()
        if now - self.lastBufferedDataGramTime < .25:
            return

        self.lastBufferedDataGramTime = now
        self.SendDatagram(data)

    def UpdateActionsData(self, data):
        for toggle in self.toggles:
            match toggle.selectedToggleCode:
                case "ALT":
                    toggle.toggleState = data["AltitudeToggle"]

                case "AP":
                    toggle.toggleState = data["AutopilotMasterToggle"]

                case "APR":
                    toggle.toggleState = data["ApproachToggle"]

                case "AT":
                    toggle.toggleState = data["AutoThrottleToggle"]
                    toggle.manuallySet = data["AutoThrottleManToggle"]

                case "FD":
                    toggle.toggleState = data["FlightDirectorToggle"]

                case "FLC":
                    toggle.toggleState = data["FlightLevelChangeToggle"]

                case "HDG":
                    toggle.toggleState = data["HeadingToggle"]

                case "LVL":
                    toggle.toggleState = data["LevelerToggle"]

                case "NAV":
                    toggle.toggleState = data["NavigationToggle"]

                case "VS":
                    toggle.toggleState = data["VerticalSpeedToggle"]

                case "VNAV":
                    toggle.toggleState = data["VerticalNavigationToggle"]

                case "YD":
                    toggle.toggleState = data["YawDamperToggle"]

            toggle.UpdateVisuals()

        now = time.monotonic()
        for dial in self.dials:
            match dial.selectedDialCode:
                case "ALT":
                    dial.SetDialState(int(data["SelectedAltitude"] / 100), now)

                case "HDG":
                    dial.SetDialState(int(data["SelectedHeading"]), now)

                case "VS":
                    dial.SetDialState(int(data["SelectedVerticalSpeed"] / 100), now)

                case "SPD":
                    dial.SetDialState(int(data["SelectedSpeed"]), now)

            dial.UpdateVisuals()

    def GetDial(self, dialCode: str) -> DialAction:
        for dial in self.dials:
            if dial.selectedDialCode == dialCode:
                return dial

        return None

class UdpClient(threading.Thread):
    def __init__(self, plugin):
        super().__init__(daemon=True)
        self.plugin = plugin
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.sock.sendto(b"Initialized", UdpAddress)

    def run(self):
        while True:
            data, addr = self.sock.recvfrom(10_000)
            decoded = data.decode()
            if Debug:
                print(f"Received from {addr}: {decoded}")

            try:
                self.plugin.UpdateActionsData(json.loads(decoded))
            except Exception as ex:
                print(ex)
