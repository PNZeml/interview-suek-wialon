using System.Text;

namespace Suek.Interview.Wialon.WialonProtocols.Ips1d1;

internal static class WialonIps1d1Decoder {
    public static bool TryDecodePacket(ReadOnlyMemory<byte> buffer, out IWialonPacket? packet, out int offset) {
        packet = default;

        var span = buffer.Span;

        // Seek for packet start.
        offset = span.IndexOf(WialonProtocol.Constants.Delimiter);
        if (offset == -1) {
            return false;
        }
        // Add '#' character to offset
        offset++;
        
        var packetTypeLength = span[offset..].IndexOf(WialonProtocol.Constants.Delimiter);
        if (packetTypeLength == -1) {
            return false;
        }

        var packetType = Encoding.UTF8.GetString(span.Slice(offset, packetTypeLength));

        // Add '#' character to offset and length of Packet Type as UTF8 characters
        offset += 1 + packetTypeLength;

        var packetTailIdx = span[offset..].IndexOfAny(WialonProtocol.Constants.PacketEnd);
        var payload = Encoding.UTF8.GetString(span.Slice(offset, packetTailIdx));

        if (Wialon1d1PacketFactory.TryCreate(packetType, payload, out packet) == false) {
            return false;
        }

        offset += packetTailIdx + 2;
    
        return true;
    }
}