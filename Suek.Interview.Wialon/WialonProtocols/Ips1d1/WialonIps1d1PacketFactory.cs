using Suek.Interview.Wialon.WialonProtocols.Ips1d1.Packets;

namespace Suek.Interview.Wialon.WialonProtocols.Ips1d1;

internal static class Wialon1d1PacketFactory {
    private delegate IWialonPacket PacketHandler(string payload);

    private static readonly Dictionary<string, PacketHandler> PacketHandlers =
        new() {
            [WialonProtocol.Constants.PacketTypes.Login] = HandleLogin,
            [WialonProtocol.Constants.PacketTypes.Message] = HandleMessage,
        };

    public static bool TryCreate(string packetType, string payload, out IWialonPacket? packet) {
        packet = default;

        if (PacketHandlers.TryGetValue(packetType, out var handler) == false) {
            return false;
        }

        packet = handler(payload);

        return true;
    }
    
    private static LoginWialon1d1Packet HandleLogin(string payload) {
        var fields = payload.Split(";");

        const int deviceIdFieldIdx = 0;
        const int passwordFieldIdx = 1;

        return new(fields[deviceIdFieldIdx], fields[passwordFieldIdx]);
    }

    private static MessageWialon1d1Packet HandleMessage(string payload) {
        return new(payload);
    }
}
