namespace Suek.Interview.Wialon.WialonProtocols.Ips1d1.Packets;

internal record LoginWialon1d1Packet(string DeviceId, string Password) : IWialonPacket {
    public string Version => WialonIpsProtocol.Version.Ips1d1;
}