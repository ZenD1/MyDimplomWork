using DiplomZabpo43.classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
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
    /// Логика взаимодействия для PrifilePage.xaml
    /// </summary>
    public partial class PrifilePage : Page
    {

        public int role_id;
        public string email;
        public string phone;
        public string firstname;
        public string lastname;
        public string birthday;
        MainWindow mainWindow = new MainWindow();

        public string username;
        public PrifilePage(MainWindow mainWindow, int user_Id)
        {
            InitializeComponent();
            HideElements();
            LoadDataUser(user_Id);
            UserEmail.Text = email;
            UserPhoneNumber.Text = phone;
            UserFirstName.Text = firstname;
            UserLastName.Text = lastname;
            UserBirthday.Text = birthday;

            // Загрузка изображения из базы данных
            byte[] imageData = LoadImageFromDatabase(user_Id);
            if (imageData != null)
            {
                BitmapImage bitmapImage = ByteArrayToBitmapImage(imageData);
                imgProfile.Source = bitmapImage;
            }

            // Загрузка комментариев из базы данных
            List<string> comments = LoadCommentsFromDatabase(user_Id);
            foreach (string comment in comments)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = comment;
                listBoxComments.Items.Add(item);
            }
        }

        public int user_Id;


        private void Btn_back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();

        }

        private List<string> LoadCommentsFromDatabase(int userId)
        {
            List<string> comments = new List<string>();

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(new Connection().ConnString))
                {
                    sqlConnection.Open();

                    string query = "SELECT [name_product], [content_com], [raiting] " +
                                   "FROM [dbo].[Comment] " +
                                   "JOIN [dbo].[Medikaments] ON [medikament] = [product_id] " +
                                   "WHERE [usercom_id] = @userId";

                    using (SqlCommand command = new SqlCommand(query, sqlConnection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string comment = $"Продукт: {reader["name_product"]}, Комментарий: {reader["content_com"]}, Рейтинг: {reader["raiting"]}";
                                comments.Add(comment);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке комментариев из базы данных: " + ex.Message);
            }

            return comments;
        }

        private void InsertImageIntoDatabase(byte[] imageBytes)
{
    try
    {
                Connection connection = new Connection();

                using (SqlConnection sqlConnection = new SqlConnection(connection.ConnString))
                {
                    sqlConnection.Open();

                    // Создаем SQL-запрос для вставки данных в таблицу
                    string query = "UPDATE [dbo].[User_table] SET [image_user] = @imageBytes WHERE [user_id] = @userId";

            // Создаем команду SQL
            using (SqlCommand command = new SqlCommand(query, sqlConnection))
            {
                // Передаем параметры в команду
                command.Parameters.AddWithValue("@imageBytes", imageBytes);
                command.Parameters.AddWithValue("@userId", user_Id); // Замените user_id на ваше реальное значение

                // Выполняем команду SQL
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Изображение успешно загружено и сохранено в базе данных.");
                }
                else
                {
                    MessageBox.Show("Ошибка при сохранении изображения в базе данных.");
                }
            }
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show("Ошибка при загрузке изображения в базу данных: " + ex.Message);
    }
}
        private void Btn_selectImage(object sender, RoutedEventArgs e)
        {
            // Создаем диалоговое окно выбора файла
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg; *.jpeg; *.png; *.bmp|All files (*.*)|*.*";

            // Открываем диалоговое окно выбора файла
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Загружаем выбранное изображение в элемент Image
                    BitmapImage bitmapImage = new BitmapImage(new Uri(openFileDialog.FileName));
                    imgProfile.Source = bitmapImage;

                    // Преобразуем изображение в массив байтов
                    byte[] imageBytes = File.ReadAllBytes(openFileDialog.FileName);

                    // Выполняем вставку массива байтов изображения в базу данных
                    InsertImageIntoDatabase(imageBytes);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке изображения: " + ex.Message);
                }
            }
        }

        private void ChangedataElement_Click(object sender, RoutedEventArgs e)
        {
            ToggleDataElementsVisibility();

        }
        private void HideElements()
        {
           
                Second_bor.Visibility = Visibility.Collapsed;
                SaveChanged.Visibility = Visibility.Collapsed;



        }
        private void ToggleDataElementsVisibility()
        {

                Second_bor.Visibility = Visibility.Visible;
                SaveChanged.Visibility = Visibility.Visible;
                ChangedataElement.Visibility = Visibility.Collapsed;


        }

        private void SavedataElement_Click(object sender, RoutedEventArgs e)
        {
            HomePageUser homePageUser = new HomePageUser(mainWindow, user_Id, role_id);

            ChangedataElement.Visibility = Visibility.Visible;
            Second_bor.Visibility = Visibility.Collapsed;
            SaveData();
            mainWindow.MainFrame.Navigate(homePageUser);
            NavigationService.GoBack();
        }

        private BitmapImage ByteArrayToBitmapImage(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0) return null;

            BitmapImage bitmapImage = new BitmapImage();
            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(byteArray))
            {
                memoryStream.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }



        private void SaveData()
        {
            try
            {
                // Создаем объект подключения к базе данных
                using (SqlConnection sqlConnection = new SqlConnection(new Connection().ConnString))
                {
                    // Открываем соединение с базой данных
                    sqlConnection.Open();

                    // Создаем SQL-запрос для вставки данных в таблицу
                    StringBuilder queryBuilder = new StringBuilder("UPDATE [dbo].[User_table] SET ");
                    List<string> parameters = new List<string>();

                    if (!string.IsNullOrEmpty(txtFirstName.Text))
                    {
                        queryBuilder.Append("[user_name] = @firstName, ");
                        parameters.Add("@firstName");
                    }

                    if (!string.IsNullOrEmpty(txtLastName.Text))
                    {
                        queryBuilder.Append("[user_lastname] = @lastName, ");
                        parameters.Add("@lastName");
                    }

                    if (dtpPick2.SelectedDate != null)
                    {
                        queryBuilder.Append("[birthday] = @birthday, ");
                        parameters.Add("@birthday");
                    }

                    // Убираем лишнюю запятую и пробел в конце запроса
                    queryBuilder.Remove(queryBuilder.Length - 2, 2);

                    queryBuilder.Append(" WHERE [user_id] = @userId");

                    // Создаем команду SQL
                    using (SqlCommand command = new SqlCommand(queryBuilder.ToString(), sqlConnection))
                    {
                        // Передаем параметры в команду
                        if (parameters.Contains("@firstName"))
                            command.Parameters.AddWithValue("@firstName", txtFirstName.Text);
                        if (parameters.Contains("@lastName"))
                            command.Parameters.AddWithValue("@lastName", txtLastName.Text);
                        if (parameters.Contains("@birthday"))
                            command.Parameters.AddWithValue("@birthday", dtpPick2.SelectedDate);

                        command.Parameters.AddWithValue("@userId", user_Id); // Замените YourUserIdValueHere на реальное значение ID пользователя

                        // Выполняем команду SQL
                        int rowsAffected = command.ExecuteNonQuery();

                        // Проверяем, были ли изменения сохранены успешно
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Данные успешно сохранены!");
                        }
                        else
                        {
                            MessageBox.Show("Ошибка при сохранении данных.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении данных: " + ex.Message);
            }
        }
        private byte[] LoadImageFromDatabase(int userId)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(new Connection().ConnString))
                {
                    sqlConnection.Open();

                    string query = "SELECT [image_user] FROM [dbo].[User_table] WHERE [user_id] = @userId";

                    using (SqlCommand command = new SqlCommand(query, sqlConnection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);

                        // Выполняем запрос и получаем изображение в виде массива байтов
                        object result = command.ExecuteScalar();

                        if (result != DBNull.Value)
                        {
                            return (byte[])result;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке изображения из базы данных: " + ex.Message);
                return null;
            }
        }

        public void LoadDataUser(int user_Id)
        {
            using (SqlConnection connection = new SqlConnection(new Connection().ConnString))
            {
                SqlCommand command = new SqlCommand("SELECT [email], [phone], [user_name], [user_lastname], [birthday] FROM [dbo].[User_table] WHERE user_id = @user_id", connection);
                command.Parameters.AddWithValue("@user_id", user_Id);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        // Получаем данные пользователя из базы данных
                        email = reader.GetString(reader.GetOrdinal("email"));
                        phone = reader.GetString(reader.GetOrdinal("phone"));
                        firstname = reader.GetString(reader.GetOrdinal("user_name"));
                        lastname = reader.GetString(reader.GetOrdinal("user_lastname"));
                        birthday = reader.GetDateTime(reader.GetOrdinal("birthday")).ToString("dd.MM.yyyy");
                    }
                    else
                    {
                        // Если не удалось найти пользователя с указанным user_id
                        MessageBox.Show("Пользователь не найден.");
                    }
                }
                catch (Exception ex)
                {
                    // Обработка ошибок, если не удалось выполнить запрос
                    MessageBox.Show($"Ошибка при загрузке данных пользователя: {ex.Message}");
                }
            }
        }

        private void listBoxComments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
