using DiplomZabpo43.classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для RegistrPage.xaml
    /// </summary>
    public partial class RegistrPage : Page
    {
        private MainWindow mainWindow;
        private Connection connection;

        public RegistrPage(MainWindow mainWindow)
        {
            InitializeComponent();
            connection = new Connection();
            this.mainWindow = mainWindow;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string firstName = txtFirstName.Text;
            string lastName = txtLastName.Text;
            string email = tbtDr.Text;
            string phone = tbtTel.Text;
            string password = tbtTel2.Text;
            DateTime birthday = dtpPick2.SelectedDate ?? DateTime.MinValue;

            // Check if email is valid
            if (!IsEmailValid(email))
            {
                MessageBox.Show("Неправильный формат email.");
                return;
            }

            // Check if password meets requirements
            if (!CheckPasswordRequirements(password))
            {
                MessageBox.Show("Пароль должен содержать от 6 до 12 символов, хотя бы одну заглавную букву и одну цифру.");
                return;
            }

            bool registrationResult = AddUserToDatabase(firstName, lastName, email, phone, password, birthday);

            if (registrationResult)
            {
                NavigationService navigationService = NavigationService.GetNavigationService(this);

                MessageBox.Show("Регистрация успешна!");
                if (navigationService.CanGoBack)
                {
                    navigationService.GoBack();
                }
                else
                {
                    MessageBox.Show("Невозможно вернуться на предыдущую страницу.");
                }
            }
            else
            {
                MessageBox.Show("Ошибка при регистрации. Пожалуйста, проверьте данные.");
            }
        }

        private bool AddUserToDatabase(string firstName, string lastName, string email, string phone, string password, DateTime birthday)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connection.ConnString))
                {
                    conn.Open();
                    string query = "INSERT INTO [User_table] (user_name, user_lastname, email, phone, [password], birthday) VALUES (@FirstName, @LastName, @Email, @Phone, @Password, @Birthday)";
                    SqlCommand command = new SqlCommand(query, conn);
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Phone", phone);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@Birthday", birthday);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при регистрации пользователя: " + ex.Message);
                return false;
            }
        }

        private bool CheckPasswordRequirements(string password)
        {
            if (password.Length < 6 || password.Length > 12)
            {
                return false;
            }

            if (!Regex.IsMatch(password, @"^(?=.*[A-Z])(?=.*\d).+$"))
            {
                return false;
            }

            return true;
        }

        private bool IsEmailValid(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}