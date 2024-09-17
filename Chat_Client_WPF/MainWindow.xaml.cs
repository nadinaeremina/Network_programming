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
using System.Text.Json;

namespace Chat_Client_WPF
{
    // нужен для обновления инф-ции в текстовом поле
    delegate void AppendText(List<string> user);

    public partial class MainWindow : Window
    {
        // создаем 'thread' для прослушивания
        Thread listener;

        void AppendTextToOutput(List<string> user)
        {
            Random rand = new Random();
            int color = rand.Next(150, 255);

            TextBox txt1 = new TextBox();
            TextBox txt2 = new TextBox();

            txt1.IsEnabled = true;
            txt2.IsEnabled = true;

            txt1.BorderThickness = new Thickness(0);
            txt2.BorderThickness = new Thickness(0);

            txt1.Text = user[0] + ":";
            txt1.Foreground = new SolidColorBrush(Color.FromRgb(Convert.ToByte(color), 0, 0));

            txt2.Text = user[1];

            RecTB.Children.Add(txt1);
            RecTB.Children.Add(txt2);
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
        void Listen()
        {
            // этот метод будет блокировать пр-му каждую секунду, пока сервер не пришлет ответ
            while (true)
            {
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                // создаем 'EndPoint' для прослушки
                IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 4567);
                // IPAddress.Any - будет прослушивать все, порт как у сервера

                // биндим сокет к нашему 'IPEndPoint'
                sock.Bind(ipep);

                // создаем новый ip-адрес
                IPAddress ip = IPAddress.Parse("224.5.5.5");

                sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, IPAddress.Any));
                // если какой-то сокет из этого ip - делает рассылку - то все остальные сокеты будут ее получать
                // здесь мы подписываем сокет на адрес, который описывает multicast - 'ip'
                // 'IPAddress.Any' ждем сигнал от любого адреса с этой группы, от всех, кто подписался

                string json = "";
                byte[] buff = new byte[1024];
                int l;

                // здесь пр-ма встанет и будет ждать, пока не прийдет ответ от сервера
                l = sock.Receive(buff);
                // 'Receive' измеряет размер сообщения, в то время как буффер яв-ся массивом, поэтому он будет изменяться внутри метода

                json += Encoding.ASCII.GetString(buff, 0, l);

                User user = JsonSerializer.Deserialize<User>(json);

                List<string> user_list = new List<string>();

                user_list.Add(user.Name);
                user_list.Add(user.Message);

                this.Dispatcher.Invoke(new AppendText(AppendTextToOutput), user_list);
                // обращаемся в главном потоке

                sock.Close();
            }
        }

        private void send_btn_Click(object sender, RoutedEventArgs e)
        {
            if (txt_name.Text.Length > 0)
            {
                if (DataTB.Text.Length > 0)
                {
                    IPAddress ip = IPAddress.Parse("127.0.0.1");
                    IPEndPoint ep = new IPEndPoint(ip, 1024);
                    Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

                    try
                    {
                        s.Connect(ep);

                        if (s.Connected)
                        {
                            User user = new User { Name = txt_name.Text, Message = DataTB.Text };
                            string json = JsonSerializer.Serialize(user);
                            s.Send(Encoding.ASCII.GetBytes(json));
                            DataTB.Text = string.Empty;

                            s.Shutdown(SocketShutdown.Both);
                            s.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Server is not answering!");
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Enter your name!");
            }
        }
    }
}
