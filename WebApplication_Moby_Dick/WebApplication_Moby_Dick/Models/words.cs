using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace WebApplication_Moby_Dick.Models
{
    public class words
    {

        public string text { get; set; }
        public int count { get; set; }


    }
    public class liste
    {
        public List<words> kelimeler { get; set; }
        public liste()

        { kelimeler = new List<words>(); }
    }
    

}
