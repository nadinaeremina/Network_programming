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

namespace MultiCast_Server
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string message = "Hello, Network!";
        static int interval = 1000;

        // создаем поток, который будет запускать 'MulticastSend'
        Thread Sender = new Thread(new ThreadStart(MulticastSend));

        public MainWindow()
        {
            InitializeComponent();

            // ставим поток на фон
            Sender.IsBackground = true;

            // запускаем // приложение блокировать не будет - будет работать на фоне
            Sender.Start();
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

                // устанавливаем опции
                sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);
                // 'MulticastTimeToLive' - сколько роутеров он может пройти
                // 1 // уровень // 2 // название опции // 3 // значение 

                // создаем ip-адрес
                IPAddress dest = IPAddress.Parse("224.5.5.5");

                // создаем еще один 'sock.SetSocketOption' // это будет сокет-мультикаст
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
    }
}
