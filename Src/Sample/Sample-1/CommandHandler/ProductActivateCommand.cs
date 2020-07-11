using System;
using System.Collections.Generic;
using System.Text;
using TinyService.Cqrs.Commands;

namespace Sample_1.CommandHandler
{
    public class ProductActivateCommand:ICommand
    {
        public ProductActivateCommand(int id, DateTime createTime)
        {
            Id = id;
            CreateTime = createTime;
        }

        public int Id { get; }

        public DateTime CreateTime { get; }
    }
}
