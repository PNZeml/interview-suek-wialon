using System.Net;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Suek.Interview.Wialon.WialonProtocols.Ips1d1;

namespace Suek.Interview.Wialon.WialonProtocols;

internal static class WialonKestrelEx {
    public static IWebHostBuilder ConfigureWialonServer(this IWebHostBuilder builder) {
        builder.ConfigureKestrel(AddWialonServer);

        return builder;
    }
    private static void AddWialonServer(WebHostBuilderContext context, KestrelServerOptions options) {
        // TODO: Refactor
        var wialonOptions = context.Configuration.GetRequiredSection(WialonServerOptions.Section)
            .Get<WialonServerOptions>();

        if (wialonOptions == null) {
            throw new("Wialon server options has not been provided");
        }

        foreach (var l in wialonOptions.Listeners) {
            options.Listen(IPAddress.Parse(l.Host), l.Port, static options => {
                options.UseConnectionHandler<WialonIps1d1ConnectionHandler>();
            });
        }
    }
}
