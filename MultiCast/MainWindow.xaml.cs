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

namespace MultiCast
{
    // нужен для обновления инф-ции в текстовом поле
    delegate void AppendText(string text);
    public partial class MainWindow : Window
    {
        // создаем 'thread' для прослушивания
        Thread listener;

        void AppendTextToOutput(string text)
        {
            OutputDataTB.Text = text;
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

        // будет прослушивать сообщения от сервера в потоке
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
    }
}
