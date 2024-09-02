using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace UDP_Server
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        delegate void AddTextDelegate(string text);

        Socket socket;
        Thread thread;
        public MainWindow()
        {
            InitializeComponent();
        }

        void AddText(string text)
        {
            TextLabel.Content = TextLabel.Content.ToString() + text;
        }

        void ReceiveFunction(object obj)
        {
            Socket rs = (Socket)obj;
            byte[] buffer = new byte[1024];

            do
            {
                EndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1024);
                int l = rs.ReceiveFrom(buffer, ref ep);
                string strClientIP = ((IPEndPoint)ep).Address.ToString();
                string str = $"Получено от {strClientIP}: {Encoding.Unicode.GetString(buffer, 0, l)}";
                TextLabel.Dispatcher.BeginInvoke(AddText, str);

            } while (true);
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            if (socket != null && thread != null)
                return;

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP);
            socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1024);
            thread = new Thread(ReceiveFunction);
            thread.Start(socket);
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (socket != null)
            {
                thread.Abort();
                thread = null;
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket = null;
                TextLabel.Content = string.Empty;
            }
        }
    }
}
