using System.Buffers;

namespace Suek.Interview.Wialon.WialonProtocols.Ips1d1;

internal static class WialonIps1d1Encoder {
    public static ArraySegment<byte> Encode(IWialonPacket packet) {
        var buffer = ArrayPool<byte>.Shared.Rent(256);

        try {

            var offset = 0;

            buffer[offset++] = WialonIpsProtocol.Constants.PacketStart;

            WialonIpsProtocol.Packets.Login.Ack!.Bytes.CopyTo(buffer, offset);
            offset += 2;

            buffer[offset++] = WialonIpsProtocol.Constants.PacketStart;

            // TODO: Fix magic string
            WialonIpsProtocol.Packets.Login.AckCodes["Success"].CopyTo(buffer, offset);
            offset += WialonIpsProtocol.Packets.Login.AckCodes["Success"].Length;

            buffer[offset++] = 0x0D;
            buffer[offset++] = 0x0A;

            var encodedPacket = new ArraySegment<byte>(buffer, 0, offset);

            return encodedPacket;
        } finally {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}
