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

namespace UDP_Server_async
{
    // у 'IAsyncResult' есть св-во 'AsyncState' - в него можно передать 1 об-т
    // но нам нужны аж три поля - поэтому мы создаем классс
    public class StateObject
    {
        public Socket WorkSocket { get; set; }
        public byte[] Buffer { get; set; }
        public int BufferSize { get; set; }
    }

    public partial class MainWindow : Window
    {
        delegate void AddTextDelegate(string text);

        Socket socket;
        EndPoint clientEP;

        IAsyncResult Res;

        public MainWindow()
        {
            InitializeComponent();
        }
        void Add_Text(string text)
        {
            textLabel.Text = textLabel.Text + text + "\n";
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // чтобы второй раз сокет не перезаписывался
            if (socket != null)
                return;

            //clientEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1024);

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP);

            // биндим к порту, который будем прослушивать
            socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1024));

            StateObject state = new StateObject();

            state.WorkSocket = socket;

            // clientEP создастся внутри этого метода
            Res = socket.BeginReceiveFrom(state.Buffer, 0, state.BufferSize, SocketFlags.None, 
                ref clientEP, new AsyncCallback(ReceiveCompleted), state);
            // последним арг-ом передается то, что будет у 'IAsyncResult' в св-ве 'AsyncState'
            // это позволяет передать доп инф-цию в метод, кот яв-ся колбеком
            // колбек - это ф-ция, кот вызывается, когда функция 'BeginReceiveFrom' завершится
            // те когда 'BeginReceiveFrom' завершится - у нас вызовется ф-ция 'ReceiveCompleted',
            // он вызывается без пар-ов, тк у него есть свой пар-р - 'IAsyncResult'
        }

        private void ReceiveCompleted(IAsyncResult ia)
        {
            try
            {
                // в 'AsyncState' может наход-ся экземпляр любого класса или стр-ры
                // по факту здесь нах-ся то, что мы передаем в 'BeginReceiveFrom' последним арг-ом
                StateObject so = (StateObject)ia.AsyncState;

                // достаем из него сокет
                Socket clientSocket = so.WorkSocket;

                // если нет клиентского сокета
                if (socket == null)
                    return;

                // начали там, где 'BeginReceiveFrom', а здесь мы его завершаем
                int bufferLength = clientSocket.EndReceiveFrom(Res, ref clientEP);
                // 'EndReceiveFrom' - Завершает отложенное асинхронное чтение с определенной конечной точки
                // 'Res' - Объект IAsyncResult, в котором хранятся сведения о состоянии и любые данные,
                // определенные пользователем, для этой асинхронной операции

                // получаем адрес клиента
                string strClient = ((IPEndPoint)clientEP).Address.ToString();

                string str = $"Получено от {strClient}: {Encoding.Unicode.GetString(so.Buffer, 0, bufferLength)}";

                textLabel.Dispatcher.BeginInvoke(new AddTextDelegate(Add_Text), str);
            }
            catch (SocketException)
            {

                throw;
            }
        }

        // в ассинхрнном способе нет необ-ти писать 'Thread'
    }
}
