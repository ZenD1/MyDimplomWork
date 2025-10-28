using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomZabpo43.filters
{
    public class DosaList
    {
        public int Dosa_id { get; set; }
        public string Dosa_name { get; set; }


        public DosaList(int dosa_id, string dosa_name)
        {
            Dosa_id = dosa_id;
            Dosa_name = dosa_name;
        }
    }
   
}
