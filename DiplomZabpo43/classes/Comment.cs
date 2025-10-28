using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomZabpo43.classes
{
    public class Comment
    {
        public int ProductUd { get; set; } // Идентификатор продукта
        public string UserName { get; set; } // Имя пользователя
        public int Rating { get; set; } // Оценка
        public string Content_com { get; set; } // Содержание комментария

        // Конструктор класса Comment
        public Comment(int productId, string userName, int rating, string content_com)
        {
            ProductUd = productId;
            UserName = userName;
            Rating = rating;
            Content_com = content_com;
        }
    }
}