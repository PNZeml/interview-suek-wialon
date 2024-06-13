using System.Text;

namespace Suek.Interview.Wialon.WialonProtocols.Ips1d1;

internal static class WialonIps1d1Decoder {
    /// <summary>
    ///
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="packet"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public static bool TryDecodePacket(ReadOnlySpan<byte> bytes, out IWialonPacket? packet, out int offset) {
        packet = default;

        // Получение начальной позиции пакета
        offset = bytes.IndexOf(WialonIpsProtocol.Constants.PacketStart);
        if (offset == -1) {
            return false;
        }
        // Добавить символ '#' в сдвиг
        offset++;

        var packetTypeLength = bytes[offset..].IndexOf(WialonIpsProtocol.Constants.PacketStart);
        if (packetTypeLength == -1) {
            return false;
        }

        var packetType = Encoding.UTF8.GetString(bytes.Slice(offset, packetTypeLength));

        // Добавить символ '#' в сдвиг и количество символов в опрееделнии типа пакета.
        offset += 1 + packetTypeLength;

        var packetTailIdx = bytes[offset..].IndexOfAny(WialonIpsProtocol.Constants.PacketEnd);
        // Используется прямой перевод строки, т.к. нет условия по опитимальному чтению. Можно читать байты
        // до разделителя, но это приведет все к темже аллокациям новых строк.
        var packetPayload = Encoding.UTF8.GetString(bytes.Slice(offset, packetTailIdx));

        var packetDefinition = WialonIpsProtocol.Packets.ByName(packetType);

        if (Wialon1d1PacketFactory.TryCreate(packetDefinition, packetPayload, out packet) == false) {
            return false;
        }

        offset += packetTailIdx + 2;

        // Мы можем не расспарсить пакет, так как не получили всех байт, поэтому нужно отделньо
        // учитывать сдвиги по "обработанным" и "просмотренным" байтам. Но эта потенциальная
        // проблема не описывалась в условиях задачи.
        return true;
    }
}
