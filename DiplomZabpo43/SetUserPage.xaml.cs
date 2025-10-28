using DiplomZabpo43.classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
    /// Логика взаимодействия для SetUserPage.xaml
    /// </summary>
    public partial class SetUserPage : Page
    {
        public SetUserPage(MainWindow mainWindow, int user_Id)
        {
            InitializeComponent();
            LoadUsers();
            LoadRoles();
        }
        public int user_id;
        private void LoadUsers()
        {
            using (SqlConnection sqlConnection = new SqlConnection(new Connection().ConnString))
            {
                sqlConnection.Open();
                SqlCommand command = new SqlCommand("SELECT user_id, email, phone, [password], role_id, role_name FROM User_table JOIN Role_table ON user_role = role_id", sqlConnection);
                SqlDataReader reader = command.ExecuteReader();
                List<User> users = new List<User>();

                while (reader.Read())
                {
                    users.Add(new User(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetString(3),
                        reader.GetInt32(4),
                        reader.GetString(5)
                    ));
                }

                combouser.ItemsSource = users;
                combouser.DisplayMemberPath = "Email";
                combouser.SelectedValuePath = "UserId";
            }
        }

        private void LoadRoles()
        {
            using (SqlConnection sqlConnection = new SqlConnection(new Connection().ConnString))
            {
                sqlConnection.Open();
                SqlCommand command = new SqlCommand("SELECT role_id, role_name FROM Role_table", sqlConnection);
                SqlDataReader reader = command.ExecuteReader();
                List<User> roles = new List<User>();

                while (reader.Read())
                {
                    roles.Add(new User(
                        reader.GetInt32(0),
                        null, // Email
                        null, // Phone
                        null, // Password
                        reader.GetInt32(0), // RoleId
                        reader.GetString(1) // RoleName
                    ));
                }

                comborole.ItemsSource = roles;
                comborole.DisplayMemberPath = "RoleName";
                comborole.SelectedValuePath = "RoleId";
            }
        }
        private void combouser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (combouser.SelectedValue != null)
            {
                int userId = (int)combouser.SelectedValue;
                LoadUserData(userId);
            }
        }

        private void LoadUserData(int userId)
        {
            using (SqlConnection sqlConnection = new SqlConnection(new Connection().ConnString))
            {
                sqlConnection.Open();
                SqlCommand command = new SqlCommand("SELECT user_id, email, phone, [password], role_id FROM User_table JOIN Role_table ON user_role = role_id WHERE user_id = @userId", sqlConnection);
                command.Parameters.AddWithValue("@userId", userId);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    comborole.SelectedValue = reader.GetInt32(4);
                    tbEmail.Text = reader.GetString(1);
                    tbPhone.Text = reader.GetString(2);
                    tbPassword.Text = reader.GetString(3);
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (combouser.SelectedValue != null)
            {
                int userId = (int)combouser.SelectedValue;
                int userRole = (int)comborole.SelectedValue;
                string email = tbEmail.Text;
                string phone = tbPhone.Text;
                string password = tbPassword.Text;

                using (SqlConnection sqlConnection = new SqlConnection(new Connection().ConnString))
                {
                    sqlConnection.Open();
                    SqlCommand command = new SqlCommand("UPDATE User_table SET user_role = @userRole, email = @Email, phone = @Phone, [password] = @Password WHERE user_id = @userId", sqlConnection);
                    command.Parameters.AddWithValue("@userRole", userRole);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Phone", phone);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@userId", userId);

                    command.ExecuteNonQuery();
                }

                MessageBox.Show("Пользователь успешно изменен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        { 
                NavigationService.GoBack();         
        }

      
    }
}
