﻿using System;
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
    // у 'IAsyncResult' есть св-во 'AsyncState' - в него можно передать 1 об-т
    // но нам нужны аж три поля - поэтому мы создаем классс
    public class StateObject
    {
        public Socket WorkSocket { get; set; }
        public byte[] Buffer = new byte[1024];
    }

    public partial class MainWindow : Window
    {
        // нужен для обновления инф-ции в текстовом поле
        delegate void AppendText(string text);
        static int interval = 1000;
        static string message = "Hello, Networkgggggggggggggggg!";

        // создаем поток, который будет запускать 'MulticastSend'
        //Thread Sender = new Thread(new ThreadStart(MulticastSend));
        Socket socket;
        EndPoint clientEP;
       
        IAsyncResult Res;

        void AppendTextToOutput(string text)
        {
            test_txt.Text = text;
        }

        public MainWindow()
        {
            InitializeComponent();

            // ставим поток на фон
            //Sender.IsBackground = true;

            // запускаем // приложение блокировать не будет - будет работать на фоне
            //Sender.Start();
        }

        private void SendCompleted(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;

            socket.EndSend(Res);
            socket.Shutdown(SocketShutdown.Send);
            socket.Close();

            // в 'AsyncState' может наход-ся экземпляр любого класса или стр-ры
            // по факту здесь нах-ся то, что мы передаем в 'BeginReceiveFrom' последним арг-ом
            //StateObject so = (StateObject)ar.AsyncState;

            //so.WorkSocket = (Socket)ar.AsyncState;

            //socket.EndSend(Res);
            //socket.Shutdown(SocketShutdown.Send);
            //socket.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // чтобы второй раз сокет не перезаписывался
            if (socket != null)
                return;

            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint ep = new IPEndPoint(ip, 1024);
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            s.Bind(ep);
            s.Listen(10);

            try
            {
                Socket ns = s.Accept();
                //ns.Send(Encoding.ASCII.GetBytes("Hello, client!"));

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
                //txt_label.AppendText(ex.ToString());
            }
        }

        private void MulticastSend()
        {
            // этот метод будет исп-ся как потоковый // повторяться каждую секунду на фоне
            //while (true)
            //{
                // в начале каждого цикла мы ждем секунду
                //Thread.Sleep(interval);

                // Протокол 'MULTICAST' всегда 'UDP' 
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                // достать IP, если нужно (будет строка соед-ия)
                // sock.RemoteEndPoint.ToString();

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
                sock.Send(Encoding.Default.GetBytes(message));

                sock.Close();
            //}
        }

        private void send_btn_Click(object sender, RoutedEventArgs e)
        {
            MulticastSend();
        }
    }
}
