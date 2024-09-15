﻿using System;
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

namespace Project_client_WPF_async
{
    public partial class MainWindow : Window
    {

        private void SendCompleted(IAsyncResult ia)
        {
            Socket socket = (Socket)ia.AsyncState;

            socket.EndSend(Res);
            socket.Shutdown(SocketShutdown.Send);
            socket.Close();
        }

        Create create = new Create();

        IAsyncResult Res;

        public MainWindow()
        {
            InitializeComponent();

            create.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP);

            // тк здесь мы используем UDP, а он не устанавливает постоянного соединения - 's.Connect(ep)' - не нужно,
            // мы вместо этого у 'BeginSendTo' и 'BeginReceiveTO' в качестве одного из пар-ов передаем 'EndPoint'
            Res = socket.BeginSendTo(Encoding.Unicode.GetBytes(txt_label.Text), 0, txt_label.Text.Length,
                SocketFlags.None, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1024), new AsyncCallback(SendCompleted), socket);
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            create.ShowDialog();
        }
    }
}
