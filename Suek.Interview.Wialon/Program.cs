using System.Net;
using Microsoft.AspNetCore.Connections;
using Suek.Interview.Wialon;
using Suek.Interview.Wialon.Application;
using Suek.Interview.Wialon.WialonProtocols.Ips1d1;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddTransient<DevicePacketHandler>();

builder.WebHost.ConfigureKestrel((context, options) => {
    // TODO: Refactor
    var wialonOptions = context.Configuration
        .GetRequiredSection(WialonServerOptions.Section)
        .Get<WialonServerOptions>();

    foreach (var l in wialonOptions.Listeners) {        
        options.Listen(IPAddress.Parse(l.Host), l.Port, static options => {
            options.UseConnectionHandler<WialonIps1d1ConnectionHandler>();
        });
    }
});

var app = builder.Build();

app.Run();