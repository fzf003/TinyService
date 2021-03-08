using Microsoft.Extensions.Logging;
using Sample_1.CommandHandler;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinyService.Core;
using TinyService.RequestResponse;

namespace Sample_1.Actors
{
    public class UserActor: ProcessActor
    {
        readonly IStateManager stateManager;

        ILogger<UserActor> logger;

        public UserActor(ILogger<UserActor> logger)
        {
            this.stateManager = new StateManager();
            this.logger = logger;
        }

        public override void Handle(Started message)
        {
            logger.LogInformation($"Actor:{Self.ToShortString()} is Start!!");
        }

        public void Handle(SuccessMessage successMessage)
        {
            logger.LogInformation("成功:{0}",successMessage.Message);
        }

        public void Handle(FailureMessage  failureMessage)
        {
            logger.LogInformation("失败:{0}", failureMessage.Exception);
        }


        public override void Stop(PID pid)
        {
            base.Stop(pid);
            logger.LogInformation($"FANNNNActor:{Self.ToShortString()} is Stop!!");
        }
        

        public void Handle(User productCommand)
        {
            logger.LogInformation($"开始:{Thread.CurrentThread.ManagedThreadId}--{this.Self.Id}--{productCommand.Name}");
            //if (!await this.stateManager.ContainsStateAsync(this.Id))
            {
                this.stateManager.SetStateAsync(this.Self.Id, productCommand);
                    
               Task.Run(()=>throw new Exception("OOPPPP")) .ToPipe(this.Self, success: () =>
                    {
                        return new SuccessMessage(productCommand .Name+ "--"+Guid.NewGuid().ToString("N"));

                    }, failure: (ex) =>
                    {
                        return new FailureMessage(new Exception(productCommand.Name+"--aaa"));
                    });
            }

            logger.LogInformation($"{this.Self.Id}--执行中.....");

            logger.LogInformation($"结束:{Thread.CurrentThread.ManagedThreadId}--{this.Self.Id}--{productCommand.Name}");
        }

    }

    public class User
    {
        public string Name { get; set; }
    }

    public class SuccessMessage
    {
        public SuccessMessage(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
    public class FailureMessage
    {
        public FailureMessage(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }

    }


    public static class PipeToSupport
    {
       
        public static Task ToPipe<T>(this Task<T> taskToPipe,  Func<T, object> success = null, Func<Exception, object> failure = null)
        {
            return taskToPipe.ContinueWith(tresult =>
            {
                if (tresult.IsCanceled || tresult.IsFaulted)

                    failure(tresult.Exception);

                else if (tresult.IsCompleted)

                    success(tresult.Result);
                       
            }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

      
        public static Task ToPipe(this Task taskToPipe, PID recipient, Func<object> success = null, Func<Exception, object> failure = null)
        {
            
            return taskToPipe.ContinueWith(tresult =>
            {
                if (tresult.IsCanceled || tresult.IsFaulted)
                {
                    recipient.Tell(failure(tresult.Exception));
                }
                else if (tresult.IsCompleted && success != null)
                    recipient.Tell(success());
            }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }
    }
}
