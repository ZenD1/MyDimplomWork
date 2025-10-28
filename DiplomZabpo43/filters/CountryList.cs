using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomZabpo43.filters
{
    public class CountryList
    {
        public int Country_id { get; set; }
        public string Name_country { get; set; }

        public CountryList(int country_id, string name_country)
        {
            Country_id = country_id;
            Name_country = name_country;
        }
    }
}
