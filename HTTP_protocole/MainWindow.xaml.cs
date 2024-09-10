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
using System.IO;

namespace HTTP_protocole
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //// класс, описывающий запрос
            //HttpWebRequest request; // наследник класса 'WebRequest'

            //// класс, описывающий ответ
            //HttpWebResponse response; // наследник класса 'WebResponse'

            //// методы отправки и получения данных содержатся в этом класса
            //WebClient client;

            //// класс для обработки конкретного соединения с URI
            //ServicePoint SP; // обработка соед-ия с конкретным адресом 

            //// класс, который позволяет управлять об-ми 'ServicePoint'
            //ServicePointManager servicePointManager;

            //// сам 'URI' не ввиде строки
            //Uri uri;

            //// для удобства создания
            //UriBuilder uriBuilder;
        }

        private void MakeRequestbutton_Click(object sender, RoutedEventArgs e)
        {
            // класс, описывающий запрос
            HttpWebRequest request; // наследник класса 'WebRequest'

            // класс, описывающий ответ
            HttpWebResponse response; // наследник класса 'WebResponse'

            // чтобы отправить запрос - нужно его создать
            // вызываем у класса 'HttpWebRequest' статическую ф-цию
            // 1 // request = (HttpWebRequest)HttpWebRequest.Create("http://top-academy.ru");
            // нужно явное преобразование, потому что этот метод считается устаревшим

            // 2
            request = (HttpWebRequest)HttpWebRequest.Create(RequestTB.Text);

            // можем перед отправкой установить какие-то заголовки // кол-ция заголовков
            request.Headers.Add("Accept-Language: ru-ru"); // запрашиваем страницу на русском языке

            //////////// некоторые аголовки недоступны через 'Headers.Add', они доступны через св-ва
            //request.Accept;
            //request.Connection;
            //request.ContentLength;
            //request.Expect;
            //request.IfModifiedSince;
            //request.Referer;
            //request.TransferEncoding;
            //request.ContentType;
            //request.UserAgent;

            // для получения исп-ем след метод // делаем запрос к веб-серверу
            response = (HttpWebResponse)request.GetResponse(); // здесь пр-ма будет заблокирована

            // ответ будет нах-ся в неком потоке // обявляет нач. заголовок с некой строкой
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default);
            // вернет соответ-ий битовый поток

            // ответ в виде текста - расшифровываем в строчку
            // у ответа можем почитать заголовки
            string data = string.Empty;

            foreach (string header in response.Headers) // 'Headers' - словарь
            {
                data += $"{header}: {response.Headers[header]}\n"; 
                // перечисляем св-ва заголовка
            }

            data += sr.ReadToEnd();

            // читаем
            sr.Close();

            OutputTB.Text = data;
        }
    }
}
