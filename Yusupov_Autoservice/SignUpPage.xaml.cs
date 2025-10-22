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

namespace Yusupov_Autoservice
{
    /// <summary>
    /// Логика взаимодействия для SignUpPage.xaml
    /// </summary>
 

    public partial class SignUpPage : Page
    {

        private Service _currentService = new Service();

        public SignUpPage(Service SelectedService)
        {
            InitializeComponent();
            if (SelectedService != null)
                this._currentService = SelectedService;


            DataContext = _currentService;

            var _currentClient = YusupovAutoserviceEntities.GetContext().Client.ToList();

            ComboClient.ItemsSource = _currentClient;
        }

        private ClientService _currentClientService = new ClientService();

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (ComboClient.SelectedItem == null)
                errors.AppendLine("Укажите ФИО клиента");

            if (Startdate.Text == "")
                errors.AppendLine("Укажите дату услуги");

            if (TBStart.Text == "")
                errors.AppendLine("Укажите время начала услуги");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            //_currentClientService.ClientID = ComboClient.SelectedIndex + 1;
            Client selectedClient = ComboClient.SelectedItem as Client;
            _currentClientService.ClientID = selectedClient.ID;
            _currentClientService.ServiceID = _currentService.ID;
            _currentClientService.StartTime = Convert.ToDateTime(Startdate.Text + " " + TBStart.Text);

            if (_currentClientService.ID == 0)
                YusupovAutoserviceEntities.GetContext().ClientService.Add(_currentClientService);

            try
            {
                YusupovAutoserviceEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void TBStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            string s = TBStart.Text;

            string[] start = s.Split(':');

            if (start.Length != 2)
            {
                TBEnd.Text = "Неверный формат времени";
                return;
            }

            if (s.Length != 5 || s[2] != ':')
            {
                TBEnd.Text = "Неверный формат времени (должно быть HH:mm)";
                return;
            }

            if (!int.TryParse(start[0], out int startHour) || !int.TryParse(start[1], out int startMinute))
            {
                TBEnd.Text = "Неверный формат времени";
                return;
            }

            // Добавляем проверки на часы и минуты
            if (startHour < 0 || startHour > 23 || startMinute < 0 || startMinute > 59)
            { 
                TBEnd.Text = "Неверный формат времени (часы: 0-23, минуты: 0-59)";
                return;
            }

            int totalMinutes = startHour * 60 + startMinute + _currentService.DurationInSeconds;
            int EndHour = totalMinutes / 60;
            int EndMin = totalMinutes % 60;

            EndHour = EndHour % 24; // Это остается для обработки переполнения

            s = EndHour.ToString("D2") + ":" + EndMin.ToString("D2");
            TBEnd.Text = s;
            
            /*string s = TBStart.Text;

            if (s.Length < 3 || !s.Contains(':'))
                TBEnd.Text = "";
            else
            {
                string[] start = s.Split(new char[] { ":" });
                int startHour = Convert.ToInt32(start[0].ToString()) * 60;
                int startMin = Convert.ToInt32(start[1].ToString());

                int sum = startHour + startMin + _currentService.DurationInSeconds;

                int EndHour = sum / 60;
                int EndMin = sum % 60;
                s = EndHour.ToString() + ":" + EndMin.ToString();
                TBEnd.Text = s;
            }*/

        }
    }
}
