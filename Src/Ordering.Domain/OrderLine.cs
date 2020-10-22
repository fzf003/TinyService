using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ordering.Domain
{

    public class OrderLine : BaseEntity<long>
    {
        public long OrderId { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string ProductId { get; set; }
        [Column(TypeName = "nvarchar(200)")]
        public string Name { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;

        public void UpdateTime()
        {
            this.UpdateDate = DateTime.Now;
        }
    }


  
}
