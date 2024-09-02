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
using System.Windows.Shapes;

namespace Project_client_WPF
{
    /// <summary>
    /// Логика взаимодействия для Create.xaml
    /// </summary>
    public partial class Create : Window
    {
        public Create()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (txt_ip_1.Text.Length > 0 && txt_ip_2.Text.Length > 0 && txt_ip_3.Text.Length > 0 && txt_ip_4.Text.Length > 0 && txt_port.Text.Length > 0)
            {
                My_ip = txt_ip_1.Text;
                My_ip += ".";
                My_ip += txt_ip_2.Text;
                My_ip += ".";
                My_ip += txt_ip_3.Text;
                My_ip += ".";
                My_ip += txt_ip_4.Text;
                
                My_port = Convert.ToInt32(txt_port.Text);
            }
            else
                MessageBox.Show("Проверьте правильность написания данных и поробуйте снова!");

            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private string my_ip;

        public string My_ip
        {
            get { return my_ip; }
            set { my_ip = value; }
        }

        private int my_port;

        public int My_port
        {
            get { return my_port; }
            set { my_port = value; }
        }
    }
}
