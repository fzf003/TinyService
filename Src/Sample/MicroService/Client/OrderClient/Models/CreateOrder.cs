using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderClient.Models
{
    /*{
  "customerId": "fzf007",
  "totalPrice": 10,
  "productLines": [
    {
      "productId": "product-110",
      "name": "苹果",
      "quantity": 1,
      "unitPrice": 10
    }
  ]
}*/
    public class CreateOrder
    {
        public string customerId { get; set; }

        public decimal totalPrice { get; set; }

        public List<OrderLine> productLines { get; set; }
        public CreateOrder()
        {
            this.productLines = new List<OrderLine>();
        }
    }

    public class OrderLine
    {
        public string productId { get; set; }

        public string name { get; set; }

        public int quantity { get; set; }

        public decimal unitPrice { get; set; }
    }


}
