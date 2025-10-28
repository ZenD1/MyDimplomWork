using DiplomZabpo43.classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace DiplomZabpo43
{
    /// <summary>
    /// Логика взаимодействия для HomePageUser.xaml
    /// </summary>
    public partial class HomePageUser : Page
    {

        public string useremail;
        public int user_Id;
        public string username;
        public int role_id;
        MainWindow mainWindow = new MainWindow();
        
        public HomePageUser(MainWindow mainWindow, int user_Id, int role_id)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.user_Id = user_Id; // Присваиваем user_Id
            DataContext = this; // Установите DataContext для возможности привязки данных
            UserIDText.Text = user_Id.ToString();
            Get_nameuser();
            EmailText.Text = username;
            if (role_id <= 2) // Если пользователь - администратор
            {
                ShowAdmin();
            }
            else // Если пользователь не является администратором
            {
                HideAdmin();
            }

        }
        public void HideAdmin()
        {
            Admin_Button.Visibility = Visibility.Collapsed;
        }

        public void ShowAdmin()
        {
            Admin_Button.Visibility=Visibility.Visible;
        }
        private void Profile_Click(object sender, RoutedEventArgs e)
        {

            // Создаем экземпляр ProfilePage
            PrifilePage prifilePage = new PrifilePage(mainWindow, user_Id);
            prifilePage.username = username;
            prifilePage.user_Id = user_Id;
            // Открываем ProfilePage
            mainWindow.MainFrame.Navigate(prifilePage);               
        }
        private void PrivacySettings_Click(object sender, RoutedEventArgs e)
        {
            SecretPage secretPage = new SecretPage(mainWindow, user_Id);
            secretPage.user_Id= user_Id;
            mainWindow.MainFrame.Navigate(secretPage);

        }
        private void MyPurchases_Click(object sender, RoutedEventArgs e)
        {
            AdminPage adminPage = new AdminPage(mainWindow, user_Id, role_id);
            adminPage.role_id = role_id;
            adminPage.user_Id = user_Id;
            mainWindow.MainFrame.Navigate(adminPage);

        }
        public void Get_nameuser()
        {
            using (SqlConnection connection = new SqlConnection(new Connection().ConnString))
            {
                SqlCommand command = new SqlCommand("SELECT user_name, user_lastname FROM User_table WHERE user_id = @user_id", connection);
                command.Parameters.AddWithValue("@user_id", user_Id);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        // Получаем имя и фамилию пользователя
                        string userFirstName = reader.GetString(reader.GetOrdinal("user_name"));
                        string userLastName = reader.GetString(reader.GetOrdinal("user_lastname"));
                        // Конкатенируем имя и фамилию
                        username = $"{userFirstName} {userLastName}";
                    }
                    else
                    {
                        // Если не удалось найти пользователя с указанным user_id
                        username = "Неизвестный пользователь";
                    }
                }
                catch (Exception ex)
                {
                    // Обработка ошибок, если не удалось выполнить запрос
                    MessageBox.Show($"Ошибка при получении имени пользователя: {ex.Message}");
                    username = "Ошибка получения имени";
                }
            }
        }
        public void Get_roleser()
        {

        }

        private void btnBackAut_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
