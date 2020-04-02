using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualPAS.Models
{
    public class ResultsCollection
    {
        public string Range { get; set; }
        public string MajorDimension { get; set; }
        public List<JArray> Values { get; set; }
    }
}
