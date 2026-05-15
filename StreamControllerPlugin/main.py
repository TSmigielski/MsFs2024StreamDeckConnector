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

    def SendDatagram(self, data):
        self.udpClient.sock.sendto(data, UdpAddress)

    def SendBufferedDatagram(self, data):
        now = time.perf_counter()
        if now - self.lastBufferedDataGramTime < .25:
            return

        self.lastBufferedDataGramTime = now
        self.SendDatagram(data)

    def UpdateActionsData(self, data):
        for toggle in self.toggles:
            match toggle.selectedToggleCode:
                case "ALT":
                    toggle.toggleState = data["AltitudeHold"]

                case "AP":
                    toggle.toggleState = data["AutopilotMaster"]

                case "FD":
                    toggle.toggleState = data["FlightDirector"]

                case "FLC":
                    toggle.toggleState = data["FlightLevelChange"]

                case "HDG":
                    toggle.toggleState = data["HeadingMode"]

                case "LVL":
                    toggle.toggleState = data["LevelerMode"]

                # case "NAV":
                #     toggle.toggleState = data["FlightDirector"]

                case "VS":
                    toggle.toggleState = data["VerticalSpeed"]

            toggle.UpdateVisuals()


class UdpClient(threading.Thread):
    def __init__(self, plugin):
        super().__init__(daemon=True)
        self.plugin = plugin
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.sock.sendto(b"Initialized", UdpAddress)

    def run(self):
        while True:
            data, addr = self.sock.recvfrom(100_000)
            decoded = data.decode()
            print(f"Received from {addr}: {decoded}")
            self.plugin.UpdateActionsData(json.loads(decoded))
