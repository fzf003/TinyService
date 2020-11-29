using System;
using System.Collections.Generic;
using System.Text;
using TinyService.Cqrs.Commands;

namespace Ordering.Application.Commands
{
    public class CancelOrderCommand :ICommand
    {
            
        public int OrderNumber { get;  set; }
       
       
    }
}
