using System.Globalization;
using Suek.Interview.Wialon.WialonProtocols.Ips1d1.Packets;

namespace Suek.Interview.Wialon.WialonProtocols.Ips1d1;

internal static class Wialon1d1PacketFactory {
    private const string DateTimeFormat = "ddMMyy HHmmss";

    private delegate IWialonPacket PacketHandler(string payload);

    private static readonly Dictionary<WialonIpsProtocol.Packets.Definition, PacketHandler> PacketHandlers =
        new() {
            [WialonIpsProtocol.Packets.Login]     = HandleLogin,
            [WialonIpsProtocol.Packets.Message]   = HandleMessage,
            [WialonIpsProtocol.Packets.ShortData] = HandleShortData,
        };

    public static bool TryCreate(
        WialonIpsProtocol.Packets.Definition packetDefinition,
        string payload,
        out IWialonPacket? packet
    ) {
        packet = default;

        if (PacketHandlers.TryGetValue(packetDefinition, out var handler) == false) {
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
            throw new("Invalid Short Data packet");
        }

        var collectedAt = DateTimeOffset
            .ParseExact(
                $"{fields[collectedAtDataFieldIdx]} {fields[collectedAtTimeFieldIdx]}",
                DateTimeFormat,
                CultureInfo.InvariantCulture.DateTimeFormat,
                DateTimeStyles.AssumeUniversal
            );

        const string notAvailable = "NA";

        return new(
            CollectedAt: collectedAt,
            Location: new(
                ParseDouble(fields[latitudeFieldIdx]),
                fields[northEastFieldIdx],
                ParseDouble(fields[longitudeFieldIdx]),
                fields[eastWestFieldIdx]
            ),
            Speed: fields[speedFieldIdx] == notAvailable
                ? null : Convert.ToInt32(fields[speedFieldIdx]), 
            Course: fields[courseFieldIdx] == notAvailable
                ? null : Convert.ToInt32(fields[courseFieldIdx]), 
            Height: fields[altitudeFieldIdx] == notAvailable
                ? null : Convert.ToInt32(fields[altitudeFieldIdx]), 
            Satellites: fields[satellitesFieldIdx] == notAvailable
                ? null : Convert.ToInt32(fields[satellitesFieldIdx]) 
        );

        double ParseDouble(string number) {
            return double.Parse(number, NumberStyles.Any, CultureInfo.InvariantCulture);
        }
    }
}
