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
    // по сути это синхронный режим
    // но полноценно организовать его на UDP синронно не можем - нужно выносить в отдельный поток

    public partial class MainWindow : Window
    {
        delegate void AddTextDelegate(string text);

        Socket socket;
        Thread thread;

        public MainWindow()
        {
            InitializeComponent();
        }

        void Add_Text(string text)
        {
            textLabel.Text = textLabel.Text + text + "\n";
        }

        void ReceiveFunction(object obj) // сюда в методе 'StartButton_Click' передали сокет
        {
            // получающий сокет
            Socket rs = (Socket)obj;

            byte[] buffer = new byte[1024];

            // просходит в бесконечном цикле, потому что 'ReceiveFunction' запускается в отдельном 'Thread'
            // на фоне ждет новых сообщений от новых клиентов
            do
            {
                EndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1024); // клиент

                // 'ReceiveFrom' - получаем инф-цию с помощью протокола UDP 
                // яв-ся блокирующей ф-цией - должен стоять и ждать, пока не прийдет какое-то сообщение
                // от клиента, который подключается по этому сокету - 'ep'
                int l = rs.ReceiveFrom(buffer, ref ep); // 'ref ep' - данные клиента
                // 'ref ep' - Ссылка на EndPoint объект того же типа,
                // что и конечная точка удаленного узла, обновляемая при успешном получении
                // 'l' - Количество полученных байтов

                // ((IPEndPoint)ep) - получаем IP клиента // кто прислал сообщение, с какого адреса
                string strClientIP = ((IPEndPoint)ep).Address.ToString();

                string str = $"Получено от {strClientIP}: {Encoding.Unicode.GetString(buffer, 0, l)}";

                // 'BeginInvoke' - Выполняет делегат асинхронно в потоке, в котором был создан базовый дескриптор элемента управления
                textLabel.Dispatcher.BeginInvoke(new AddTextDelegate(Add_Text), str);
                // тк это не главный поток - доступ к об-ам, которые созданы в главном потоке мы не имеем
                // все об-ты WPF создаются в главном потоке
                // как из доп потока направить сообщение в главный - с помощью 'Dispatcher' (он есть у каждого потока)
                // 'Dispatcher' - об-т, который в рамках одного прцесса осуществляет взаимдействие между его потоками
                // все об-ты WPF унаследованы от 'Dispatcher Object', чтобы иметь возм-ть взаимодействовать м/у разными потоками одного процесса
                // 'Dispatcher' - переводит направление делегата в главный поток
                // 'Add_Text' - будет вызван в главном потоке и соот-но изменит 'textLabel' в главном потоке

            } while (true);
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (socket != null && thread != null)
                return;

            // 'SocketType.Dgram' - тк мы создаем UDP-сокет, а не TCP
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP);

            // привязываем сокет к 'IPRndPoint'
            socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1024));
            // 'Listen' - не нужен, тк мы не слушаем в пост режиме, мы просто ждем, когда из неоткуда прилетают сообщения

            // будет запускаться в фоновом потоке
            thread = new Thread(ReceiveFunction);
            // выносим в отдельный поток - тк метод 'ReceiveFrom' - это блокирующая ф-ция
            // иначе она просто встанет и будет ждать, не даст в основном потоке вып-ся программе дальше

            thread.Start(socket); // 'Start' передает об-ты как 'object'
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (socket != null)
            {
                // завершаем поток
                thread.Abort();

                // очищаем ссылку
                thread = null;

                // завершаем сокет
                socket.Shutdown(SocketShutdown.Both);

                // закрываем сокет
                socket.Close();

                // очищаем ссылку
                socket = null;

                // очистили контент
                textLabel.Text = string.Empty;
            }
        }
    }
}
