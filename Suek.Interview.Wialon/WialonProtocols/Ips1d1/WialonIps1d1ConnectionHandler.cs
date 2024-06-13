using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using Suek.Interview.Wialon.Application;
using Suek.Interview.Wialon.Utils;
using Suek.Interview.Wialon.WialonProtocols.Ips1d1.Packets;

namespace Suek.Interview.Wialon.WialonProtocols.Ips1d1;

/// <summary>
/// Обрботчки соединения устройства предающего информацию по протоклу Wialon IPS 1.1.
/// </summary>
internal sealed class WialonIps1d1ConnectionHandler : ConnectionHandler {
    private readonly WialonPacketHandler packetHandler;

    private readonly WialonIpsServerOptions options;

    private readonly ILogger<WialonIps1d1ConnectionHandler> logger;

    private TimeSpan ConsumeTimeout => options.NoPacketTimeout;

    public WialonIps1d1ConnectionHandler(
        WialonPacketHandler packetHandler,
        IOptions<WialonIpsServerOptions> options,
        ILogger<WialonIps1d1ConnectionHandler> logger
    ) {
        this.packetHandler = packetHandler;
        this.options = options.Value;
        this.logger = logger;
    }

    public override async Task OnConnectedAsync(ConnectionContext connection) {
        // Есть вариант спрятать логгирования в "миддлвэр", но для этого лучше уже реализовывать
        // "хаб" подключенных устройств, а пайплайн по обработке пакетов оставить здесь и вызвать при
        // запросе следующего пакета на уровне нашего кастомного фреймворка.
        logger.LogDebug("Wialon Device has been connected");

        var reader = connection.Transport.Input;
        var writer = connection.Transport.Output;

        var lifetimeCancellation = GetConnectionLifetime().ConnectionClosedRequested;

        WialonDeviceContext? wialonDeviceContext = null;

        try {
            while (true) {
                using var timeoutCancellation = new CancellationTokenSource(ConsumeTimeout);

                using var cancellation = CancellationTokenSource
                    .CreateLinkedTokenSource(lifetimeCancellation, timeoutCancellation.Token);

                var readResult = await reader.ReadAsync(cancellation.Token);

                if (readResult.IsCompleted) {
                    logger.LogDebug("Wialon Device has finished sending packets");

                    throw new("Consuming has been completed");
                }

                var buffer = readResult.Buffer;

                var isPacketDecoded = WialonIps1d1Decoder.TryDecodePacket(
                    ByteReader.GetBuffer(ref buffer).Span, out var packet, out var consumed
                );

                if (isPacketDecoded == false) {
                    // Данное исключение приведет к обрыву соединения, т.к. пока не было условия
                    // на валидацию и отправки соотвествующего кода.
                    throw new("Could not decode consuming packet");
                }

                if (packet is LoginWialon1d1Packet p) {
                    wialonDeviceContext = new(p.DeviceId);
                    // TODO: Fix device logger scope init
                    wialonDeviceContext.LogScope =
                        logger.BeginScope("WialonDeviceId: {DeviceId}", wialonDeviceContext.DeviceId);
                }

                await packetHandler.Handle(packet!, cancellation.Token);

                var consumedPosition = buffer.Slice(0, consumed).End;

                reader.AdvanceTo(consumedPosition, consumedPosition);
                // Формирование ответа лучше также вынести в отдлеьный метод и вызвать через
                // кастомный фреймврок.
                var answer = WialonIps1d1Encoder.Encode(packet!);

                await writer.WriteAsync(answer, cancellation.Token);
            }
        } catch (Exception exception) {
            await reader.CompleteAsync(exception);
            await writer.CompleteAsync(exception);

            throw;
        } finally {
            wialonDeviceContext?.Dispose();

            logger.LogDebug("Wialon device has been disconnected");
        }

        return;

        IConnectionLifetimeNotificationFeature GetConnectionLifetime() {
            return connection.Features
                .GetRequiredFeature<IConnectionLifetimeNotificationFeature>();
        }
    }

    private record WialonDeviceContext(string DeviceId) : IDisposable {
        public IDisposable? LogScope;

        public void Dispose() {
            LogScope?.Dispose();
        }
    }
}
