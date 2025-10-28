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
    /// Логика взаимодействия для SecretPage.xaml
    /// </summary>
    public partial class SecretPage : Page
    {
        public int user_Id;
        private readonly Connection connectionInfo = new Connection();

        public SecretPage(MainWindow mainWindow, int user_Id)
        {
            InitializeComponent();
        }
        private string GetPasswordFromDatabase(int userId)
        {
            string password = string.Empty;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionInfo.ConnString))
                {
                    connection.Open();
                    string query = @"
                        SELECT [password] 
                        FROM User_table 
                        WHERE user_id = @userId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@userId", userId);

                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        password = result.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Пользователь с указанным идентификатором не найден.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении пароля из базы данных: {ex.Message}");
            }

            return password;
        }

        // Метод для изменения пароля в базе данных
        private void ChangePasswordInDatabase(int userId, string newPassword)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionInfo.ConnString))
                {
                    connection.Open();
                    string query = @"
                        UPDATE User_table 
                        SET [password] = @newPassword
                        WHERE user_id = @userId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@newPassword", newPassword);
                    command.Parameters.AddWithValue("@userId", userId);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Пароль успешно изменен.");
                        NavigationService.GoBack();


                    }
                    else
                    {
                        MessageBox.Show("Не удалось изменить пароль. Пользователь с указанным идентификатором не найден.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при изменении пароля в базе данных: {ex.Message}");
            }
        }
        private void ShowPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            int userId = user_Id; // Получите идентификатор пользователя, для которого нужно получить пароль
            string password = GetPasswordFromDatabase(userId);
            MessageBox.Show("Ваш текущий пароль: " + password);
        }
        private void btnBackAut_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            int userId = user_Id; // Получите идентификатор пользователя, для которого нужно изменить пароль
            string newPassword = NewPasswordTextBox.Text;
            ChangePasswordInDatabase(userId, newPassword);
        }
    }
}
    