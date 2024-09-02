using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net; // подключаем пр-во имен 
using System.Net.Sockets; // подключаем пр-во имен 
using System.Text;

namespace Async_sockets
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket s;

            //s.AcceptAsync();

            // s.BeginAccept(); // пишем там, где начинаем получать данные
            // s.EndAccept(); // получение данных становится критично // мы дожидаемся, что мы их получим

            // s.ConnectAsync();

            // s.BeginConnect(); // начинаем действие, оно происходит на фоне (в потоке)
            // s.EndConnect(); // дожидаемся и заканчиваем (точка, в которой мы находимся)

            // s.DisconnectAsync();

            // s.BeginDisconnect();
            // s.EndDisconnect();

            // s.ReceiveAsync();

            // s.BeginReceive();
            // s.EndReceive();

            // s.SendAsync();

            // s.BeginSend();
            // s.EndSend();


        }
    }
}
