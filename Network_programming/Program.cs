using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net; // подключаем пр-во имен 
using System.Net.Sockets; // подключаем пр-во имен 
using System.Text;

namespace Active_socket
{
    // будем из нашей пр-мы обращаться к сокетам 
    // из сеансового уровня запрашивать к сетевому - создать сокет
    class Program
    {
        static void Main(string[] args)
        {
            // Socket s; // этот класс реализует на плаформе .NET интерфейс Berkeley
            // имеет набор методов и св-в д/реализации сетевого взаимодействия 
            // позволяет осуществлять передачу данных с исп-ем коммуникационных протоколов

            // s.ProtocolType = ProtocolType.IP; // в 'ProtocoleType' есть мн-во протоколов
            // при исп-ии 'IP' можем уточнить, мы исп-ем 4 или 6 версию 'IP', UDP и TSP тут тоже есть

            /////////////////// Установление синхронного соед-ия со стороны клиента /////////////////////
            ///Активный сокет

            // создаем ip-адрес
            IPAddress ip = IPAddress.Parse("5.255.255.77"); // класс, который будет содержать в себе ip-адрес
            // можем создать его из строки с пом. метода 'Parse' - 'фабричный метод'

            // создаем порт
            IPEndPoint ep = new IPEndPoint(ip, 80); // класс, кот. описывает порт для разных протоколов
            // вторым пар-ом требует порт (80 - стандартный порт для передачи http-сообщений)

            // Создаем об-т 'Socket' 
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            // требует принять семейство адресов 
            // чаще всего будем выбирать 'InterNetwork' - соответствует обычной передаче 
            // тк данные будем передавать по 'TSP' - 'SocketType' - 'Stream' (соотв-ет протоколу 'TCP' на транспортном уровне)
            // на сетевом уровне будем исп-ть протокол 'IP'

            try
            {
                s.Connect(ep); // подключаем наш 'end point' к указанному сокету // подключаемся к порту 80

                // синхронное подключение - пр-ма стоит и ждет, удалось или нет подключитья
                if(s.Connected) // если удалось подключиться
                {
                    // на прикладном уровне посылаем запрос на определенном протоколе
                    string strSend = "GET\r\n\r\n"; // здесь формируется наш http
                    // дальше эту полученную строчку мы кодируем в послед-ть байт - возникает представительный уровень
                    // эту послед-ть байт мы отправляем по сокету
                    s.Send(Encoding.ASCII.GetBytes(strSend));
                    // отправляем get-запрос, зашифровываем в ASCII, преобразовываем в байт и отправляем в сокет

                    // для ответа создаем буфер, который будет состоять из 1024 символов
                    byte[] buff = new byte[1024];
                    int l;

                    do
                    {
                        // получаем данные от сокета - ответ кладем в буфер
                        l = s.Receive(buff);
                        // 'l' содержит длину полученной информации

                        // расшифровываем его по мере подключения
                        // мы получаем данные в виде послед-ти байт и преобразовываем в строку
                        Console.WriteLine(Encoding.ASCII.GetString(buff, 0, l));
                        
                    } while (l > 0); // пока что-то в ответе содержится

                    Console.WriteLine(); // выводим данные в консоль
                }
                else
                {
                    throw new Exception("Connection Error");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToString());
                Console.ForegroundColor = ConsoleColor.White;
            }
            finally
            {
                s.Shutdown(SocketShutdown.Both); // блокируем передачу данных
                s.Close(); // закрываем и освобождаем ресурс
            }

            // Синхронный режим: 
            //s.Connect(ep);
            //if (s.Connected)
            //    s.Send(Encoding.ASCII.GetBytes("GET\r\n\r\n"));
        }
    }
}
