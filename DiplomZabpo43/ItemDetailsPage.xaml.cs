using DiplomZabpo43.classes;
using DiplomZabpo43.classes.DiplomZabpo43.classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
using System.Xml;
using System.Xml.Linq;

namespace DiplomZabpo43
{
    /// <summary>
    /// Логика взаимодействия для ItemDetailsPage.xaml
    /// </summary>
    public partial class ItemDetailsPage : Page
    {
        public int user_Id;

        private readonly Connection connectionInfo = new Connection();
        public int productId;

        public ItemDetailsPage(MainWindow mainWindow, int userId)
        {

            InitializeComponent();
            LoadData();
            LoadAptekaAddresses();
            itemsListView.Visibility = Visibility.Collapsed;
            Star1.Visibility = Visibility.Collapsed;
            Star2.Visibility = Visibility.Collapsed;
            Star3.Visibility = Visibility.Collapsed;
            Star4.Visibility = Visibility.Collapsed;
            Star5.Visibility = Visibility.Collapsed;


        }


        private void LoadData()
        {

            List<SelectItemList> itemList = new List<SelectItemList>();

            Connection connectionInfo = new Connection();
            string connectionString = connectionInfo.ConnString;

            string query = @"
        SELECT 
            [product_id], 
            [dbo].[Medikaments].[image_med], 
            [dbo].[Medikaments].[name_product], 
            [dbo].[Brand].[brand_name], 
            [dbo].[Medikaments].[form], 
            [dbo].[Manufacturer].[manufac_name], 
            [dbo].[Medikaments].[price], 
            [dbo].[Country].[name_country], 
            [dbo].[Dosa].[dosa_name], 
            [dbo].[Subsubcategories].[subsubcategory_name], 
            [dbo].[Subcategories].[subcategory_name], 
            [dbo].[Categories].[category_name],
            [dbo].[Medikaments].[storage_condition],
            [dbo].[Medikaments].[contraindications],
            [dbo].[Comment].[content_com],	
            [dbo].[Comment].[raiting]
        FROM 
            [dbo].[Medikaments] 
        LEFT JOIN 
            [dbo].[Brand] ON [dbo].[Medikaments].[brand] = [brand_id] 
        RIGHT JOIN 
            [dbo].[Manufacturer] ON [manefac_id] = [dbo].[Medikaments].[manufacturer] 
        RIGHT JOIN 
            [dbo].[Subsubcategories] ON [subsubcategory_id] = [dbo].[Medikaments].[subsubcategory] 
        JOIN 
            [dbo].[Subcategories] ON [dbo].[Subsubcategories].[subcategory_id] = [dbo].[Subcategories].[subcategory_id] 
        JOIN 
            [dbo].[Country] ON [country_id] = [dbo].[Medikaments].[country] 
        LEFT JOIN 
            [dbo].[Dosa] ON [dbo].[Medikaments].[dosa_id] = [dbo].[Dosa].[dosa_id] 
        JOIN 
            [dbo].[Categories] ON [dbo].[Categories].[category_id] = [dbo].[Subcategories].[category_id] 
        JOIN 
            [dbo].[Comment] ON [dbo].[Comment].[medikament] = [dbo].[Medikaments].[product_id]              
        ORDER BY 
            [dbo].[Medikaments].[name_product];";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {

                        SelectItemList item = new SelectItemList(
                            
                            productId: reader.GetInt32(0),
                            imageMed: reader.IsDBNull(1) ? null : (byte[])reader.GetValue(1),
                            nameProduct: reader.IsDBNull(2) ? null : reader.GetString(2),
                            brandName: reader.IsDBNull(3) ? null : reader.GetString(3),
                            form: reader.IsDBNull(4) ? null : reader.GetString(4),
                            manufacName: reader.IsDBNull(5) ? null : reader.GetString(5),
                            price: reader.GetInt32(6),
                            country_name: reader.IsDBNull(7) ? null : reader.GetString(7),
                            dosa_name: reader.IsDBNull(8) ? null : reader.GetString(8),
                            subsubcategory_name: reader.IsDBNull(9) ? null : reader.GetString(9),
                            subcategory_name: reader.IsDBNull(10) ? null : reader.GetString(10),
                            category_name: reader.IsDBNull(11) ? null : reader.GetString(11),
                            storageCondition: reader.IsDBNull(12) ? null : reader.GetString(12),
                            contraindications: reader.IsDBNull(13) ? null : reader.GetString(13),
                            commentContent: reader.IsDBNull(14) ? null : reader.GetString(14),
                            rating: reader.IsDBNull(15) ? 0 : reader.GetInt32(15)

                        );

                        itemList.Add(item);
                        //itemsListBox.ItemsSource = itemList;

                    }

