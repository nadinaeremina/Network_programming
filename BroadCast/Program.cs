using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BroadCast
{
    class Program
    {
        static void Main(string[] args)
        {
            ///////////////////////////////////////////// Broadcast //////////////////////////////////////

            //// так как расслаем сразу на несколько комп-ов - делаем это с пом. UDP
            //Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            //// здесь промежуток указать едьзя - только один адрес
            //IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("192.168.255.255"), 1234);

            //// 'Sock' коннектится к этому адресу
            //sock.Connect(ipep);

            //// это сообщение прийдет всем комп-ам подсети
            //sock.Send(Encoding.Default.GetBytes("Hello, Network!"));

            ///////////////////////////////////////////// MultiCast //////////////////////////////////////
            
            Socket multiSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            // устанавливаем опцию milticast
            multiSock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);
            // 1 - на каком уровне OSI мы выставляем эту опцию 
            // 2 - название опции
            // 3 - значение опции

            // если мы поставим значение - 1 - пакет не выйдет за пределы локальной сети
            // если поставим другое - датаграмма пройдет несколько роутеров
            // установка обязательно - чтобы пакет жил, иначе будет - 0, он вообще никуда не пойдет

            // создаем IP-адрес, который будет описывать конечную точку
            // создаем ip-адрес, указывающий на один из адресов, предназначенных для 'multicast'
            IPAddress dest = IPAddress.Parse("224.5.5.5.5");
            // в данном лучае конечная точка будет принадлежать одному из адресов 'multicast'

            // у нашего сокета устанавливаем еще одну опцию
            multiSock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(dest));
            // в качестве значения опции устанавливается экземпляр класса 'MultiCastOption',
            // в который мы передаем наш ip-адрес

            // создаем 'EndPoint', который ведет к этому же адресу с произвольным портом
            IPEndPoint ipep = new IPEndPoint(dest, 4567);

            // подсоединяемся к этой точке
            multiSock.Connect(ipep);

            // теперь можно из сокета отправлять наши сообщения
            multiSock.Send(Encoding.Default.GetBytes("rredyrh"));
        }
    }
}
