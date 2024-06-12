using System.Buffers;
using System.Text;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http.Features;

namespace Suek.Iterview.Wialon;

public sealed class WialonIps1d1ConnectionHandler : ConnectionHandler {
    
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
            
                // Получаем пачку байт в передаче в буффер
                var readResult = await reader.ReadAsync(combinedCancellation.Token);
            
                // Обрабатываем пакет
                var buffer = readResult.Buffer;
                var (consumed, observed) = (buffer.Start, buffer.Start);

                var offset = 0;
            
                // Найти стартовый байт '#'
                var wialonPacketStart = buffer.FirstSpan[offset++];

                if (wialonPacketStart != WialonProtocol.Constants.Delimiter) {
                    throw new Exception();
                }

                var foo = buffer.FirstSpan[offset..].ToArray();

                var delimeterIdx = buffer.FirstSpan.Slice(offset)
                    .IndexOf(WialonProtocol.Constants.Delimiter);

                var wialontPacketType = Encoding.UTF8
                    .GetString(buffer.FirstSpan[offset..(offset + delimeterIdx)]);

                var wOffset = 0;
                var pool    = ArrayPool<byte>.Create();
                var b       = pool.Rent(128);

                b[wOffset++] = WialonProtocol.Constants.Delimiter;
                // Ack Type
                WialonProtocol.Constants.LoginAckBytes.CopyTo(b, wOffset);
                wOffset += 2;
                b[wOffset++] = WialonProtocol.Constants.Delimiter;
                // Ack Status
                b[wOffset++] = 0x31;
                b[wOffset++] = 0x0D;
                b[wOffset++] = 0x0A;

                var r = new ArraySegment<byte>(b.AsSpan(..wOffset).ToArray());

                pool.Return(b);
                
                var writeResult = await writer.WriteAsync(r, combinedCancellation.Token);
           
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