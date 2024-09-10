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

using System.Net.Http; // подключаем
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace HTTP_Client
{
    // простые классы для банальных записей
    // поля класса будут совпадать с полями возвращаемого 'json'
    //public record class Todo(
    //    int? UserId = null,
    //    int? Id = null,
    //    string? Titile = null,
    //    bool? Completedd = null
    //    );
    public partial class MainWindow : Window
    {
        // этот класс будет отвечать за вып-ие запросов
        private HttpClient client;
        private string jsonPlaceholder = "http://jsonplaceholder.typecode.com/";
        public MainWindow()
        {
            InitializeComponent();

            // создаем экземпляр, ему следует передать URI, к которому будем подключатья
            client = new HttpClient
            {
                // встроена сис-ма взаим-ия с адресами // кладем адрес
                BaseAddress = new Uri(jsonPlaceholder),
                // сколько будем ждать, прежде чем запрос будет считаться потерянным
                // по умолчанию - 100 секунд // лучше не ставить меньше 15 секунд
                Timeout = TimeSpan.FromSeconds(120)
            };
            // для 'client' базовым адресом яв-ся текущий адрес, который готов к нему подключаться
        }

        private async void Do_request_Click(object sender, RoutedEventArgs e)
        {
            // получение ответа // в 'GetAsync' можно предоставить доп инф-цию к адресу
            using HttpResponseMessage response = await client.GetAsync("todos/3");
            // когда в адресной строке мы пишем адрес сервера - мы делаем запрос

            // вызываем эту ф-цию, чтобы убедиться, что он у нас вып-ся
            response.EnsureSuccessStatusCode();
            // этот метод вызовет искл-ие, если код 'response' не равен 200

            // получаем ответ в виде 'json' 
            string jsonResponse = await response.Content.ReadAsStringAsync();
            // отвте вернулся от сервера и он имеет содержимое 
            // 'content' относится к этому содержимому
            // 'ReadAsStringAsync' - чтобы получить ответ в виде строки

            // выводим ответ
            OutPutTB.Text = jsonResponse;
        }

        private async void Get_fromJson_but_Click(object sender, RoutedEventArgs e)
        {
            // получим список дел // можем его фильтровать
            // так как у нас 'todos' - возвращается массив
            // нужно раскрыть шаблонный метод в кол-цию
            // добавляем св-во фиьтрации
            var todos = await client.GetFromJsonAsync<List<Todo>>("todos?userId=1&completed=false");
            // получим json-файл - автоматически преобразует в список файлов 'Todo'

            OutPutTB.Text = "";

            if (todos != null)
            {
                foreach (var todo in todos)
                {
                    OutPutTB.Text += todo.ToString() + "\n";
                }
            }
        }

        private async void DoPOST_but_Click(object sender, RoutedEventArgs e)
        {
            // описывает http-content в виде строки
            StringContent jsonContent = new StringContent(
                JsonSerializer.Serialize(
                   new {
                       userId = 77,
                       id = 1,
                       title = "write code sample",
                       completed = false
                   }
                    ), Encoding.UTF8, "application/json" // тип запроса
                );

            // 'PostAsync()' - передает данные на сервер
            // нужно указать, на какую страницу мы его делаем
            HttpResponseMessage response = await client.PostAsync("todos", jsonContent);
            // у post-запроса должно быть тело обязат-но

            // вызываем эту ф-цию, чтобы убедиться, что он у нас вып-ся
            response.EnsureSuccessStatusCode();

            // выводим
            var jsonResponse = await response.Content.ReadAsStringAsync();

            OutPutTB.Text = jsonResponse;
        }

        private void DoPOSTasJson_but_Click(object sender, RoutedEventArgs e)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(
                "todos",
                new Todo(UserId: 9, IDataObject: 99, Title: InputTB.Text, DownloadDataCompletedEventArgs: false)
                );
            // автоматически происходит сериализация в json

            // вызываем эту ф-цию, чтобы убедиться, что он у нас вып-ся
            response.EnsureSuccessStatusCode();

            // автоматически десериализуем
            var todo = await response.Content.ReadFromJsonAsync<Todo>();

            OutPutTB.Text = $"Response: {todo}";
        }

        private void DoPUT_but_Click(object sender, RoutedEventArgs e)
        {
            // сериализуем класс, отправляем в 'json'
            StringContent jsonContent = new StringContent(
                JsonSerializer.Serializer( new {
                userId = 1, id = 1, title = "hello world", completed = false
            }),
            Encoding.UTF8, "application/lson"
                );

            HttpResponseMessage response = await client.PutAsync(
                "todos/1",
                jsonContent
                );

            // вызываем эту ф-цию, чтобы убедиться, что он у нас вып-ся
            response.EnsureSuccessStatusCode();

            // выводим
            var jsonResponse = await response.Content.ReadAsStringAsync();

            OutPutTB.Text = jsonResponse;
        }

        private void DoPUTasJson_but_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
