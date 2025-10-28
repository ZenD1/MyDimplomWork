using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomZabpo43.filters
{
    public class CategoryList
    {
        public int Category_id { get; set; }
        public string Category_name { get; set; }
        public ObservableCollection<SubcategoryList> Subcategories { get; set; }


        public CategoryList(int category_id, string category_name)
        {
            Category_id = category_id;
            Category_name = category_name;
            Subcategories = new ObservableCollection<SubcategoryList>();

        }
    }
}