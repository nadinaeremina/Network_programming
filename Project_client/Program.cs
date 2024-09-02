using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net; // подключаем пр-во имен 
using System.Net.Sockets; // подключаем пр-во имен 
using System.Text;

namespace Project_client
{
    // клиент запускается вторым
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");

            IPEndPoint ep = new IPEndPoint(ip, 1024);

            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            Console.WriteLine("Client");

            try
            {
                s.Connect(ep);

                if (s.Connected)
                {
                    string strSend = "Hello, server!";

                    // отправляем сообщение на подключенный сокет
                    s.Send(Encoding.ASCII.GetBytes(strSend));

                    byte[] buff = new byte[1024];
                    int l;

                    do
                    {
                        // получаем данные из связанного об-та в приемный буфер
                        l = s.Receive(buff);

                        // расшифровываем
                        Console.WriteLine(Encoding.ASCII.GetString(buff, 0, l));
                        // 0 - откуда начинать (сначала), 1 - сколько инф-ции брать (бери с начала с шагом - 1)

                    } while (l > 0);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToString());
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
