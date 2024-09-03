using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net; // подключаем пр-во имен 
using System.Net.Sockets; // подключаем пр-во имен 
using System.Text;

namespace Passive_socket_async
{
    class AsyncServer
    {
        IPEndPoint endP;
        Socket socket;
        public AsyncServer(string strAddr, int port)
        {
            endP = new IPEndPoint(IPAddress.Parse(strAddr), port);
        }

        /*
         Колбэк-функция (функция обратного вызова) — это функция, предназначенная для отложенного выполнения. 
         Она должна быть выполнена после завершения работы другой функции.

         Колбэк-функции неразрывно связаны с асинхронностью и позволяют «запланировать» действие, 
         которое будет совершено после выполнения какого-то другого, возможно длительного действия.
         */

        // ф-ция callback - будет вызываться, когда выполнился 'Accept' 
        void MyAcceptCallback(IAsyncResult ia) // принимает 'IAsyncResult', как все ф-ции callback
        {
            // IAsyncResult ia - какой-то обьект, реализующий интерфейс 'IAsyncResult'
            // Представляет состояние асинхронной операции

            // 'IAsyncResult' - объекты также передаются в методы,
            // вызываемые делегатами AsyncCallback при завершении асинхронной операции

            // Объект, поддерживающий 'IAsyncResult' интерфейс, сохраняет сведения о состоянии 
            // для асинхронной операции и предоставляет объект синхронизации, 
            // позволяющий сигнализировать потоки после завершения операции.

            // тк наша ф-ция callback будет вызывать об-т, слушающий сокет
            // получаем ссылку на тот сокет, который нас прослушивает
            Socket socket = (Socket)ia.AsyncState;
            // 'AsyncState' - Получает определенный пользователем объект, который определяет
            // или содержит сведения об асинхронной операции.

            // тк мы где-то там начали 'Accept' - здесь мы его заканчиваем
            // получаем сокет для обмена данными с клиентом
            Socket ns = socket.EndAccept(ia);
            // public System.Net.Sockets.Socket EndAccept(IAsyncResult asyncResult);
            // 'EndAccept' - асинхронно принимает попытку входящего подключения
            // и создает новый объект Socket для связи с удаленным узлом.

            // выводим в консоль данные о подключившемся клиенте
            Console.WriteLine(ns.RemoteEndPoint.ToString());
            // 'RemoteEndPoint' - получает EndPoint объект , содержащий удаленный IP-адрес и номер порта, к которому Socket подключен объект

            // создаем буфер
            byte[] buffer = Encoding.ASCII.GetBytes(DateTime.Now.ToString());

            // ассинхронная ф-ция // отправляем данные
            ns.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(MySendCallback), ns);
            // сам буфер, с какого эл-та и сколько эл-ов, отправлять стандартным образом (дополнительно ничего не нужно)
            // какую ф-цию вызывать, когда 'Send' завершится
            // 'BeginSend' можно создавать сколько угодно - каждый в своем потоке

            // возобновляем наш ассинхронный 'Accept'
            // привязываем 'callback' к 'accept' - чтобы дождаться ответа
            socket.BeginAccept(new AsyncCallback(MyAcceptCallback), socket);
            // 'BeginAccept' требует 2 пар-ра - функцию 'AsyncCallback', которая в кон-ре требует ф-цию,
            // которая будет вызываться в качестве 'callback'
            // когда 'accept'  у сокета заверщится - вызовется эта ф-ция 'MyAcceptCallback'
            // и сокет, с кот. мы получаем данные
        }

        private void MySendCallback(IAsyncResult ia)
        {
            Socket ns = (Socket)ia.AsyncState; // 'AsyncState' - это об-т, который вызвал ф-цию 'BeginSend'
            // 'ns' - клиентский сокет, который вызвал ф-цию

            int n = ns.EndSend(ia); // Завершает отложенную операцию асинхронной передачи
            // Если операция завершилась успешно — значение количества байтов, переданных в объект Socket;
            // в противном случае — ошибка, указывающая на недопустимость объекта Socket

            ns.Shutdown(SocketShutdown.Send); // 'Shutdown' - Блокирует передачу и получение данных для объекта Socket
            ns.Close();
        }

        public void StartServer()
        {
            // не запускаем сервер, который уже запущен
            if (socket != null)
                return;

            // запускаем // инициализируем сокет
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            
            socket.Bind(endP);
            socket.Listen(10);

            // начинаем принимать сообщения от клиентов
            // 'BeginAccept' - Начинает асинхронную операцию, чтобы принять попытку входящего подключения.
            socket.BeginAccept(new AsyncCallback(MyAcceptCallback), socket);
            // 1-ый аргумент - Делегат 'AsyncCallback', 2-ой - Объект, содержащий сведения о состоянии для этого запроса
            // Возвращаемое значение - Объект 'IAsyncResult', который ссылается на асинхронное создание объекта Socket
            // т.о. для каждого клиента будет свой поток
            // Ждем, когда подключится клиент - передаем ему данные и начинаем новый 'BeginAccept' -
            // снова ждем, когда подключится новый клиент - получаем рекурсивный цикл (бесконечный)
            // то для каждого клиента будет свой поток
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // создаем новый экземпляр сервера
            AsyncServer server = new AsyncServer("127.0.0.1", 1024); // подключаться будем по локал хосту
            server.StartServer();
            Console.Read(); // чтобы пр-ма продолжала работать, пока мы не нажмем какую-то клавишу
        }

        // если мы работаем с протоколом 'TCP' - он устанавливает постоянное соединение
        // если мы работаем с 'UDP' - то будем исп-ть 'Sendto' и 'ReceiveFrom'
    }
}
