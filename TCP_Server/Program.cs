using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCP_Server
{
    class Program
    {
        static async void Main(string[] args)
        {
            var ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 15);

            TcpClient client = new TcpClient ();
            await client.ConnectAsync(ipEndPoint);
            await NetworkStream stream = client.GetStream();
            var buffer = new byte[1024];
            int recLength = await stream.ReadAsync(buffer);
            var msg = Encoding.UTF8.GetString(buffer, 0, recLength);
            Console.WriteLine($"Message: {msg}");
        }
    }
}
