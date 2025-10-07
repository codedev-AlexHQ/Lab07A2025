using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Product
    {
        public int ProductId { get; set; }   // product_id
        public string Name { get; set; }     // name
        public decimal Price { get; set; }   // price
        public int Stock { get; set; }       // stock
        public bool Active { get; set; }     // active
    }
}
