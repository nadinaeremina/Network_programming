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

        // ф-ция callback - будет вызываться, когда выполнился 'Accept'
        void MyAcceptCallback(IAsyncResult ia)
        {
            // получаем ссылку на тот сокет, который нас прослушивает
            Socket socket = (Socket)ia.AsyncState;

            // здесь заканчивается 'Accept'// получаем сокет для обмена данными с клиентом
            Socket ns = socket.EndAccept(ia);

            // выводим в консоль данные о подключившемся клиенте
            Console.WriteLine(ns.RemoteEndPoint.ToString());

            // создаем буфер
            byte[] buffer = Encoding.ASCII.GetBytes(DateTime.Now.ToString());

            // ассинхронная ф-ция
            ns.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(MySendCallback), ns);
            // сам буфер, с какого эл-та и сколько эл-ов, отправлять стандартным образом (дополнительно ничего не нужно)
            // какую ф-цию вызывать, когда 'Send' завершится

            // привязываем 'callback' к 'accept' - чтобы дождаться ответа
            socket.BeginAccept(new AsyncCallback(MyAcceptCallback), socket);
            // 'BeginAccept' требует 2 пар-ра - функцию 'AsyncCallback', которая в кон-ре требует ф-цию,
            // которая будет вызываться в качестве 'callback'
            // когда 'accept'  у сокета заверщится - вызовется эта ф-ция 'MyAcceptCallback'
            // и сокет, с кот. мы получаем данные
        }

        private void MySendCallback(IAsyncResult ia)
        {
            Socket ns = (Socket)ia.AsyncState;
            // 'ns' - клиентский сокет вызвал ф-цию

            int n = ns.EndSend(ia);
            ns.Shutdown(SocketShutdown.Send);
            ns.Close();
        }

        public void StartServer()
        {
            if (socket != null)
                return;

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            socket.Bind(endP);
            socket.Listen(10);
            socket.BeginAccept(new AsyncCallback(MyAcceptCallback), socket);   

            // т.о. для каждого клиента будет свой поток
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
