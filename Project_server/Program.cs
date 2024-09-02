using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net; // подключаем пр-во имен 
using System.Net.Sockets; // подключаем пр-во имен 
using System.Text;

namespace Project_server
{
    // сервер запускаем первым всегда
    class Program
    {
        static void Main(string[] args)
        {
            // создаем ip-адрес у сервера
            IPAddress ip = IPAddress.Parse("127.0.0.1");

            IPEndPoint ep = new IPEndPoint(ip, 1024);

            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            Console.WriteLine("Server");

            // говорим - реагируй, когда будем обращаться к этому ip и проверяй, обращаются к твоему порту или нет
            s.Bind(ep);

            // говорим-начни слушать // число определяет, сколько max клиентов может обслуживать сервер одновременно
            s.Listen(10);

            try
            {
                while (true)
                {
                    // 'Accept' - синхронно извлекает первый ожидающий запрос на подключение из очереди запросо на подключение
                    // прослшиваемого сокета, а затем создает и возвращает новый сокет
                    Socket ns = s.Accept();

                    // выводим инф-ию об ip и о том, кто подключился
                    Console.WriteLine($"{ns.RemoteEndPoint.ToString()}: ");
                    // 'RemoteEndPoint' - получает 'EndPoint' обьект, содержащий удаленный IP-адрес и номер порта, 
                    // к кот. 'Socket' подключен обьект

                    // отправляем сообщение на подключенный сокет
                    ns.Send(Encoding.ASCII.GetBytes("Hello, client!"));

                    byte[] buff = new byte[1024];
                    int l;

                    do
                    {
                        // получаем данные из связанного об-та в приемный буфер
                        l = ns.Receive(buff);

                        // расшифровываем
                        Console.WriteLine(Encoding.ASCII.GetString(buff, 0, l));
                        // 0 - откуда начинать (сначала), 1 - сколько инф-ции брать (бери с начала с шагом - 1)
                    } while (l > 0);

                    // указываем режим - что мы блокируем ('Both': и 'send' и 'receive')
                    ns.Shutdown(SocketShutdown.Both); 

                    // закрываем соединение
                    ns.Close();
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
