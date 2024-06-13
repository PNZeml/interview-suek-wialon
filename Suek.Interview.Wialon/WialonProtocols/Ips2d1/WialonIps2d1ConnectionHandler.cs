using Microsoft.AspNetCore.Connections;

namespace Suek.Interview.Wialon.WialonProtocols.Ips2d1;

internal sealed class WialonIps2d1ConnectionHandler : ConnectionHandler {

    public override Task OnConnectedAsync(ConnectionContext connection) {
        // Задачи реализовывать не было. Можно разместить на другом порту.
        throw new NotImplementedException();
    }

}
