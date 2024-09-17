using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCP_Client
{
    class Program
    {
        static async void Main(string[] args)
        {
            var ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5001);

            var client = new TcpClient(ep);
            // 'AddressFamily' - может быть 'InterNetwork', 'InterNetworkV6', 'Unknown'

            var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

            socket.Bind(ep);

            client.Connect(ep);
            socket.Connect(ep);

            ep = new IPEndPoint(IPAddress.Any, 5000);

            var listener = new TcpListener(ep); // IPAddress.Any, 5000

            var lsock = new Socket(ep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            listener.Start(10);

            lsock.Listen(10);

            Socket asock = await listener.AcceptSocketAsync();
            asock = await lsock.AcceptAsync();

            NetworkStream stream; // последовательность байт
        }
    }
}
