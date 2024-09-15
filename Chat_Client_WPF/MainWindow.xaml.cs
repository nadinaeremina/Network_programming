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
    // у 'IAsyncResult' есть св-во 'AsyncState' - в него можно передать 1 об-т
    // но нам нужны аж три поля - поэтому мы создаем классс
    public class StateObject
    {
        public Socket WorkSocket { get; set; }
        public byte[] Buffer = new byte[1024];
    }
    // нужен для обновления инф-ции в текстовом поле
    delegate void AppendText(string text);

    public partial class MainWindow : Window
    {    // создаем 'thread' для прослушивания
        Thread listener;
        Socket socket;
        EndPoint clientEP;

        IAsyncResult Res;

        void AppendTextToOutput(string text)
        {
            DataTB.Text = text;
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

                byte[] buff = new byte[1024];

                // здесь пр-ма встанет и будет ждать, пока не прийдет ответ от сервера
                sock.Receive(buff);
                // 'Receive' измеряет размер сообщения, в то время как буффер яв-ся массивом, поэтому он будет изменяться внутри метода

                this.Dispatcher.Invoke(new AppendText(AppendTextToOutput), Encoding.Default.GetString(buff));
                // обращаемся в главном потоке

                sock.Close();
            }
        }
        //private void ReceiveCompleted(IAsyncResult ar)
        //{
        //    try
        //    {
        //        // в 'AsyncState' может наход-ся экземпляр любого класса или стр-ры
        //        // по факту здесь нах-ся то, что мы передаем в 'BeginReceiveFrom' последним арг-ом
        //        //StateObject so = (StateObject)ar.AsyncState;

        //        // достаем из него сокет
        //        Socket clientSocket = (Socket)ar.AsyncState;

        //        // если нет клиентского сокета
        //        if (clientSocket == null)
        //            return;

        //        // начали там, где 'BeginReceiveFrom', а здесь мы его завершаем
        //        int bufferLength = clientSocket.EndReceiveFrom(ar, ref clientEP);
        //        // 'EndReceiveFrom' - Завершает отложенное асинхронное чтение с определенной конечной точки
        //        // 'Res' - Объект IAsyncResult, в котором хранятся сведения о состоянии и любые данные,
        //        // определенные пользователем, для этой асинхронной операции

        //        // получаем адрес клиента
        //        string strClient = ((IPEndPoint)clientEP).Address.ToString();

        //        byte[] Buffer = new byte[1024];

        //        string str = $"Получено от {strClient}: {Encoding.Unicode.GetString(Buffer, 0, bufferLength)}";

        //        DataTB.Dispatcher.BeginInvoke(new AppendText(AppendTextToOutput), str);
        //    }
        //    catch (SocketException)
        //    {
                
        //        throw;
        //    }
        //    finally
        //    {
        //        socket.Close();
        //    }
        //}

        private void send_btn_Click(object sender, RoutedEventArgs e)
        {
            // чтобы второй раз сокет не перезаписывался
            if (socket != null)
                return;

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
                        s.Send(Encoding.ASCII.GetBytes(DataTB.Text));
                    }
                }
                catch (Exception ex)
                {
                    //txt_label.AppendText(ex.ToString());
                }
                finally
                {
                    s.Shutdown(SocketShutdown.Both);
                    s.Close();
                }
            }

        }
    }
}
