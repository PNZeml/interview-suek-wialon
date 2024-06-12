using System.Buffers;

namespace Suek.Interview.Wialon.WialonProtocols.Ips1d1;

internal static class WialonIps1d1Encoder {
    public static ArraySegment<byte> Encode(IWialonPacket packet) {
        var buffer = ArrayPool<byte>.Shared.Rent(256);

        var offset = 0;

        buffer[offset++] = WialonProtocol.Constants.Delimiter;

        WialonProtocol.Constants.PacketTypes
            .LoginAckBytes.CopyTo(buffer, offset);
        offset += 2;

        buffer[offset++] = WialonProtocol.Constants.Delimiter;

        // Ack Status
        WialonProtocol.Constants.Acks.Login
            .Success.CopyTo(buffer, offset);
        offset += WialonProtocol.Constants.Acks.Login
            .Success.Length;

        buffer[offset++] = 0x0D;
        buffer[offset++] = 0x0A;

        var bytes = new ArraySegment<byte>(buffer, 0, offset);
        
        ArrayPool<byte>.Shared.Return(buffer);

        return bytes;
    }
}
