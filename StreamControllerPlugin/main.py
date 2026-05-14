# Import libs
import json
import socket
import threading
import time

# Import StreamController modules
from src.backend.PluginManager.PluginBase import PluginBase
from src.backend.PluginManager.ActionHolder import ActionHolder

# Import actions
from .Actions.ToggleAction import ToggleAction

PluginId = "com_TomaszSmigielski_MsFs2024Connector"
UdpAddress = ("127.0.0.1", 13337)


class MsFsConnector(PluginBase):
    def __init__(self):
        super().__init__()
        self.actions = set()
        self.lastBufferedDataGramTime = 1

        # Register actions
        self.toggleActionHolder = ActionHolder(
            plugin_base=self,
            action_base=ToggleAction,
            action_id=PluginId + "::ToggleAction",
            action_name="Toggle",
        )

        self.add_action_holder(self.toggleActionHolder)

        # Register plugin
        self.register(
            plugin_name="Microsoft Flight Simulator 2024 Connector",
            github_repo="https://github.com/StreamController/PluginTemplate",
            app_version="1.1.1-alpha"
        )

        # Start Udp client
        self.udpClient = UdpClient(self)
        self.udpClient.start()

    def RegisterAction(self, action):
        self.actions.add(action)

    def SendDatagram(self, data):
        self.udpClient.sock.sendto(data, UdpAddress)

    def SendBufferedDatagram(self, data):
        now = time.perf_counter()
        if now - self.lastBufferedDataGramTime < .25:
            return

        self.lastBufferedDataGramTime = now
        self.SendDatagram(data)

    def UpdateActionsData(self, data):
        for action in self.actions:
            match action.selectedToggleCode:
                case "AP":
                    action.toggleState = data["AutopilotMaster"]

                case "FD":
                    action.toggleState = data["FlightDirector"]

                case "FLC":
                    action.toggleState = data["FlightLevelChange"]

                # case "NAV":
                #     action.toggleState = data["FlightDirector"]

                case "VS":
                    action.toggleState = data["VerticalSpeed"]

            action.UpdateVisuals()


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
