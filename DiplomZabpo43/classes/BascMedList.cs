using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DiplomZabpo43.classes
{
    namespace DiplomZabpo43.classes
    {
        public class BascMedList
        {
            public int ProductUd { get; set; }
            public BitmapImage Image_med { get; set; }
            public string Name_product { get; set; }
            public string Brand_name { get; set; }
            public string Form { get; set; }
            public string Manufac_name { get; set; }
            public int Price { get; set; }
            public string Country_name { get; set; } // Добавляем свойство для страны производителя
            public string Dosa_name { get; set; } // Добавляем свойство для дозы применения
            public string Subsubcategory_name { get; set; } // Добавляем свойство для подкатегории
            public string Subcategory_name { get; set; } // Добавляем свойство для категории
            public string Category_name { get; set; } // Добавляем свойство для категории
            public int Count_Med { get; set; }
            public int TotalPrice { get; set; } // Общая стоимость товара (Цена * Количество)

            public BascMedList(int productId, byte[] imageMed, string nameProduct, string brandName, string form, string manufacName, int price, string country_name, string dosa_name, string subsubcategory_name, string subcategory_name, string category_name, int count_Med)
            {
                ProductUd = productId;
                Image_med = ByteArrayToBitmapImage(imageMed);
                Name_product = nameProduct;
                Brand_name = brandName;
                Form = form;
                Manufac_name = manufacName;
                Price = price;
                Country_name = country_name;
                Dosa_name = dosa_name;
                Subsubcategory_name = subsubcategory_name;
                Subcategory_name = subcategory_name;
                Category_name = category_name;
                Count_Med = count_Med;
                TotalPrice = price * count_Med; // Вычисляем общую стоимость
            }

            private BitmapImage ByteArrayToBitmapImage(byte[] byteArray)
            {
                if (byteArray == null || byteArray.Length == 0) return null;

                BitmapImage bitmapImage = new BitmapImage();
                using (MemoryStream memoryStream = new MemoryStream(byteArray))
                {
                    memoryStream.Position = 0;
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = memoryStream;
                    bitmapImage.EndInit();
                }

                return bitmapImage;
            }
        }
    }
}