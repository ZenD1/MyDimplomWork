using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomZabpo43.filters
{
    public class BrandList
    {
        public int Brand_id { get; set; }
        public string Brand_name { get; set; }

        public BrandList(int brand_id, string brand_name)
        {
            Brand_id = brand_id;
            Brand_name = brand_name;
        }
    }
}
