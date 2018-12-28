using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interface.Models
{
    public class SearchContentDTO
    {
        public string Input { get; set; }

        public override string ToString()
        {
            return Input;
        }
    }
}
