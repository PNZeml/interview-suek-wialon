using System.Net;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using Suek.Interview.Wialon.WialonProtocols.Ips1d1;

namespace Suek.Interview.Wialon.WialonProtocols;

internal static class WialonKestrelEx {
    public static IWebHostBuilder ConfigureWialonServer(this IWebHostBuilder builder) {
        builder.ConfigureKestrel(AddWialonServer);

        return builder;
    }
    private static void AddWialonServer(WebHostBuilderContext context, KestrelServerOptions options) {
        var opt = options.ApplicationServices
            .GetRequiredService<IOptions<WialonIpsServerOptions>>().Value;

        foreach (var l in opt.Listeners) {
            options.Listen(IPAddress.Parse(l.Host), l.Port, static options => {
                options.UseConnectionHandler<WialonIps1d1ConnectionHandler>();
            });
        }
    }
}
