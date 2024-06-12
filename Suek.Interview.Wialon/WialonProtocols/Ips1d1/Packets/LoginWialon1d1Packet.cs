namespace Suek.Interview.Wialon.WialonProtocols.Ips1d1.Packets;

internal record LoginWialon1d1Packet(string DeviceId, string Password) : IWialonPacket {
    public string Version => WialonProtocol.Versions.Ips11;
}

internal record MessageWialon1d1Packet(string Mesage) : IWialonPacket {
    public string Version => WialonProtocol.Versions.Ips11;
}