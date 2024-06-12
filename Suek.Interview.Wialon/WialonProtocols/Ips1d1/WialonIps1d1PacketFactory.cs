using System.Globalization;
using Suek.Interview.Wialon.WialonProtocols.Ips1d1.Packets;

namespace Suek.Interview.Wialon.WialonProtocols.Ips1d1;

internal static class Wialon1d1PacketFactory {
    private delegate IWialonPacket PacketHandler(string payload);

    private static readonly Dictionary<string, PacketHandler> PacketHandlers =
        new() {
            [WialonProtocol.Constants.PacketTypes.Login] = HandleLogin,
            [WialonProtocol.Constants.PacketTypes.Message] = HandleMessage,
            [WialonProtocol.Constants.PacketTypes.ShortData] = HandleShortData,
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

    private static ShortDataWialon1d1Packet HandleShortData(string payload) {
        var fields = payload.Split(";");

        const int collectedAtDataFieldIdx = 0;
        const int collectedAtTimeFieldIdx = 1;
        const int latitudeFieldIdx        = 2;
        const int northEastFieldIdx       = 3;
        const int longitudeFieldIdx       = 4;
        const int eastWestFieldIdx        = 5;
        const int speedFieldIdx           = 6;
        const int courseFieldIdx          = 7;
        const int altitudeFieldIdx        = 8;
        const int satellitesFieldIdx      = 9;

        if (fields.Length != 10) {
            throw new Exception("Invalid Short Data packet");
        }

        var collectedAt = DateTimeOffset
            .ParseExact(
                $"{fields[collectedAtDataFieldIdx]} {fields[collectedAtTimeFieldIdx]}",
                "ddMMyy HHmmss",
                CultureInfo.InvariantCulture.DateTimeFormat
            );

        return new(
            collectedAt,
            new(
                ParseDouble(fields[latitudeFieldIdx]),
                fields[northEastFieldIdx],
                ParseDouble(fields[longitudeFieldIdx]),
                fields[eastWestFieldIdx]
            ),
            fields[speedFieldIdx] == "NA" ? null : Convert.ToInt32(fields[speedFieldIdx]), 
            fields[courseFieldIdx] == "NA" ? null : Convert.ToInt32(fields[courseFieldIdx]), 
            fields[altitudeFieldIdx] == "NA" ? null : Convert.ToInt32(fields[altitudeFieldIdx]), 
            fields[satellitesFieldIdx] == "NA" ? null : Convert.ToInt32(fields[satellitesFieldIdx]) 
        );

        double ParseDouble(string number) {
            return double.Parse(number, NumberStyles.Any, CultureInfo.InvariantCulture);
        }
    }
}
