using DiplomZabpo43.classes;
using DiplomZabpo43.filters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
    /// Логика взаимодействия для SpisokPage.xaml
    /// </summary>
    public partial class SpisokPage : Page
    {
        private readonly MainWindow _mainWindow;

        private ObservableCollection<Medikament> medikaments;
        private ObservableCollection<CountryList> countryLists;
        private ObservableCollection<ManufacList> manufacLists;
        private ObservableCollection<DosaList> dosa;
        private ObservableCollection<CategoryList> categoryLists;
      
        private readonly Connection connectionInfo = new Connection();
        public CountryList SelectedCountry { get; set; }
        public ManufacList SelectedPublisher { get; set; }
        public BrandList SelectedBrand { get; set; }

        private bool isInitialized = false; // Добавляем переменную для отслеживания инициализации
        public CategoryList SelectedCategory { get; set; }

        public DosaList SelectedDosa { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int user_id;
        public SpisokPage(MainWindow mainWindow, int user_Id)
        {
            _mainWindow = mainWindow;
            InitializeComponent();
            this.user_id = user_Id;
            categoryLists = new ObservableCollection<CategoryList>();
            medikaments = new ObservableCollection<Medikament>();
            countryLists = new ObservableCollection<CountryList>();
            manufacLists = new ObservableCollection<ManufacList>(); // Initialize manufacLists collection
            dosa = new ObservableCollection<DosaList>(); // Initialize dosa collection
            PanelFilter.Visibility = Visibility.Collapsed;
            PanelFilter2.Visibility = Visibility.Collapsed;

            DataContext = this;

            ListViewMedTechnika.ItemsSource = medikaments;
            DataContext = this;
            SearchTextBox.TextChanged += SearchTextBox_TextChanged;
            FillPublishersList();
            LoadMedikamentFromDB();
            FillCountrysList();
            FillDosaList();
            FillBrandList();
            FillCategoriesList();
            DataContext = this;
            isInitialized = true; // Помечаем, что инициализация завершена
            SearchTexttt.Visibility = Visibility.Collapsed;
        }

      
        //-----------------------------------------------------------------------------------
        public int role_id;
        public int productId;

        private void ListViewMedTechnika_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Получаем выбранный элемент
            var selectedItem = ListViewMedTechnika.SelectedItem as Medikament;
            user_id = _mainWindow.user_Id;
            if (selectedItem != null)
            {
                // Создаем экземпляр страницы ItemDetailsPage.xaml
                ItemDetailsPage itemDetailsPage = new ItemDetailsPage(_mainWindow, user_id);
                // Передаем user_id
                itemDetailsPage.user_Id = user_id;             
                // Передаем productId
                itemDetailsPage.productId = selectedItem.ProductUd;

                // Передаем выбранный элемент в качестве DataContext
                itemDetailsPage.DataContext = selectedItem;

                // Переходим на страницу ItemDetailsPage.xaml
                NavigationService.Navigate(itemDetailsPage);
            }
        }
        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                SearchTextBox_TextChanged(null, null); // Вызов метода поиска при изменении текста
                OnPropertyChanged(nameof(SearchText));
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
                    m.form,
                    mf.manufac_name,
                    m.price,
                    c.name_country,
                    d.dosa_name,
                    ss.subsubcategory_name,
                    sc.subcategory_name,
                    cat.category_name,
                    m.storage_condition,
                    m.contraindications,
                    cmt.content_com,
                    cmt.raiting
                FROM 
                    dbo.Medikaments m
                LEFT JOIN dbo.Brand b ON m.brand = b.brand_id
                RIGHT JOIN dbo.Manufacturer mf ON mf.manefac_id = m.manufacturer
                RIGHT JOIN dbo.Subsubcategories ss ON ss.subsubcategory_id = m.subsubcategory
                JOIN dbo.Subcategories sc ON ss.subcategory_id = sc.subcategory_id
                JOIN dbo.Country c ON c.country_id = m.country
                LEFT JOIN dbo.Dosa d ON m.dosa_id = d.dosa_id
                JOIN dbo.Categories cat ON sc.category_id = cat.category_id
                FULL JOIN (
                    SELECT 
                        medikament,
                        STUFF((
                            SELECT ', ' + content_com
                            FROM dbo.Comment c2
                            WHERE c.medikament = c2.medikament
                            FOR XML PATH('')), 1, 2, '') AS content_com,
                        MAX(raiting) AS raiting
                    FROM dbo.Comment c
                    GROUP BY medikament
                ) cmt ON m.product_id = cmt.medikament
                ORDER BY m.name_product";

                    SqlCommand command = new SqlCommand(query, connection);

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
                            var storageCondition = reader.IsDBNull(12) ? null : reader.GetString(12);
                            var contraindications = reader.IsDBNull(13) ? null : reader.GetString(13);
                            var commentContent = reader.IsDBNull(14) ? null : reader.GetString(14);
                            var rating = reader.IsDBNull(15) ? 0 : reader.GetInt32(15);

                            var medikament = new Medikament(productId, imageMedBytes, nameProduct, brandName, form, manufacName, price, countryName, dosa_name, subsubcategory_name, subcategory_name, category_name, storageCondition, contraindications, commentContent, rating);
                            medikaments.Add(medikament);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке медикаментов из базы данных: {ex.Message}");
            }
        }
        private void FillPublishersList()
        {
            List<ManufacList> publishers = new List<ManufacList>();

            string query = "SELECT [manefac_id], [manufac_name] FROM [dbo].[Manufacturer] ORDER BY manufac_name";

            using (SqlConnection connection = new SqlConnection(connectionInfo.ConnString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int manufacId = reader.GetInt32(0);
                    string manufacName = reader.GetString(1);
                    publishers.Add(new ManufacList(manufacId, manufacName));
                }
                reader.Close();
            }

            PublicherList.DataContext = publishers;
        }

        private void FillCountrysList()
        {
            List<CountryList> countryLists = new List<CountryList>();

            string query = "SELECT [country_id], [name_country] FROM [dbo].[Country] ORDER BY name_country";

            using (SqlConnection connection = new SqlConnection(connectionInfo.ConnString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int country_id = reader.GetInt32(0);
                    string country_name = reader.GetString(1);
                    countryLists.Add(new CountryList(country_id, country_name));
                }
                reader.Close();
            }

            CountryList.DataContext = countryLists;
        }

        private void FillDosaList()
        {
            List<DosaList> dosa = new List<DosaList>();

            string query = "select [dosa_id],[dosa_name] from [dbo].[Dosa] ORDER BY dosa_name";

            using (SqlConnection connection = new SqlConnection(connectionInfo.ConnString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int dosa_id = reader.GetInt32(0);
                    string dosa_name = reader.GetString(1);
                    dosa.Add(new DosaList(dosa_id, dosa_name));
                }
                reader.Close();
            }

            DosaList.DataContext = dosa;
        }

        private void FillBrandList()
        {
            List<BrandList> brand = new List<BrandList>();

            string query = "SELECT [brand_id], [brand_name] FROM [dbo].[Brand] ORDER BY [brand_name]";

            using (SqlConnection connection = new SqlConnection(connectionInfo.ConnString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int brand_id = reader.GetInt32(0);
                    string brand_name = reader.GetString(1);
                    brand.Add(new BrandList(brand_id, brand_name));
                }
                reader.Close();
            }

            BrandList.DataContext = brand;
        }
        private void FillCategoriesList()
        {
            List<CategoryList> categories = new List<CategoryList>();

            string query = @"SELECT category_id, category_name FROM Categories ORDER BY category_name";

            using (SqlConnection connection = new SqlConnection(connectionInfo.ConnString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int categoryId = reader.GetInt32(0);
                    string categoryName = reader.GetString(1);
                    categories.Add(new CategoryList(categoryId, categoryName));
                }
                reader.Close();
            }

            CategoryList.DataContext = categories;
        }


        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ApplyFilters(SelectedCategory, SelectedPublisher, SelectedCountry, SelectedDosa, SelectedBrand);
            priceLabel.Content = ((int)priceSlider.Value).ToString();
        }

        private void CloseFilt0_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(user_id.ToString());
            MessageBox.Show(_mainWindow.user_Id.ToString());
            user_id = _mainWindow.user_Id;
                priceSlider.Value = priceSlider.Minimum;
            priceLabel.Content = priceSlider.Value.ToString();
            ApplyFilters(SelectedCategory, SelectedPublisher, SelectedCountry, SelectedDosa, SelectedBrand);
        }

        private void CloseFilt1_Click(object sender, RoutedEventArgs e)
        {
            SelectedPublisher = null; // Сброс выбранного издателя
            selectlable1.Content = "Не выбран"; // Очистка метки для издателя
            ApplyFilters(SelectedCategory, null, SelectedCountry, SelectedDosa, SelectedBrand);
            PublicherList.SelectedIndex = -1; // Сброс выбранного элемента в ListBox
        }


        private void CloseFilt2_Click(object sender, RoutedEventArgs e)
        {
            SelectedCountry = null; // Сброс выбранной страны
            selectlable2.Content = "Не выбрана"; // Очистка метки для страны
            ApplyFilters(SelectedCategory, SelectedPublisher, null, SelectedDosa, SelectedBrand);
            CountryList.SelectedIndex = -1; // Сброс выбранного элемента в ListBox
        }
        private void CloseFilt3_Click(object sender, RoutedEventArgs e)
        {
            SelectedDosa = null; // Сброс выбранной дозы
            selectlable3.Content = "Не выбрана"; // Очистка метки для дозы
            ApplyFilters(SelectedCategory, SelectedPublisher, SelectedCountry, null, SelectedBrand);
            DosaList.SelectedIndex = -1; // Сброс выбранного элемента в ListBox
        }
        private void CloseFilt4_Click(object sender, RoutedEventArgs e)
        {
            SelectedBrand = null; // Сброс выбранной дозы
            selectlable4.Content = "Не выбрана"; // Очистка метки для дозы
            ApplyFilters(SelectedCategory, SelectedPublisher, SelectedCountry, SelectedDosa, null);
            BrandList.SelectedItem = -1; // Сброс выбранного элемента в ListBox
        }
        private void CloseFilt5_Click(object sender, RoutedEventArgs e)
        {
            SelectedCategory = null; // Сброс выбранной категории
            selectlable5.Content = "Не выбрана"; // Очистка метки для категории
            ApplyFilters(null, SelectedPublisher, SelectedCountry, SelectedDosa, SelectedBrand);
            CategoryList.SelectedItem = -1; // Сброс выбранного элемента в ListBox
        }


        private void ApplyFilters(CategoryList selectedCategory, ManufacList selectedPublisher, CountryList selectedCountry, DosaList selectedDosa, BrandList selectedBrand)
        {
            var filteredMedikaments = medikaments.AsEnumerable();

            if (priceSlider.Value > priceSlider.Minimum)
            {
                filteredMedikaments = filteredMedikaments.Where(m => m.Price <= priceSlider.Value);
            }

            if (selectedCategory != null)
            {
                filteredMedikaments = filteredMedikaments.Where(m => m.Category_name == selectedCategory.Category_name);
            }

            if (selectedPublisher != null)
            {
                filteredMedikaments = filteredMedikaments.Where(m => m.Manufac_name == selectedPublisher.Manufac_name);
            }

            if (selectedCountry != null)
            {
                filteredMedikaments = filteredMedikaments.Where(m => m.Country_name == selectedCountry.Name_country);
            }

            if (selectedDosa != null)
            {
                filteredMedikaments = filteredMedikaments.Where(m => m.Dosa_name == selectedDosa.Dosa_name);
            }

            if (selectedBrand != null)
            {
                filteredMedikaments = filteredMedikaments.Where(m => m.Brand_name == selectedBrand.Brand_name);
            }

            var resultList = filteredMedikaments.ToList();
            ListViewMedTechnika.ItemsSource = resultList.Any() ? resultList : null;

            // Закрытие экспандеров
            ClosCont.IsExpanded = false;
            CloseDosaExpander.IsExpanded = false;
            BrandExpander.IsExpanded = false;
            CloseCategExpander.IsExpanded = false;
        }
        private void ListBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox != null)
            {
                var selectedCategory = listBox.SelectedItem as CategoryList;
                SelectedCategory = selectedCategory;
                ApplyFilters(SelectedCategory, SelectedPublisher, SelectedCountry, SelectedDosa, SelectedBrand);
                selectlable5.Content = selectedCategory != null ? selectedCategory.Category_name : "Не выбран";
            }
        }



        private void ListBoxDosa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox != null)
            {
                var selectedDosa = listBox.SelectedItem as DosaList;
                SelectedDosa = selectedDosa;
                ApplyFilters(SelectedCategory, SelectedPublisher, SelectedCountry, SelectedDosa, SelectedBrand);
                selectlable3.Content = selectedDosa != null ? selectedDosa.Dosa_name : "Не выбрана";
            }
        }
       
        private void ListBoxPublishers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox != null)
            {
                var selectedPublisher = listBox.SelectedItem as ManufacList;
                SelectedPublisher = selectedPublisher;
                ApplyFilters(SelectedCategory, SelectedPublisher, SelectedCountry, SelectedDosa, SelectedBrand);
                selectlable1.Content = selectedPublisher != null ? selectedPublisher.Manufac_name : "Не выбран";
            }
        }

        private void ListBoxCountryExpa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox != null)
            {
                var selectedCountry = listBox.SelectedItem as CountryList;
                SelectedCountry = selectedCountry;
                ApplyFilters(SelectedCategory, SelectedPublisher, selectedCountry, SelectedDosa, SelectedBrand);
                selectlable2.Content = selectedCountry != null ? selectedCountry.Name_country : "Не выбрана";
            }
        }

        private void ListBoxBrands_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox != null)
            {
                var selectedBrand = listBox.SelectedItem as BrandList;
                SelectedBrand = selectedBrand;
                ApplyFilters(SelectedCategory, SelectedPublisher, SelectedCountry, SelectedDosa, SelectedBrand);
                selectlable4.Content = selectedBrand != null ? selectedBrand.Brand_name : "Не выбран";
            }
        }
       
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                string searchText = textBox.Text.ToLower();

                var filteredMedikaments = medikaments.Where(m => m.Name_product.ToLower().Contains(searchText) || m.Brand_name.ToLower().Contains(searchText)).ToList();

                if (filteredMedikaments.Any())
                {
                    ListViewMedTechnika.ItemsSource = new ObservableCollection<Medikament>(filteredMedikaments);
                }
                else
                {
                    ListViewMedTechnika.ItemsSource = null;
                }
            }
        }
      

       
    }
}