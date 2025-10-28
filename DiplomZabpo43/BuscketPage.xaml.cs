using DiplomZabpo43.classes;
using DiplomZabpo43.classes.DiplomZabpo43.classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Логика взаимодействия для BuscketPage.xaml
    /// </summary>
    public partial class BuscketPage : Page, INotifyPropertyChanged
    {
        private ObservableCollection<BascMedList> bascmedlist;

        private readonly Connection connectionInfo = new Connection();
        public int role_id;
        public event PropertyChangedEventHandler PropertyChanged;

        private int countMed;
        public int Count_Med
        {
            get { return countMed; }
            set
            {
                if (countMed != value)
                {
                    countMed = value;
                    OnPropertyChanged(nameof(Count_Med));
                }
            }
        }

        private int totalPrice;
        public int TotalPrice
        {
            get { return totalPrice; }
            set
            {
                if (totalPrice != value)
                {
                    totalPrice = value;
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }
        private int fullPrice;
        public int FullPrice
        {
            get { return fullPrice; }
            set
            {
                if (fullPrice != value)
                {
                    fullPrice = value;
                    OnPropertyChanged(nameof(FullPrice));
                }
            }
        }



        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public BuscketPage(MainWindow mainWindow, int user_Id, int role_Id)
        {
            InitializeComponent();
            this.DataContext = this; // Установка DataContext
            bascmedlist = new ObservableCollection<BascMedList>();
            charactersListBox.ItemsSource = bascmedlist; // Привязка к коллекции данных
            DataContext = this;
            Count_Med = bascmedlist.Count;

            this.user_Id = user_Id; // Установка user_Id
            this.role_id = role_Id; // Установка role_Id
            LoadMedikamentFromDB();
        }
        public int user_Id;


        private void GetAdd_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected item from the ListBox
            BascMedList selectedMed = (sender as Button).DataContext as BascMedList;

            // Update the quantity in the database by adding 1
            UpdateQuantity(selectedMed, selectedMed.Count_Med + 1);

            // Update the ListBox to reflect the changes
            charactersListBox.Items.Refresh();
        }

        private void GetDel_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected item from the ListBox
            BascMedList selectedMed = (sender as Button).DataContext as BascMedList;

            // Update the quantity in the database by subtracting 1
            UpdateQuantity(selectedMed, selectedMed.Count_Med - 1);

            // Update the ListBox to reflect the changes
            charactersListBox.Items.Refresh();
        }
        private void UpdateFullPrice()
        {
            FullPrice = bascmedlist.Sum(item => item.TotalPrice);
        }

        private void UpdateQuantity(BascMedList med, int newQuantity)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionInfo.ConnString))
                {
                    connection.Open();
                    string query = @"
    UPDATE Order_List
    SET count_ord = @newQuantity
    FROM Order_List ol
    JOIN Orders o ON ol.order_id = o.order_id
    WHERE ol.product_id = @productId AND o.user_id = @userId;";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@newQuantity", newQuantity);
                    command.Parameters.AddWithValue("@productId", med.ProductUd);
                    command.Parameters.AddWithValue("@userId", user_Id);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        if (newQuantity <= 0)
                        {
                            // Если количество стало равным или меньше 0, удаляем медикамент из базы данных и из коллекции
                            RemoveMedFromDBAndList(med);
                        }
                        else
                        {
                            // Иначе обновляем количество в UI
                            med.Count_Med = newQuantity;
                            // Обновляем общую цену
                            med.TotalPrice = med.Price * med.Count_Med;
                            // Обновляем общую стоимость за все товары
                            UpdateFullPrice();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to update quantity in the database.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating quantity: {ex.Message}");
            }
        }


        private void RemoveMedFromDBAndList(BascMedList med)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionInfo.ConnString))
                {
                    connection.Open();
                    string query = @"
                DELETE FROM Order_List
                FROM Order_List ol
                JOIN Orders o ON ol.order_id = o.order_id
                WHERE ol.product_id = @productId AND o.user_id = @userId;";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@productId", med.ProductUd);
                    command.Parameters.AddWithValue("@userId", user_Id);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        // Удаляем медикамент из коллекции
                        bascmedlist.Remove(med);
                    }
                    else
                    {
                        MessageBox.Show("Failed to remove medicine from the database.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing medicine: {ex.Message}");
            }
        }
             
        private void LoadMedikamentFromDB()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionInfo.ConnString))
                {
                    connection.Open();
                    string query = @"
                SELECT 
                    m.product_id,
                    m.image_med,
                    m.name_product,
                    b.brand_name,
                    m.[form],
                    man.manufac_name,
                    m.price,
                    c.name_country,
                    d.dosa_name,
                    ss.subsubcategory_name,
                    sc.subcategory_name,
                    cat.category_name,
                    ol.count_ord
                FROM 
                    Orders o
                JOIN 
                    Order_List ol ON o.order_id = ol.order_id
                JOIN 
                    Medikaments m ON ol.product_id = m.product_id
                JOIN 
                    Brand b ON m.brand = b.brand_id
                JOIN 
                    Manufacturer man ON m.manufacturer = man.manefac_id
                JOIN 
                    Country c ON m.country = c.country_id
                JOIN 
                    Dosa d ON m.dosa_id = d.dosa_id
                JOIN 
                    Subsubcategories ss ON m.subsubcategory = ss.subsubcategory_id
                JOIN 
                    Subcategories sc ON ss.subcategory_id = sc.subcategory_id
                JOIN 
                    Categories cat ON sc.category_id = cat.category_id
                WHERE 
                    o.user_id = @user_id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@user_id", user_Id);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var productId = reader.GetInt32(0);
                            var imageMedBytes = reader.IsDBNull(1) ? null : (byte[])reader.GetValue(1);
                            var nameProduct = reader.GetString(2);
                            var brandName = reader.GetString(3);
                            var form = reader.GetString(4);
                            var manufacName = reader.GetString(5);
                            var price = reader.GetInt32(6);
                            var countryName = reader.GetString(7);
                            var dosa_name = reader.GetString(8);
                            var subsubcategory_name = reader.GetString(9);
                            var subcategory_name = reader.GetString(10);
                            var category_name = reader.GetString(11);
                            var count_Med = reader.GetInt32(12); // Установка значения Count_ord

                            var bascmed = new BascMedList(productId, imageMedBytes, nameProduct, brandName, form, manufacName, price, countryName, dosa_name, subsubcategory_name, subcategory_name, category_name, count_Med);

                            // Расчет общей стоимости и присваивание значения TotalPrice
                            bascmed.TotalPrice = bascmed.Price * bascmed.Count_Med;

                            bascmedlist.Add(bascmed);
                        }
                    }
                }

                // Вычисление общей стоимости всех товаров
                FullPrice = bascmedlist.Sum(item => item.TotalPrice);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке медикаментов из базы данных: {ex.Message}");
            }
        }

        private int CreateOrder()
        {
            int orderId = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionInfo.ConnString))
                {
                    connection.Open();
                    string query = @"
                        INSERT INTO Orders (user_id, apteka_id)
                        VALUES (@userId, (SELECT TOP 1 apteka_id FROM Apteka));
                        SELECT SCOPE_IDENTITY();";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@userId", user_Id);

                    object result = command.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out orderId))
                    {
                        MessageBox.Show("Заказ успешно создан.");
                    }
                    else
                    {
                        MessageBox.Show("Не удалось создать заказ.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании заказа: {ex.Message}");
            }
            return orderId;
        }

        private void DecreaseProductCountInApteka(int productId, int count)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionInfo.ConnString))
                {
                    connection.Open();
                    string query = @"
                        UPDATE Availability
                        SET [count] = [count] - @count
                        WHERE medikaments = @productId;";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@count", count);
                    command.Parameters.AddWithValue("@productId", productId);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Количество товара в аптеке успешно уменьшено.");
                    }
                    else
                    {
                        MessageBox.Show("Не удалось уменьшить количество товара в аптеке.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при уменьшении количества товара в аптеке: {ex.Message}");
            }
        }

        private void CreateOrd_Click(object sender, RoutedEventArgs e)
        {
            int orderId = CreateOrder();

            if (orderId != -1)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionInfo.ConnString))
                    {
                        connection.Open();
                        string deleteQuery = @"
                    DELETE FROM Order_List
                    WHERE order_id IN (SELECT order_id FROM Orders WHERE user_id = @userId)";

                        SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection);
                        deleteCommand.Parameters.AddWithValue("@userId", user_Id);
                        deleteCommand.ExecuteNonQuery();
                    }

                    StringBuilder receiptBuilder = new StringBuilder();
                    receiptBuilder.AppendLine("Чек заказа");
                    receiptBuilder.AppendLine("--------------------");

                    using (SqlConnection connection = new SqlConnection(connectionInfo.ConnString))
                    {
                        connection.Open();

                        foreach (var item in bascmedlist)
                        {
                            receiptBuilder.AppendLine($"Товар: {item.Name_product}, Количество: {item.Count_Med}, Цена: {item.Price}");

                            if (item.Subcategory_name == "Простуда и грипп" || item.Subcategory_name == "Обезбаливающие таблетки")
                            {
                                string checkReceptQuery = @"
                            SELECT code
                            FROM Reciepts
                            WHERE [user] = @userId AND medikament = @productId";

                                SqlCommand checkReceptCommand = new SqlCommand(checkReceptQuery, connection);
                                checkReceptCommand.Parameters.AddWithValue("@userId", user_Id);
                                checkReceptCommand.Parameters.AddWithValue("@productId", item.ProductUd);

                                object result = checkReceptCommand.ExecuteScalar();

                                if (result != null)
                                {
                                    string receptCode = result.ToString();
                                    receiptBuilder.AppendLine($"Код рецепта: {receptCode}");
                                }
                                else
                                {
                                    receiptBuilder.AppendLine("Требуется рецепт на этот медикамент.");
                                }
                            }

                            DecreaseProductCountInApteka(item.ProductUd, item.Count_Med);
                        }
                    }

                    receiptBuilder.AppendLine("--------------------");
                    receiptBuilder.AppendLine($"Общая стоимость: {FullPrice}");

                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string filePath = System.IO.Path.Combine(desktopPath, "чек_заказа.txt");
                    File.WriteAllText(filePath, receiptBuilder.ToString());

                    MessageBox.Show($"Чек заказа сохранен в файл: {filePath}");

                    bascmedlist.Clear();
                    FullPrice = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обработке заказа: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Не удалось создать заказ.");
            }
        }

        private void CloseBus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionInfo.ConnString))
                {
                    connection.Open();
                    string query = @"
                DELETE FROM Order_List
                FROM Order_List ol
                JOIN Orders o ON ol.order_id = o.order_id
                WHERE o.user_id = @userId;";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@userId", user_Id);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        // Очистить коллекцию товаров
                        bascmedlist.Clear();
                        // Обновить общую стоимость
                        FullPrice = 0;
                    }
                    else
                    {
                        MessageBox.Show("Failed to clear the cart.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error clearing the cart: {ex.Message}");
            }
        }
          private void btnBackAut_Click(object sender, RoutedEventArgs e)
        {
            
            NavigationService.GoBack();
        }
    }
}