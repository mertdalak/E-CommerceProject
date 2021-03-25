using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kimyon.Models
{
    public class Cart
    {
        public int product_id { get; set; }
        public string product_name { get; set; }


        public float price { get; set; }

        public int  qty { get; set; }
        public float bill { get; set; }

    }
}