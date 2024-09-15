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
using System.Net.Sockets;
using System.Net;

namespace Project_server_WPF_async
{
    public class StateObject
    {
        public Socket WorkSocket { get; set; }
        public byte[] Buffer = new byte[65536];
    }

    public partial class MainWindow : Window
    {
        delegate void AddTextDelegate(string text);

        Socket socket;
        EndPoint clientEP;

        IAsyncResult Res;

        Create create = new Create();
        public MainWindow()
        {
            InitializeComponent();

            create.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        void Add_Text(string text)
        {
            txt_label.Text = txt_label.Text + text + "\n";
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

                clientEP = new IPEndPoint(ip, create.My_port);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP);

                socket.Bind(clientEP);

                StateObject state = new StateObject();
                state.WorkSocket = socket;

                Res = socket.BeginReceiveFrom(state.Buffer, 0, state.Buffer.Length, SocketFlags.None,
                ref clientEP, new AsyncCallback(ReceiveCompleted), state);
            }
            else
                return;
        }

        private void ReceiveCompleted(IAsyncResult ar)
        {
            try
            {
                // в 'AsyncState' может наход-ся экземпляр любого класса или стр-ры
                // по факту здесь нах-ся то, что мы передаем в 'BeginReceiveFrom' последним арг-ом
                StateObject so = (StateObject)ar.AsyncState;

                // достаем из него сокет
                Socket clientSocket = so.WorkSocket;

                // если нет клиентского сокета
                if (socket == null)
                    return;

                // начали там, где 'BeginReceiveFrom', а здесь мы его завершаем
                int bufferLength = clientSocket.EndReceiveFrom(ar, ref clientEP);
                // 'EndReceiveFrom' - Завершает отложенное асинхронное чтение с определенной конечной точки
                // 'Res' - Объект IAsyncResult, в котором хранятся сведения о состоянии и любые данные,
                // определенные пользователем, для этой асинхронной операции

                // получаем адрес клиента
                string strClient = ((IPEndPoint)clientEP).Address.ToString();

                string str = $"Получено от {strClient}: {Encoding.Unicode.GetString(so.Buffer, 0, bufferLength)}";

                txt_label.Dispatcher.BeginInvoke(new AddTextDelegate(Add_Text), str);
            }
            catch (SocketException)
            {
                throw;
            }
        }
    }
}
