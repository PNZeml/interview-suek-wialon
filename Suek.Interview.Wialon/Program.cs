using Suek.Interview.Wialon.Application;
using Suek.Interview.Wialon.WialonProtocols;

var builder = WebApplication.CreateBuilder(args);

builder.Logging
    .AddFile<BasicFormatter>(
        configuration => {
            configuration.Directory = @".\";
            configuration.UseRollingFiles = true;
            configuration.RollingFileTimestampFormat = @"yyyy-MM-dd";
            configuration.FileExtension = "log";
            configuration.FileNamePrefix = "suek-wialon";
            configuration.MinimumLogLevel = LogLevel.Debug;
        },
        formatter => {
            formatter.CaptureScopes = true;
            formatter.UseUtcTimestamp = true;
            formatter.IncludePID = true;
            formatter.IncludeUser = true;
        }
    );

builder.Services
    .AddTransient<DevicePacketHandler>();

builder.WebHost
    .ConfigureWialonServer();

var app = builder.Build();

app.Run();