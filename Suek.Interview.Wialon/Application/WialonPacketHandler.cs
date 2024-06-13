using Suek.Interview.Wialon.WialonProtocols;

namespace Suek.Interview.Wialon.Application;

internal sealed class WialonPacketHandler {
    private readonly ILogger<WialonPacketHandler> logger;

    public WialonPacketHandler(ILogger<WialonPacketHandler> logger) {
        this.logger = logger;
    }

    public Task Handle(IWialonPacket packet, CancellationToken cancellation) {
        // По хорошему, это должен быть диспетчер, который создает для каждого нового сообещения
        // область прикладной обработки из IServiceProvider и маршрутизирует пакеты
        // до подписанных на них обработчиков.
        logger.LogInformation("Got Wialon packet: {packet}", packet);

        return Task.CompletedTask;
    }
}
