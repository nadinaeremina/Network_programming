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

namespace Chat_Server_WPF
{
    // нужен для обновления инф-ции в текстовом поле
    delegate void AppendText(string text);
    public partial class MainWindow : Window
    {
        void AppendTextToOutput(string text)
        {
            OutputDataTB.Text = text;
        }

        static string message = "Hello, Network!";
        static int interval = 1000;

        // создаем поток, который будет запускать 'MulticastSend'
        Thread Sender = new Thread(new ThreadStart(MulticastSend));
        Thread Receiver;

        public MainWindow()
        {
            InitializeComponent();

            Receiver = new Thread(new ThreadStart(Receive));

            // ставим поток на фон
            Sender.IsBackground = true;
            Receiver.IsBackground = true;

            // запускаем // приложение блокировать не будет - будет работать на фоне
            Sender.Start();
            //Receiver.Start();
        }

        static void MulticastSend()
        {
            // этот метод будет исп-ся как потоковый // повторяться каждую секунду на фоне
            while (true)
            {
                // в начале каждого цикла мы ждем секунду
                Thread.Sleep(interval);

                // Протокол 'MULTICAST' всегда 'UDP' 
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                // достать IP, если нужно (будет строка соед-ия)
                // sock.RemoteEndPoint.ToString();


                // создаем ip-адрес
                IPAddress dest = IPAddress.Parse("224.5.6.7");

                // устанавливаем опции

                // Задаем время жизни для сокета — это очень важно для возможностей многоадресной передачи данных.
                // Значение 1 означает что многоадресная передача данных не выйдет за пределы локальной сети
                // Установка значения >1  позволит многоадресной передаче данных пройти через несколько маршрутизаторов
                // Каждый маршрутизатор будет уменьшать значение TTL на 1.
                sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);
                // 'MulticastTimeToLive' - сколько роутеров он может пройти
                // 1 // уровень // 2 // название опции // 3 // значение 


                // создаем еще один 'sock.SetSocketOption' // это будет сокет-мультикаст
                sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(dest));
                // если какой-то сокет из этого ip - делает рассылку - то все остальные сокеты будут ее получать

                // Здесь мы создаем конечную точку, которая позволяет нам отправлять и передавать данные, то есть, мы связываем сокет с этой конечной точкой
                // Теперь мы полноправные члены группы многоадресной рассылки и можем передавать данные в группу.
                IPEndPoint ipep = new IPEndPoint(dest, 4567);

                // подключаемя к этому 'IPEndPoint'
                sock.Connect(ipep);

                // посылаем сообщение
                sock.Send(Encoding.Default.GetBytes(message));

                sock.Close();
            }
        }

        void Receive()
        {
            while (true)
            {
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                // создаем 'EndPoint' для прослушки
                IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 4567);
                // IPAddress.Any - будет прослушивать все, порт как у сервера

                // биндим сокет к нашему 'IPEndPoint'
                sock.Bind(ipep);

                // создаем новый ip-адрес
                IPAddress ip = IPAddress.Parse("224.5.5.4");

                sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, IPAddress.Any));
                // если какой-то сокет из этого ip - делает рассылку - то все остальные сокеты будут ее получать
                // здесь мы подписываем сокет на адрес, который описывает multicast - 'ip'
                // 'IPAddress.Any' ждем сигнал от любого адреса с этой группы, от всех, кто подписался

                byte[] buff = new byte[1024];

                // здесь пр-ма встанет и будет ждать, пока не прийдет ответ от сервера
                sock.Receive(buff);
                // 'Receive' измеряет размер сообщения, в то время как буффер яв-ся массивом, поэтому он будет изменяться внутри метода

                this.Dispatcher.Invoke(new AppendText(AppendTextToOutput), Encoding.Default.GetString(buff));
                // обращаемся в главном потоке

                sock.Close();
            }
        }

        private void OutputDataTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            message = OutputDataTB.Text;
        }

        private void InputDataTB_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
    }
}
