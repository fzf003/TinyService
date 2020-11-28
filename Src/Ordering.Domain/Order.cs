using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordering.Domain
{

    public class Order : BaseEntity<long>
    {
        [Column(TypeName = "nvarchar(200)")]
        public string CustomerId { get; set; }
        
        [Column(TypeName = "decimal(15, 2)")]
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }

        public List<OrderLine> Items { get; set; }

       

        public void Cancel()
        {
            this.Status = OrderStatus.Cancel;
            this.UpdateDate = DateTime.Now;
        }

        public void Complete()
        {
            this.Status = OrderStatus.Completed;
            this.UpdateDate = DateTime.Now;
        }

       
        public void Failed()
        {
            this.Status = OrderStatus.Failed;
            this.UpdateDate = DateTime.Now;
        }

    }

    public enum OrderStatus
    {
        Created = 0,
        Completed = 1,
        Failed = 2,
        Cancel=3
    }



}
