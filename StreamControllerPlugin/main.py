# Import libs
import asyncio

# Import StreamController modules
from src.backend.PluginManager.PluginBase import PluginBase
from src.backend.PluginManager.ActionHolder import ActionHolder

# Import actions
from .Actions.ToggleAction import ToggleAction

PluginId = "com_TomaszSmigielski_MsFs2024Connector"


class MsFsConnector(PluginBase):
    def __init__(self):
        super().__init__()

        self.udpTransport = None

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

        # Start Udp loop
        asyncio.run(self.UdpLoop())

    async def UdpLoop(self):
        self.loop = asyncio.get_running_loop()

        await self.loop.create_datagram_endpoint(
            lambda: UdpHandler(self),
            remote_addr=("127.0.0.1", 13337)
        )


class UdpHandler(asyncio.DatagramProtocol):
    def __init__(self, plugin):
        self.plugin = plugin

    def connection_made(self, udpTransport):
        self.plugin.udpTransport = self.updTransport = udpTransport
        self.updTransport.sendto(b"Hello async UDP!")

    def datagram_received(self, data, addr):
        print("Received:", data.decode())
