namespace Suek.Interview.Wialon.WialonProtocols.Ips1d1.Packets;

internal record ShortDataWialon1d1Packet(
    DateTimeOffset CollectedAt,
    GpsLocation Location,
    int? Speed,
    int? Course,
    int? Height,
    int? Satellites
) : IWialonPacket {
    public string Version => WialonIpsProtocol.Version.Ips1d1;
}

public record GpsLocation(
    double Latitude,
    string NorthEast,
    double Longitude,
    string EastWest
);