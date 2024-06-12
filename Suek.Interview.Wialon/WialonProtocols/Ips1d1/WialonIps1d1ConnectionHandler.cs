using System.Buffers;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http.Features;
using Suek.Interview.Wialon.Utils;

namespace Suek.Interview.Wialon.WialonProtocols.Ips1d1;

internal interface IWialonPacket;

internal sealed class WialonIps1d1ConnectionHandler : ConnectionHandler {
    
    private static readonly TimeSpan ConsumeTimeout = TimeSpan.FromSeconds(90.0d);

    public override async Task OnConnectedAsync(ConnectionContext connection) {
        var reader = connection.Transport.Input;
        var writer = connection.Transport.Output;

        var connectionLifetime = GetConnectionLifetime();

        try {
            while (true) {
                using var consumeTimeoutCancellation = new CancellationTokenSource(ConsumeTimeout);

                using var combinedCancellation = CancellationTokenSource
                    .CreateLinkedTokenSource(
                        connectionLifetime.ConnectionClosedRequested,
                        consumeTimeoutCancellation.Token
                    );

                var readResult = await reader.ReadAsync(combinedCancellation.Token);

                var buffer = readResult.Buffer;
                var (consumed, observed) = (buffer.Start, buffer.Start);

                if (WialonIps1d1Decoder.TryDecode(ByteReader.GetBuffer(ref buffer), out var packet) == false) {
                    
                }

                reader.AdvanceTo(buffer.End, buffer.End);

                // Читать секцию с типами пактов, пока не встретим '#'
                // Читать сообщение, пока не встретим конец пакета
            }
        } finally {
            
        }

        IConnectionLifetimeNotificationFeature GetConnectionLifetime() {
            return connection.Features
                .GetRequiredFeature<IConnectionLifetimeNotificationFeature>();
        }
    }

}