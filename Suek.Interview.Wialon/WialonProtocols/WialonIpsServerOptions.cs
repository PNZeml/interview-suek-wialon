namespace Suek.Interview.Wialon.WialonProtocols;

public class WialonIpsServerOptions {
    public const string Section = "Wialon";

    public ICollection<Listener> Listeners { get; set; } = null!;

    public TimeSpan NoPacketTimeout { get; set; } = TimeSpan.FromSeconds(30.0d);

    public class Listener {
        public string Host { get; init; } = "0.0.0.0";

        public int Port { get; init; } = 8080;

        public string Version { get; init; } = WialonIpsProtocol.Version.Ips1d1;
    }

}
