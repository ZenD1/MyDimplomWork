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
    public partial class LoginPage : Page
    {
        private MainWindow mainWindow;

        public string enteredEmail;
        public int user_Id;
        public int role_Id; // Добавляем role_Id

        public SpisokPage SpisokPageInstance { get; private set; }
        public event EventHandler<string> EmailEntered;
        public event EventHandler<string> SuccessfulLogin;
        private AdminPage adminPage;
        private SetMedikamentPage setMedikamentPage;
        private SetUserPage setUserPage;
        private SecretPage secretPage;
        private SpisokPage spisokPage;
        private HomePageUser homepage;
        private BuscketPage buscketPage; // Добавьте поле для хранения экземпляра BuscketPage
        private ItemDetailsPage itemDetailsPage;
        public LoginPage(MainWindow mainWindow, int user_Id, int role_Id)
        {
            InitializeComponent();
            DataContext = this;
            this.mainWindow = mainWindow;
            this.role_Id = role_Id; // Сохраняем role_Id
            this.user_Id = user_Id; // Сохраняем user_Id
            Loaded += LoginPage_Loaded;

            // Создаем экземпляр SpisokPage
            SpisokPageInstance = new SpisokPage(mainWindow, user_Id);
        }

        protected virtual void OnSuccessfulLogin(string email)
        {
            SuccessfulLogin?.Invoke(this, email);
        }

        public string EnteredEmail
        {
            get { return enteredEmail; }
            private set { enteredEmail = value; }
        }

        private void Login()
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль");
                return;
            }

            string query;
            if (IsEmail(username))
            {
                query = "SELECT user_id, user_role, email FROM User_table WHERE email = @username AND [password] = @password"; // Добавляем user_role в запрос
            }
            else
            {
                query = "SELECT user_id, user_role, email FROM User_table WHERE phone = @username AND [password] = @password"; // Добавляем user_role в запрос
            }

            try
            {
                Connection connection = new Connection();

                using (SqlConnection sqlConnection = new SqlConnection(connection.ConnString))
                {
                    sqlConnection.Open();

                    using (SqlCommand command = new SqlCommand(query, sqlConnection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password);

                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            user_Id = reader.GetInt32(reader.GetOrdinal("user_id"));
                            role_Id = reader.GetInt32(reader.GetOrdinal("user_role")); // Получаем role_Id из базы данных
                            string email = reader.GetString(reader.GetOrdinal("email"));
                            EnteredEmail = email;
                            mainWindow.SetIsLoggedIn(true);
                            MessageBox.Show("Авторизация успешна!");
                            OnSuccessfulLogin(email);                           
                            EmailEntered?.Invoke(this, email);
                            mainWindow.CheackBuscket();
                            mainWindow.CheackLogingUser();
                            mainWindow._isLoggedIn = true; //
                            mainWindow.colorMenusUser();

                            NavigationService.GoBack();

                        }
                        else
                        {
                            MessageBox.Show("Неверный логин или пароль");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при авторизации: {ex.Message}");
            }
        }


        private bool IsEmail(string input)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(input);
                return addr.Address == input;
            }
            catch
            {
                return false;
            }
        }

        private void LoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            setMedikamentPage = new SetMedikamentPage(mainWindow, user_Id);
            setUserPage = new SetUserPage(mainWindow, user_Id);
            secretPage = new SecretPage(mainWindow, user_Id);
            buscketPage = new BuscketPage(mainWindow, user_Id, role_Id);
            spisokPage = new SpisokPage(mainWindow, user_Id);
            itemDetailsPage = new ItemDetailsPage(mainWindow, user_Id);
            AdminPage adminPage = new AdminPage(mainWindow, user_Id, role_Id);
            HomePageUser homepage = new HomePageUser(mainWindow, user_Id, role_Id); // Передаем role_Id
            txtUsername.Text = "";
            txtPassword.Password = "";
            homepage = new HomePageUser(mainWindow, user_Id, role_Id);
        }
       

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            setMedikamentPage = new SetMedikamentPage(mainWindow, user_Id);
            setUserPage = new SetUserPage(mainWindow, user_Id);
            adminPage = new AdminPage(mainWindow, user_Id, role_Id);
            homepage = new HomePageUser(mainWindow, user_Id, role_Id);
            SpisokPageInstance = new SpisokPage(mainWindow, user_Id);
            
            Login();
            setUserPage.user_id = user_Id;
            setMedikamentPage.user_id = user_Id;
            secretPage.user_Id = user_Id;
            mainWindow.role_id = role_Id;
            mainWindow._enteredEmail = enteredEmail;
            mainWindow.user_Id = user_Id;
            homepage.useremail = enteredEmail;
            homepage.user_Id = user_Id;
            homepage.role_id = role_Id;
            adminPage.user_Id =user_Id;
            adminPage.role_id = role_Id;
            spisokPage.user_id = user_Id;
            itemDetailsPage.user_Id = user_Id;
            mainWindow.HomePageInstance = homepage;
            buscketPage.role_id = role_Id;
            mainWindow.busyPage = buscketPage; 

            // Отображаем user_Id в MessageBox для проверки
        }

        private void btnBackAut_Click(object sender, RoutedEventArgs e)
        {

            spisokPage.user_id = user_Id;
            NavigationService.GoBack();
        }
    }
}