using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chat_Client_WPF
{
    // нужен для обновления инф-ции в текстовом поле
    delegate void AppendText(string text);

    public partial class MainWindow : Window
    {
        static string message = "";
        static int interval = 3000;

        // создаем 'thread' для прослушивания
        Thread listener;

        void AppendTextToOutput(string text)
        {
            OutputDataTB.Text = text;
        }

        public MainWindow()
        {
            InitializeComponent();

            listener = new Thread(new ThreadStart(Listen));

            // ставим его на фон
            listener.IsBackground = true;

            // запускаем
            listener.Start();
        }

        //void Send()
        //{
        //    listener.Abort(); // 1

        //    while (true)
        //    {
        //        // в начале каждого цикла мы ждем секунду
        //        Thread.Sleep(interval);

        //        IPAddress ip = IPAddress.Parse("224.5.5.6");
        //        IPEndPoint ep = new IPEndPoint(ip, 4567);
        //        Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        //        s.Connect(ep);

        //        if (s.Connected)
        //        {
        //            s.Send(Encoding.ASCII.GetBytes(InputDataTB.Text));
        //        }

        //        s.Shutdown(SocketShutdown.Both);
        //        s.Close();
        //    }

        //    listener.Start(); //2
        //}

        private void InputDataTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            message = InputDataTB.Text;
        }

        // будет прослушивать сообщения от сервера в потоке
        void Listen()
        {
            // этот метод будет блокировать пр-му каждую секунду, пока сервер не пришлет ответ
            while (true)
            {
                // в начале каждого цикла мы ждем секунду
                Thread.Sleep(interval);

                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                // создаем 'EndPoint' для прослушки на входящие данные на любой IP адрес, на порт 4567
                IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 4567);
                // IPAddress.Any - будет прослушивать все, порт как у сервера

                // биндим сокет к нашему 'IPEndPoint'
                sock.Bind(ipep);

                // Присоединяемся к multicast группе 224.5.6.7

                // 1 // создаем новый ip-адрес
                IPAddress ip = IPAddress.Parse("224.5.5.5");

                // 2
                sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, IPAddress.Any));
                // если какой-то сокет из этого ip - делает рассылку - то все остальные сокеты будут ее получать
                // здесь мы подписываем сокет на адрес, который описывает multicast - 'ip'
                // 'IPAddress.Any' ждем сигнал от любого адреса с этой группы, от всех, кто подписался

                // Получаем данные отправленные в multicast группу.
                byte[] buff = new byte[1024];

                // здесь пр-ма встанет и будет ждать, пока не прийдет ответ от сервера
                sock.Receive(buff);
                // 'Receive' измеряет размер сообщения, в то время как буффер яв-ся массивом, поэтому он будет изменяться внутри метода

                this.Dispatcher.Invoke(new AppendText(AppendTextToOutput), Encoding.Default.GetString(buff));
                // обращаемся в главном потоке

                sock.Close();
            }
        }

        private void send_btn_Click(object sender, RoutedEventArgs e)
        {
            listener.Abort();

            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            // тк здесь мы используем UDP, а он не устанавливает постоянного соединения - 's.Connect(ep)' - не нужно,
            // мы вместо этого у 'SendTo' и 'ReceiveTO' в качестве одного из пар-ов передаем 'EndPoint'
            s.SendTo(Encoding.Unicode.GetBytes(InputDataTB.Text), new IPEndPoint(IPAddress.Parse("224.5.5.6"), 4567));
            // первым арг-ом - указываем само сообщение, вторым - куда мы посылаем

            // тк у нас нет постоянного соед-ия - мы сокеты сразу закрываем
            s.Shutdown(SocketShutdown.Send);
            s.Close();

            listener.Start();
        }
    }
}