                    reader.Close();
                    

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);

                }

            }
        }
        
        private void Star1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Star2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Star3_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Star4_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Star5_Click(object sender, RoutedEventArgs e)
        {

        }
        private void LoadAptekaAddresses()
        {
            AptekaComboBox.Items.Clear();
            Connection connectionInfo = new Connection();
            string connectionString = connectionInfo.ConnString;

            string query = "SELECT apteka_id, adres FROM Apteka JOIN Adres ON Apteka.adres_apteki_id = Adres.adres_id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int aptekaId = reader.GetInt32(0);
                        string address = reader.GetString(1);
                        AptekaComboBox.Items.Add(new AptekaAddress { AptekaId = aptekaId, Address = address });
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке адресов аптек: " + ex.Message);
                }
            }
        }

        private void AptekaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AptekaAddress selectedApteka = (AptekaAddress)AptekaComboBox.SelectedItem;
            if (selectedApteka != null)
            {
                CheckAvailability(selectedApteka.AptekaId, productId);
            }
        }
        private void CheckAvailability(int aptekaId, int productId)
        {
            string connectionString = connectionInfo.ConnString;

            string query = @"
            SELECT [count]
            FROM [Availability]
            WHERE apteka = @aptekaId AND medikaments = @productId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@aptekaId", aptekaId);
                command.Parameters.AddWithValue("@productId", productId);

                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        int count = Convert.ToInt32(result);
                        NalLabel.Content = $"Наличие: {count}";
                    }
                    else
                    {
                        NalLabel.Content = "Наличие: нет данных";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при проверке наличия медикамента: " + ex.Message);
                }
            }
        }
        private void CheackStar(int productId)
        {
            string connectionString = connectionInfo.ConnString;

            string query = @"
        SELECT AVG([raiting]) AS averageRating
        FROM [dbo].[Comment]
        WHERE medikament = @productId;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@productId", productId);

                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();

                    if (result != DBNull.Value && result != null)
                    {
                        double averageRating = Convert.ToDouble(result);

                        // Установка цвета звездочек в зависимости от среднего значения рейтинга
                        if (averageRating >= 1)
                            Star1.Foreground = Brushes.Yellow;
                        if (averageRating >= 2)
                            Star2.Foreground = Brushes.Yellow;
                        if (averageRating >= 3)
                            Star3.Foreground = Brushes.Yellow;
                        if (averageRating >= 4)
                            Star4.Foreground = Brushes.Yellow;
                        if (averageRating >= 5)
                            Star5.Foreground = Brushes.Yellow;
                    }
                    else
                    {
                        // Если результат равен null, сбросить цвет звездочек
                        Star1.Foreground = Brushes.Black;
                        Star2.Foreground = Brushes.Black;
                        Star3.Foreground = Brushes.Black;
                        Star4.Foreground = Brushes.Black;
                        Star5.Foreground = Brushes.Black;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при проверке комментария: " + ex.Message);
                }
            }
        }
      

       

        private void CheckComment(int productId)
        {
            CheackStar(productId);
            string connectionString = connectionInfo.ConnString;

            string query = @"
        SELECT c.content_com, c.raiting, u.user_name 
        FROM [dbo].[Comment] c
        INNER JOIN [dbo].[User_table] u ON c.usercom_id = u.user_id
        WHERE c.medikament = @productId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@productId", productId);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        string commentContent = reader.GetString(0);
                        int rating = reader.GetInt32(1);
                        string userEmail = reader.GetString(2);

                        CommentLabel.Content = $"Комментарий: {commentContent}";
                        RatingLabel.Content = $"Рейтинг: {rating}";
                        UserLabel.Content = $"Пользователь: {userEmail}";
                    }
                    else
                    {
                        CommentLabel.Content = "Комментарий: нет данных";
                        RatingLabel.Content = "Рейтинг: нет данных";
                        UserLabel.Content = "Пользователь: нет данных";
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при проверке комментария: " + ex.Message);
                }
            }
        }
        private void AddComm_Click(object sender, RoutedEventArgs e)
        {
            string commentContent = CommentTextBox.Text;
            int rating = GetSelectedStarRating();

            if (user_Id != 0)
            {
                string query = @"
            INSERT INTO [dbo].[Comment] (content_com, raiting, medikament, usercom_id)
            VALUES (@content_com, @raiting, @medikament, @usercom_id)";

                using (SqlConnection connection = new SqlConnection(connectionInfo.ConnString))
                {
                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@content_com", commentContent);
                    command.Parameters.AddWithValue("@raiting", rating);
                    command.Parameters.AddWithValue("@medikament", productId);
                    command.Parameters.AddWithValue("@usercom_id", user_Id);

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Комментарий успешно добавлен!");
                            CheckComment(productId);
                        }
                        else
                        {
                            MessageBox.Show("Не удалось добавить комментарий.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при добавлении комментария: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Вы должны войти в систему, чтобы добавить комментарий.");
            }
        }

        private int GetSelectedStarRating()
        {
            if (Star15.IsChecked == true) return 5;
            if (Star14.IsChecked == true) return 4;
            if (Star13.IsChecked == true) return 3;
            if (Star12.IsChecked == true) return 2;
            if (Star11.IsChecked == true) return 1;
            return 0;
        }

        private void Star_Checked(object sender, RoutedEventArgs e)
        {
            UpdateStars();
        }

        private void Star_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateStars();
        }


        private void UpdateStars()
        {
            int rating = GetSelectedStarRating();

            Star11.IsChecked = rating >= 1;
            Star12.IsChecked = rating >= 2;
            Star13.IsChecked = rating >= 3;
            Star14.IsChecked = rating >= 4;
            Star15.IsChecked = rating >= 5;

            // Обновляем цвета звезд
       
        }

      

        private void Zakaz_Click(object sender, RoutedEventArgs e)
        {
            AptekaAddress selectedApteka = (AptekaAddress)AptekaComboBox.SelectedItem;

            if (selectedApteka != null)
            {
                // Проверяем наличие медикамента в выбранной аптеке
                if (CheckAvailabilityForOrder(selectedApteka.AptekaId, productId))
                {
                    SelectItemList selectedItem = new SelectItemList(productId, null, null, null, null, null, 0, null, null, null, null, null, null, null, null, 0);

                    if (selectedItem != null)
                    {
                        int orderId = CreateOrder(selectedItem, selectedApteka);
                        if (orderId != -1)
                        {
                            AddItemToOrder(orderId, selectedItem.ProductUd);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Выберите товар для заказа!");
                    }
                }
                else
                {
                    MessageBox.Show("Товар отсутствует в выбранной аптеке!");
                }
            }
            else
            {
                MessageBox.Show("Выберите аптеку для заказа!");
            }
        }
        private void ShowComm_Click(object sender, RoutedEventArgs e)
        {
            CheckComment(productId);

            if (itemsListView.Visibility == Visibility.Visible
)
            {
                Star1.Visibility = Visibility.Collapsed;
                Star2.Visibility = Visibility.Collapsed;
                Star3.Visibility = Visibility.Collapsed;
                Star4.Visibility = Visibility.Collapsed;
                Star5.Visibility = Visibility.Collapsed;

                itemsListView.Visibility = Visibility.Collapsed;
            }
            else
            {
                Star1.Visibility = Visibility.Visible;
                Star2.Visibility = Visibility.Visible;
                Star3.Visibility = Visibility.Visible;
                Star4.Visibility = Visibility.Visible;
                Star5.Visibility = Visibility.Visible;
                itemsListView.Visibility = Visibility.Visible;
            }

        }
        private bool CheckAvailabilityForOrder(int aptekaId, int productId)
        {
            string connectionString = connectionInfo.ConnString;

            string query = @"
            SELECT [count]
            FROM [Availability]
            WHERE apteka = @aptekaId AND medikaments = @productId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@aptekaId", aptekaId);
                command.Parameters.AddWithValue("@productId", productId);

                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        int count = Convert.ToInt32(result);
                        return count > 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при проверке наличия медикамента для заказа: " + ex.Message);
                }
            }

            return false;
        }

        private int CreateOrder(SelectItemList selectedItem, AptekaAddress selectedApteka)
        {
            string connectionString = connectionInfo.ConnString;

            string checkUserQuery = @"
            SELECT COUNT(1)
            FROM User_table
            WHERE user_id = @user_id";

            string insertOrderQuery = @"
            INSERT INTO Orders (user_id, apteka_id, order_total)
            VALUES (@user_id, @apteka_id, @order_total);
            SELECT SCOPE_IDENTITY();";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand checkUserCommand = new SqlCommand(checkUserQuery, connection))
                    {
                        checkUserCommand.Parameters.AddWithValue("@user_id", user_Id);
                        int userExists = (int)checkUserCommand.ExecuteScalar();
                        if (userExists == 0)
                        {
                            MessageBox.Show("Вначале вам требуется авторизоваться!");
                            return -1;
                        }
                    }

                    using (SqlCommand insertOrderCommand = new SqlCommand(insertOrderQuery, connection))
                    {
                        insertOrderCommand.Parameters.AddWithValue("@user_id", user_Id);
                        insertOrderCommand.Parameters.AddWithValue("@apteka_id", selectedApteka.AptekaId);
                        insertOrderCommand.Parameters.AddWithValue("@order_total", 0);
                        return Convert.ToInt32(insertOrderCommand.ExecuteScalar());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при создании заказа: " + ex.Message);
                    return -1;
                }
            }
        }


        private void AddItemToOrder(int orderId, int productId)
        {
            string connectionString = connectionInfo.ConnString;

            string query = @"
            INSERT INTO Order_List (order_id, product_id, count_ord)
            VALUES (@order_id, @product_id, @count_ord);";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@order_id", orderId);
                command.Parameters.AddWithValue("@product_id", productId);
                command.Parameters.AddWithValue("@count_ord", 1); // count_ord всегда 1

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Товар добавлен в заказ!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении товара в заказ: " + ex.Message);
                }
            }
        }
        private void btnBackAut_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void itemsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
