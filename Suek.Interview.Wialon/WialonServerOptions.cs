
using Suek.Interview.Wialon.WialonProtocols;

namespace Suek.Interview.Wialon;

public class WialonServerOptions {
    public const string Section = "Wialon";
    
    public ICollection<Listener> Listeners { get; init; }

    public class Listener {
        public string Host { get; init; } = "0.0.0.0";

        public int Port { get; init; } = 8080;

        public string Version { get; init; } = WialonProtocol.Versions.Ips11;
    }

}