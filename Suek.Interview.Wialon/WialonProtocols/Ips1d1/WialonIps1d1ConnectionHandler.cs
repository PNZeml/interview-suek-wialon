using System.Buffers;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http.Features;
using Suek.Interview.Wialon.Application;
using Suek.Interview.Wialon.Utils;

namespace Suek.Interview.Wialon.WialonProtocols.Ips1d1;

internal sealed class WialonIps1d1ConnectionHandler : ConnectionHandler {
    private readonly DevicePacketHandler packetHandler;

    private static readonly TimeSpan ConsumeTimeout = TimeSpan.FromSeconds(10.0d);

    public WialonIps1d1ConnectionHandler(DevicePacketHandler packetHandler) {
        this.packetHandler = packetHandler;
    }

    public override async Task OnConnectedAsync(ConnectionContext connection) {
        var reader = connection.Transport.Input;
        var writer = connection.Transport.Output;

        var lifetimeCancellation = GetConnectionLifetime().ConnectionClosedRequested;

        try {
            while (true) {
                using var timeoutCancellation = new CancellationTokenSource(ConsumeTimeout);
                
                using var cancellation = CancellationTokenSource
                    .CreateLinkedTokenSource(lifetimeCancellation, timeoutCancellation.Token);

                var readResult = await reader.ReadAsync(cancellation.Token);
                if (readResult.IsCompleted) {
                    throw new Exception("Consuming has been completed");
                }

                var buffer = readResult.Buffer;

                var isPacketDecoded = WialonIps1d1Decoder.TryDecodePacket(
                    ByteReader.GetBuffer(ref buffer), out var packet, out var consumed
                );

                if (isPacketDecoded == false) {
                    throw new Exception("Could not decode consuming packet");
                }

                await packetHandler.Handle(packet, cancellation.Token);

                var consumedPosition = buffer.Slice(0, consumed).End;

                reader.AdvanceTo(consumedPosition, consumedPosition);

                var answer = WialonIps1d1Encoder.Encode(packet);

                await writer.WriteAsync(answer, cancellation.Token);
            }
        } catch (Exception exception) {
            await reader.CompleteAsync(exception);
            await writer.CompleteAsync(exception);

            throw;
        }

        IConnectionLifetimeNotificationFeature GetConnectionLifetime() {
            return connection.Features
                .GetRequiredFeature<IConnectionLifetimeNotificationFeature>();
        }
    }

}