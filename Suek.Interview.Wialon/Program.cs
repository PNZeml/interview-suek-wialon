using Suek.Interview.Wialon;
using Suek.Interview.Wialon.WialonProtocols;

var builder = WebApplication.CreateBuilder(args);

builder.SetupWialon();

builder.Logging.AddFileDefault();

var app = builder.Build();

app.Run();
