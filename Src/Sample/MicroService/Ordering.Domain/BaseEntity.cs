using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain
{
    public class BaseEntity<T>
    {
        public T Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public BaseEntity()
        {
            this.CreateDate = DateTime.Now;
        }
    }
}
