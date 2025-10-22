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
    /// Логика взаимодействия для EditPage.xaml
    /// </summary>
    public partial class EditPage : Page
    {
        private Service _currentService = new Service();

        public EditPage(Service SelectedService)
        {
            InitializeComponent();

            if (SelectedService != null)
                _currentService = SelectedService;

            DataContext = _currentService;
            
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentService.Title))
                errors.AppendLine("Укажите название услуги");

            if (_currentService.Cost == 0)
                errors.AppendLine("Укажите стоимость услуги");

            if (_currentService.DiscountInt < 0 || _currentService.DiscountInt > 100)
                errors.AppendLine("Укажите скидку от 0 до 100");


            if (_currentService.DurationInSeconds == 0)
                errors.AppendLine("Укажите длительность услуги");

            if (_currentService.DurationInSeconds > 240 || _currentService.DurationInSeconds < 0)
                errors.AppendLine("Длительность не может быть больше 240 минут или меньше 0");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            if (_currentService.ID == 0)
            {
                var existingService = YusupovAutoserviceEntities.GetContext().Service
                    .FirstOrDefault(p => p.Title == _currentService.Title);

                if (existingService != null)
                {
                    MessageBox.Show("Уже существует такая услуга");
                    return;
                }

                YusupovAutoserviceEntities.GetContext().Service.Add(_currentService);
            }

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
    }
}
