using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomZabpo43.filters
{
    public class SubcategoryList
    {
        public int Subcategory_id { get; set; }
        public string Subcategory_nane { get; set; }


        public SubcategoryList(int subcategory_id, string subcategory_name)
        {
            Subcategory_id = subcategory_id;
            Subcategory_nane = subcategory_name;
        }
    }
}
