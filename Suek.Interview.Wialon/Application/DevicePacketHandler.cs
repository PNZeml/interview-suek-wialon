using Suek.Interview.Wialon.WialonProtocols;

namespace Suek.Interview.Wialon.Application;

internal sealed class DevicePacketHandler {
    private readonly ILogger<DevicePacketHandler> logger;

    public DevicePacketHandler(ILogger<DevicePacketHandler> logger) {
        this.logger = logger;
    }

    public async Task Handle(IWialonPacket packet, CancellationToken cancellation) {
        logger.LogInformation("Got Wialon packet: {packet}", packet);
    }
}
