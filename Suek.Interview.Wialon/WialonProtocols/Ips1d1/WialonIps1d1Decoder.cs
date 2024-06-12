using System.Text;

namespace Suek.Interview.Wialon.WialonProtocols.Ips1d1;

internal static class WialonIps1d1Decoder {
    public static bool TryDecode(ReadOnlyMemory<byte> buffer, out IWialonPacket? packet) {
        packet = default;

        var span = buffer.Span;

        // Seek for packet start.
        var offset = span.IndexOf(WialonProtocol.Constants.Delimiter);
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

        offset += packetTailIdx;
    
        return true;
    }
}

internal static class WialonIps1d1Encoder {
    public static bool Encode() {
        // TODO: Encode

        // var wOffset = 0;
        // var pool    = ArrayPool<byte>.Create();
        // var b       = pool.Rent(128);
        //
        // b[wOffset++] = WialonProtocol.Constants.Delimiter;
        // // Ack Type
        // WialonProtocol.Constants.LoginAckBytes.CopyTo(b, wOffset);
        // wOffset += 2;
        // b[wOffset++] = WialonProtocol.Constants.Delimiter;
        // // Ack Status
        // b[wOffset++] = 0x31;
        // b[wOffset++] = 0x0D;
        // b[wOffset++] = 0x0A;
        //
        // var r = new ArraySegment<byte>(b.AsSpan(..wOffset).ToArray());
        //
        // pool.Return(b);
        //         
        // var writeResult = await writer.WriteAsync(r, combinedCancellation.Token);
        throw new NotImplementedException();
    }
}