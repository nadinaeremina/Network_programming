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
using System.Net; 
using System.Net.Sockets;

namespace Project_server_WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Create create = new Create();
        public MainWindow()
        {
            InitializeComponent();
            create.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            create.ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (create.My_ip.Length > 0 && create.My_port.ToString().Length > 0)
            {
                IPAddress ip = IPAddress.Parse(create.My_ip);
                IPEndPoint ep = new IPEndPoint(ip, create.My_port);
                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                
                s.Bind(ep);
                s.Listen(10);

                try
                {
                    while (true)
                    {
                        Socket ns = s.Accept();
                        ns.Send(Encoding.ASCII.GetBytes("Hello, client!"));

                        byte[] buff = new byte[1024];
                        int l;

                        do
                        {
                            l = ns.Receive(buff);
                            txt_label.AppendText(Encoding.ASCII.GetString(buff, 0, l));

                        } while (l > 0);
                        
                        ns.Shutdown(SocketShutdown.Both);
                        ns.Close();
                    }
                }
                catch (Exception ex)
                {
                    txt_label.AppendText(ex.ToString());
                }
            }
        }
    }
}
