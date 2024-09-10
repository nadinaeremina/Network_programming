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
        Thread listener, sender_;

        void AppendTextToOutput(string text)
        {
            OutputDataTB.Text = text;
        }

        public MainWindow()
        {
            InitializeComponent();

            listener = new Thread(new ThreadStart(Listen));
            //sender_ = new Thread(new ThreadStart(Send));

            // ставим его на фон
            listener.IsBackground = true;
            //sender_.IsBackground = true;

            // запускаем
            listener.Start();
            // sender_.Start();
        }

        void Send()
        {
            while (true)
            {
                // в начале каждого цикла мы ждем секунду
                Thread.Sleep(interval);

                // создаем UDP-сокет
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                // достать IP, если нужно (будет строка соед-ия)
                // sock.RemoteEndPoint.ToString();

                // устанавливаем опции
                sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);
                // 'MulticastTimeToLive' - сколько роутеров он может пройти
                // 1 // уровень // 2 // название опции // 3 // значение 

                // создаем ip-адрес
                IPAddress dest = IPAddress.Parse("224.5.5.4");

                // создаем еще один 'sock.SetSocketOption' // это будет сокет-мультикаст // для присоед-ия к группе
                sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(dest));
                // если какой-то сокет из этого ip - делает рассылку - то все остальные сокеты будут ее получать


                IPEndPoint ipep = new IPEndPoint(dest, 4567);

                // подключаемя к этому 'IPEndPoint'
                sock.Connect(ipep);

                // посылаем сообщение
                sock.Send(Encoding.Default.GetBytes(message));

                sock.Close();
            }
        }

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
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                // создаем 'EndPoint' для прослушки на входящие данные на любой IP адрес, на порт 4567
                IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 4567);
                // IPAddress.Any - будет прослушивать все, порт как у сервера

                // биндим сокет к нашему 'IPEndPoint'
                sock.Bind(ipep);

                // Присоединяемся к multicast группе 224.5.6.7

                // 1 // создаем новый ip-адрес
                IPAddress ip = IPAddress.Parse("224.5.6.7");

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
    }
}
