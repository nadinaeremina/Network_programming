using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
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
    public partial class MainWindow : Window
    {
        Socket s;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Start();
        }
        
        public void Start()
        {
            if (s == null)
            {
                IPAddress ip = IPAddress.Parse("127.0.0.1");
                IPEndPoint ep = new IPEndPoint(ip, 1024);
                s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

                s.Bind(ep);
                s.Listen(10);
            }
          
            try
            {
                Socket ns = s.Accept();

                byte[] buff = new byte[1024];
                int l;

                do
                {
                    l = ns.Receive(buff);
                    DataTB.AppendText(Encoding.ASCII.GetString(buff, 0, l));

                } while (l > 0);

                ns.Shutdown(SocketShutdown.Both);
                ns.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void MulticastSend()
        {
            // Протокол 'MULTICAST' всегда 'UDP' 
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            // устанавливаем опции
            sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 4);
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
            sock.Send(Encoding.Default.GetBytes(DataTB.Text));
            DataTB.Text = string.Empty;

            sock.Close();

            Start();
        }

        private void send_btn_Click(object sender, RoutedEventArgs e)
        {
            if (s == null)
            {
                MessageBox.Show("At first start the server!");
                return;
            }

            MulticastSend();
        }
    }
}
