using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomZabpo43.filters
{
    public class ManufacList
    {
        public int Manufac_id { get; set; }
        public string Manufac_name { get; set; }

        public ManufacList(int manufac_id, string manufac_name)
        {
            Manufac_id = manufac_id;
            Manufac_name = manufac_name;
        }
    }
}
