using Suek.Interview.Wialon.Application;

namespace Suek.Interview.Wialon.WialonProtocols;

internal static class WialonWebApplicationBuilderEx {
    public static void SetupWialon(this WebApplicationBuilder builder) {
        builder.Services
            .AddOptions<WialonIpsServerOptions>()
            .BindConfiguration(WialonIpsServerOptions.Section);

        builder.Services
            .AddTransient<WialonPacketHandler>();

        builder.WebHost
            .ConfigureWialonServer();
    }
}
