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
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace UDP_Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP);

            // тк здесь мы используем UDP, а он не устанавливает постоянного соединения - 's.Connect(ep)' - не нужно,
            // мы вместо этого у 'SendTo' и 'ReceiveTO' в качестве одного из пар-ов передаем 'EndPoint'
            socket.SendTo(Encoding.Unicode.GetBytes(MessageTB.Text), new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1024));
            // первым арг-ом - указываем само сообщение, вторым - куда мы посылаем

            // тк у нас нет постоянного соед-ия - мы сокеты сразу закрываем
            socket.Shutdown(SocketShutdown.Send);
            socket.Close();
        }
    }
}


