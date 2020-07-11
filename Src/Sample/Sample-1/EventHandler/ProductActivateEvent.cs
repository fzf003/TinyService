using System;
using System.Collections.Generic;
using System.Text;
using TinyService.Cqrs.Events;

namespace Sample_1.EventHandler
{
    public class ProductActivateEvent:IEvent
    {
        public ProductActivateEvent(int id, DateTime createTime, string name)
        {
            Id = id;
            CreateTime = createTime;
            Name = name;
        }

        public int Id { get; }

        public string Name { get; }

        public DateTime CreateTime { get; }
    }
}
