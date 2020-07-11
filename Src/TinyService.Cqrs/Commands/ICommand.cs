using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TinyService.Cqrs.Commands
{
    public interface ICommand
    {
    }

  

    public interface ICommandHandler<in TCommand> where TCommand : class, ICommand
    {
        Task HandleAsync(TCommand command);
    }


    



}
