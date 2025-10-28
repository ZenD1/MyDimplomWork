using DiplomZabpo43.classes;
using DiplomZabpo43.filters;
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
    /// Логика взаимодействия для SetMedikamentPage.xaml
    /// </summary>
    public partial class SetMedikamentPage : Page
    {
        public int user_id;
        private byte[] selectedImageBytes;
        private int productId; // Предположим, что productId уже имеется или генерируется где-то еще

        public SetMedikamentPage(MainWindow mainWindow, int user_Id)
        {
            InitializeComponent();
            FillComboBox(categoryLabel, "category_name", "Categories");
            FillComboBox(brandLabel, "brand_name", "Brand");
            FillComboBox(manufacturerLabel, "manufac_name", "Manufacturer");
            FillComboBox(countryLabel, "name_country", "Country");
            FillComboBox(dosaLabel, "dosa_name", "Dosa");
            FillComboBox(subcategoryLabel, "subcategory_name", "Subcategories");
            FillComboBox(subsubcategoryLabel, "subsubcategory_name", "Subsubcategories");
            LoadCategories();  // Reload categories after adding a new one
            LoadSubcategories();
            LoadPharmacies();
            LoadMedications();
        }

        private void FillComboBox(ComboBox comboBox, string columnName, string tableName)
        {
            try
            {
                Connection connection = new Connection();

                // Устанавливаем соединение с базой данных
                using (SqlConnection sqlConnection = new SqlConnection(connection.ConnString))
                {
                    // Открываем соединение
                    sqlConnection.Open();

                    // Запрос к базе данных для получения данных для комбобокса
                    string query = $"SELECT {columnName} FROM dbo.{tableName}";

                    // Создаем команду для выполнения запроса
                    using (SqlCommand command = new SqlCommand(query, sqlConnection))
                    {
                        // Выполняем запрос и получаем результат
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Пока есть строки в результате запроса
                            while (reader.Read())
                            {
                                // Добавляем каждое значение в комбобокс
                                comboBox.Items.Add(reader[columnName].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Если произошла ошибка, выводим сообщение об ошибке
                MessageBox.Show($"Ошибка при заполнении комбобокса {comboBox.Name}: {ex.Message}");
            }
        }
        private void Btn_selectImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg; *.jpeg; *.png; *.bmp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    BitmapImage bitmapImage = new BitmapImage(new Uri(openFileDialog.FileName));
                    imgProfile.Source = bitmapImage;
                    selectedImageBytes = File.ReadAllBytes(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке изображения: " + ex.Message);
                }
            }
        }

        private void AddMed_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Collect data about the new medicament from input fields
                string name = nameLabel.Text;
                string categoryName = categoryLabel.SelectedItem.ToString();
                string brandName = brandLabel.SelectedItem.ToString();
                string manufacturerName = manufacturerLabel.SelectedItem.ToString();
                string form = formLabel.Text;
                string countryName = countryLabel.SelectedItem.ToString();
                string dosa = dosaLabel.SelectedItem.ToString();
                string subcategoryName = subcategoryLabel.SelectedItem.ToString();
                string subsubcategoryName = subsubcategoryLabel.SelectedItem.ToString();
                string storageCondition = storageConditionLabel.Text;
                string recommendation = recommendationLabel.Text;
                DateTime? manufactureDate = manufactureDatePicker.SelectedDate;
                int price = int.Parse(priceLabel.Text);

                // INSERT query to add a new medicament and get the new product_id
                string query = @"
            INSERT INTO Medikaments (
                name_product, 
                subsubcategory, 
                brand, 
                dosa, 
                form, 
                manufacturer, 
                country, 
                storage_condition, 
                contraindications, 
                dosa_id, 
                healt_date, 
                price,
                image_med
            )
            OUTPUT INSERTED.product_id
            SELECT 
                @Name, 
                ss.subsubcategory_id,
                b.brand_id,
                dosa.dosa_id,
                @Form,
                mf.manefac_id,
                c.country_id,
                @StorageCondition,
                @Recommendation,
                (SELECT dosa_id FROM Dosa WHERE dosa_name = @Dosa),
                @ManufactureDate,
                @Price,
                @ImageMed
            FROM Subsubcategories ss
            JOIN Brand b ON b.brand_name = @BrandName
            JOIN Dosa dosa ON dosa.dosa_name = @Dosa
            JOIN Manufacturer mf ON mf.manufac_name = @ManufacturerName
            JOIN Country c ON c.name_country = @CountryName
            WHERE ss.subsubcategory_name = @SubsubcategoryName;";

                // Establish connection to the database
                Connection connection = new Connection();

                using (SqlConnection sqlConnection = new SqlConnection(connection.ConnString))
                {
                    sqlConnection.Open();

                    using (SqlCommand command = new SqlCommand(query, sqlConnection))
                    {
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@SubsubcategoryName", subsubcategoryName);
                        command.Parameters.AddWithValue("@BrandName", brandName);
                        command.Parameters.AddWithValue("@Dosa", dosa);
                        command.Parameters.AddWithValue("@Form", form);
                        command.Parameters.AddWithValue("@ManufacturerName", manufacturerName);
                        command.Parameters.AddWithValue("@CountryName", countryName);
                        command.Parameters.AddWithValue("@StorageCondition", storageCondition);
                        command.Parameters.AddWithValue("@Recommendation", recommendation);
                        command.Parameters.AddWithValue("@ManufactureDate", (object)manufactureDate ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Price", price);
                        command.Parameters.AddWithValue("@ImageMed", selectedImageBytes ?? (object)DBNull.Value);

                        productId = (int)command.ExecuteScalar();

                        MessageBox.Show("Медикамент успешно добавлен.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении медикамента: " + ex.Message);
            }
        }

        private void categoryLabel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void nameLabel_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void brandLabel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void manufacturerLabel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void formLabel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void countryLabel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void dosaLabel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void subsubcategoryLabel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void subcategoryLabel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
      
        private void AddBrand_Click(object sender, RoutedEventArgs e)
        {
            string brandName = brandAddTextBox.Text;

            if (!string.IsNullOrWhiteSpace(brandName))
            {
                AddToDatabase("Brand", "brand_name", brandName);
            }
        }

        private void AddManufacturer_Click(object sender, RoutedEventArgs e)
        {
            string manufacturerName = manufacturerAddTextBox.Text;

            if (!string.IsNullOrWhiteSpace(manufacturerName))
            {
                AddToDatabase("Manufacturer", "manufac_name", manufacturerName);
            }
        }

     

        private void AddCountry_Click(object sender, RoutedEventArgs e)
        {
            string countryName = countryAddTextBox.Text;

            if (!string.IsNullOrWhiteSpace(countryName))
            {
                AddToDatabase("Country", "name_country", countryName);
            }
        }

        private void AddDosa_Click(object sender, RoutedEventArgs e)
        {
            string dosaName = dosaAddTextBox.Text;

            if (!string.IsNullOrWhiteSpace(dosaName))
            {
                AddToDatabase("Dosa", "dosa_name", dosaName);
            }
        }
        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            string categoryName = categoryAddTextBox.Text;

            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                AddToDatabase("Categories", "category_name", categoryName);
            }
        }

        private void AddSubcategory_Click(object sender, RoutedEventArgs e)
        {
            string subcategoryName = subcategoryAddTextBox.Text;
            if (categoryComboBox.SelectedItem != null)
            {
                int categoryId = (int)categoryComboBox.SelectedValue;

                if (!string.IsNullOrWhiteSpace(subcategoryName))
                {
                    AddToDatabase("Subcategories", "subcategory_name", subcategoryName, "category_id", categoryId);
                    LoadSubcategories();  // Reload subcategories after adding a new one
                }
            }
        }

        private void AddSubsubcategory_Click(object sender, RoutedEventArgs e)
        {
            string subsubcategoryName = subsubcategoryAddTextBox.Text;
            if (subcategoryComboBox.SelectedItem != null)
            {
                int subcategoryId = (int)subcategoryComboBox.SelectedValue;

                if (!string.IsNullOrWhiteSpace(subsubcategoryName))
                {
                    AddToDatabase("Subsubcategories", "subsubcategory_name", subsubcategoryName, "subcategory_id", subcategoryId);
                }
            }
        }
        private void Btn_back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
        private void LoadCategories()
        {
            categoryComboBox.ItemsSource = GetItems("SELECT category_id, category_name FROM Categories");
            categoryComboBox.DisplayMemberPath = "Value";
            categoryComboBox.SelectedValuePath = "Key";
        }

        private void LoadSubcategories()
        {
            subcategoryComboBox.ItemsSource = GetItems("SELECT subcategory_id, subcategory_name FROM Subcategories");
            subcategoryComboBox.DisplayMemberPath = "Value";
            subcategoryComboBox.SelectedValuePath = "Key";
        }
        private void UpdateQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (ComboApteka.SelectedItem == null || ComboMed.SelectedItem == null || string.IsNullOrWhiteSpace(QuantityTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, выберите аптеку, медикамент и укажите количество.");
                return;
            }

            int pharmacyId = (int)ComboApteka.SelectedValue;
            int medicationId = (int)ComboMed.SelectedValue;
            int quantity;
            if (!int.TryParse(QuantityTextBox.Text, out quantity))
            {
                MessageBox.Show("Количество должно быть числом.");
                return;
            }

            UpdateMedicationQuantity(pharmacyId, medicationId, quantity);
        }
        private void UpdateMedicationQuantity(int pharmacyId, int medicationId, int quantity)
        {
            try
            {
                string query = "UPDATE Availability SET count = @Quantity WHERE apteka = @PharmacyId AND medikaments = @MedicationId";

                Connection connection = new Connection();

                using (SqlConnection sqlConnection = new SqlConnection(connection.ConnString))
                {
                    sqlConnection.Open();

                    using (SqlCommand command = new SqlCommand(query, sqlConnection))
                    {
                        command.Parameters.AddWithValue("@Quantity", quantity);
                        command.Parameters.AddWithValue("@PharmacyId", pharmacyId);
                        command.Parameters.AddWithValue("@MedicationId", medicationId);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Количество успешно обновлено.");
                        }
                        else
                        {
                            MessageBox.Show("Ошибка: запись не найдена.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении количества: {ex.Message}");
            }
        }
    
    private void AddToDatabase(string tableName, string columnName, string value, string foreignKeyColumn = null, int? foreignKeyValue = null)
        {
            try
            {
                string query = $"INSERT INTO {tableName} ({columnName}{(foreignKeyColumn != null ? $", {foreignKeyColumn}" : "")}) VALUES (@Value{(foreignKeyValue != null ? ", @ForeignKeyValue" : "")})";

                Connection connection = new Connection();

                using (SqlConnection sqlConnection = new SqlConnection(connection.ConnString))
                {
                    sqlConnection.Open();

                    using (SqlCommand command = new SqlCommand(query, sqlConnection))
                    {
                        command.Parameters.AddWithValue("@Value", value);
                        if (foreignKeyValue != null)
                        {
                            command.Parameters.AddWithValue("@ForeignKeyValue", foreignKeyValue);
                        }
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show($"{value} успешно добавлено в таблицу {tableName}.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении в таблицу {tableName}: {ex.Message}");
            }
        }
        private void LoadPharmacies()
        {
            ComboApteka.ItemsSource = GetItems("SELECT apteka_id, adres FROM Apteka JOIN Adres ON Apteka.adres_apteki_id = Adres.adres_id");
            ComboApteka.DisplayMemberPath = "Value";
            ComboApteka.SelectedValuePath = "Key";
        }

        private void LoadMedications()
        {
            ComboMed.ItemsSource = GetItems("SELECT product_id, name_product FROM Medikaments");
            ComboMed.DisplayMemberPath = "Value";
            ComboMed.SelectedValuePath = "Key";
        }
        private Dictionary<int, string> GetItems(string query)
        {
            Dictionary<int, string> items = new Dictionary<int, string>();

            try
            {
                Connection connection = new Connection();

                using (SqlConnection sqlConnection = new SqlConnection(connection.ConnString))
                {
                    sqlConnection.Open();

                    using (SqlCommand command = new SqlCommand(query, sqlConnection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                items.Add(id, name);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }

            return items;
        }

    }
}