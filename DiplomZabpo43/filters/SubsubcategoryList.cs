using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomZabpo43.filters
{
    public class SubsubcategoryList
    {
        public int Subsubcategory_id { get; set; }
        public string Subsubcategory_nane { get; set; }


        public SubsubcategoryList(int subsubcategory_id, string subsubcategory_nane)
        {
            Subsubcategory_id = subsubcategory_id;
            Subsubcategory_nane = subsubcategory_nane;
        }
    }
}
