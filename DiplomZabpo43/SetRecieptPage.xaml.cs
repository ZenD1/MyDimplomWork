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
    /// Логика взаимодействия для SetRecieptPage.xaml
    /// </summary>
    public partial class SetRecieptPage : Page
    {

        public SetRecieptPage()
        {
            InitializeComponent();
            FillUserComboBox();
            FillMedikamentComboBox();
        }
        private void GenerateCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            string code = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private void FillUserComboBox()
        {
            LoadUsers();
        }
        private void LoadUsers()
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(new Connection().ConnString))
                {
                    sqlConnection.Open();
                    SqlCommand command = new SqlCommand("SELECT user_id, email FROM User_table", sqlConnection);
                    SqlDataReader reader = command.ExecuteReader();
                    List<User> users = new List<User>();

                    while (reader.Read())
                    {
                        users.Add(new User(
                            reader.GetInt32(0), // UserId
                            reader.GetString(1), // Email
                            null, // Phone
                            null, // Password
                            0, // RoleId (You might need to change this if you want to include roles)
                            null // RoleName
                        ));
                    }

                    combouser.ItemsSource = users;
                    combouser.DisplayMemberPath = "Email";
                    combouser.SelectedValuePath = "UserId";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}");
            }
        }
        private void LoadMedikaments()
        {
            try
            {
                Connection connection = new Connection();
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

                using (SqlConnection sqlConnection = new SqlConnection(connection.ConnString))
                {
                    sqlConnection.Open();
                    using (SqlCommand command = new SqlCommand(query, sqlConnection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<Medikament> medikaments = new List<Medikament>();

                            while (reader.Read())
                            {
                                int productId = reader.GetInt32(0);
                                byte[] imageMed = reader.IsDBNull(1) ? null : (byte[])reader.GetValue(1);
                                string nameProduct = reader.GetString(2);
                                string brandName = reader.GetString(3);
                                string form = reader.GetString(4);
                                string manufacName = reader.GetString(5);
                                int price = reader.GetInt32(6);
                                string countryName = reader.GetString(7);
                                string dosaName = reader.IsDBNull(8) ? null : reader.GetString(8);
                                string subsubcategoryName = reader.GetString(9);
                                string subcategoryName = reader.GetString(10);
                                string categoryName = reader.GetString(11);
                                string storageCondition = reader.IsDBNull(12) ? null : reader.GetString(12);
                                string contraindications = reader.IsDBNull(13) ? null : reader.GetString(13);
                                string commentContent = reader.IsDBNull(14) ? null : reader.GetString(14);
                                int rating = reader.IsDBNull(15) ? 0 : reader.GetInt32(15);

                                medikaments.Add(new Medikament(
                                    productId,
                                    imageMed,
                                    nameProduct,
                                    brandName,
                                    form,
                                    manufacName,
                                    price,
                                    countryName,
                                    dosaName,
                                    subsubcategoryName,
                                    subcategoryName,
                                    categoryName,
                                    storageCondition,
                                    contraindications,
                                    commentContent,
                                    rating
                                ));
                            }

                            combomed.ItemsSource = medikaments;
                            combomed.DisplayMemberPath = "Name_product";
                            combomed.SelectedValuePath = "ProductUd";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading medikaments: {ex.Message}");
            }
        }
        private void FillMedikamentComboBox()
        {
            LoadMedikaments();
        }

        private void combouser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GenerateCode();
        }

        private void combomed_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GenerateCode();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the selected user and medikament
                User selectedUser = combouser.SelectedItem as User;
                Medikament selectedMedikament = combomed.SelectedItem as Medikament;

                // Check if both user and medikament are selected
                if (selectedUser != null && selectedMedikament != null)
                {
                    // Generate a code for the receipt
                    
                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    Random random = new Random();
                    string code = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());

                    // Insert the new receipt into the database
                    using (SqlConnection connection = new SqlConnection(new Connection().ConnString))
                    {
                        connection.Open();
                        string query = "INSERT INTO Reciepts ([user], medikament, code) VALUES (@UserId, @ProductId, @Code)";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@UserId", selectedUser.UserId);
                        command.Parameters.AddWithValue("@ProductId", selectedMedikament.ProductUd);
                        command.Parameters.AddWithValue("@Code", code);
                        command.ExecuteNonQuery();
                        lableKodl.Content = code;
                        MessageBox.Show("Receipt added successfully!");
                    }
                }
                else
                {
                    MessageBox.Show("Please select both a user and a medikament before adding a receipt.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding receipt: {ex.Message}");
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();

        }
    }
}
    