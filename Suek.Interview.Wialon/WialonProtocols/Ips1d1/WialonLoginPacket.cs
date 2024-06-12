namespace Suek.Interview.Wialon.WialonProtocols.Ips1d1;

internal record WialonLoginPacket(string DeviceId, string Password) : IWialonPacket;
