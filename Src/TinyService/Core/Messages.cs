using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyService.Utils;
using System.Threading;

using System.Collections.Concurrent;
using TinyService.RequestResponse;

namespace TinyService.Core
{
 
  /* public class ProcessStarted : IMessage
    {
        public string ProcessId { get; private set; }

        public Func<string, IActor> Props { get; private set; }

        public Type ActorType { get; private set; }

        public ProcessStarted(string processId, Func<string, IActor> props, Type actorType = null)
        {
            ProcessId = processId;
            Props = props;
            ActorType = actorType;
        }
    }


    public class ProcessStopped : IMessage
    {
        public string ProcessId { get; private set; }

        public Type ActorType { get; private set; }

        public ProcessStopped(string processId, Type actorType)
        {
            ProcessId = processId;
            ActorType = actorType;
        }
    }

    public class ProcessReStart : IMessage
    {
        public string ProcessId { get; private set; }

        public Type ActorType { get; private set; }

        public ProcessReStart(string processId, Type actorType)
        {
            ProcessId = processId;
            ActorType = actorType;
        }
    }

    public class ShowActorConsole : IMessage
    {
        public static ShowActorConsole Instance = new ShowActorConsole();
    }

    public interface IProcessManager : IActor
    {
        IActor ProcessOf(string processId);

        int Types { get; }

        int Instance { get; }
    }
    /// <summary>
    /// 暂时废弃
    /// </summary>
    public class Messages : ProcessActor, IProcessManager
    {

        private readonly ConcurrentDictionary<string, IActor> _processes;

        private readonly ConcurrentDictionary<Type, Func<string, IActor>> _props;


        ActorPropsRegistry _regsitry;

        public Messages():this(new ActorPropsRegistry())
        {

        }

        public Messages(ActorPropsRegistry regsitry)
        {
            _processes = new ConcurrentDictionary<string, IActor>();

            _regsitry = regsitry;
             
            _props = new ConcurrentDictionary<Type, Func<string, IActor>>();
        }

        public IActor ProcessOf(string processId)
        {
            if (_processes.ContainsKey(processId))
            {
                return _processes[processId];
            }
            return default(IActor);
        }

        public virtual void Handle(ProcessStarted Started)
        {
            RegisterProps(Started);

            if (Started.Props == null)
            {
                Func<string, IActor> funcactor;

                if (_props.TryGetValue(Started.ActorType, out funcactor))
                {
                    StartProcess(Started.ProcessId, funcactor);
                }
                return;
            }

            StartProcess(Started.ProcessId, Started.Props);
        }

        public virtual void Handle(ProcessStopped Stop)
        {
            StopProcess(Stop.ProcessId);
        }

        public void Handle(ProcessReStart ReStart)
        {
            var actorProps = _props[ReStart.ActorType];

            if (actorProps != null)
            {
                this.Handle(new ProcessStopped(ReStart.ProcessId, ReStart.ActorType));
                this.Handle(new ProcessStarted(ReStart.ProcessId, actorProps, ReStart.ActorType));
            }
        }

        /// <summary>
        /// 启动之前
        /// </summary>
        /// <param name="ProcessId"></param>
        protected virtual void PreStart(string ProcessId)
        {

        }

        protected virtual void PreStop(string ProcessId)
        {

        }

        /// <summary>
        /// 注册
        /// </summary>
        void RegisterProps(ProcessStarted Started)
        {
            _props.TryAdd(Started.ActorType, Started.Props);

           
        }

        /// <summary>
        /// 注册Actor
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="process"></param>
        void StartProcess(string processId, Func<string, IActor> props)
        {
            if (!_processes.ContainsKey(processId))
            {
                //  Console.WriteLine("Actor:{0}启动", processId);

                var actor = props(processId);
                _processes.TryAdd(processId, actor);
                PreStart(processId);
            }
        }

        /// <summary>
        /// 停止一个Actor
        /// </summary>
        /// <param name="processId"></param>
        void StopProcess(string processId)
        {
            if (_processes.ContainsKey(processId))
            {

                IActor process;
                _processes.TryRemove(processId, out process);
                process.Dispose();
                PreStop(processId);
            }
        }

        public override void Handle(ActorErrorMessage message)
        {
            Console.WriteLine("err:{0}", message.Exception.Message);
        }


        public void Handle(ShowActorConsole message)
        {
            Console.WriteLine("==================={0}=========================", "注册类型");
            foreach (var props in this._props)
            {
                Console.WriteLine("{0}", props.Key);
            }
            Console.WriteLine("==========================================================\n");
            Console.WriteLine("=======================实例=================================");
            foreach (var process in this._processes)
            {
                Console.WriteLine("{0}-{1}", process.Key, process.Value);
            }
            Console.WriteLine("==========================================================\n");
        }

        public int Types
        {
            get
            {
                return _props.Count;
            }
        }

        public int Instance
        {
            get
            {
                return _processes.Count;
            }
        }
    }

    public static class ProcessManagerExt
    {
        public static void Register<TActor>(this IProcessManager manager, string actorname, Func<string, IActor> actoraction) where TActor : IActor
        {
            manager.SendAsync(new ProcessStarted(actorname, actoraction, typeof(TActor)));
        }

        public static void Start<TActor>(this IProcessManager manager, string actorname) where TActor : IActor
        {
            manager.SendAsync(new ProcessStarted(actorname, null, typeof(TActor)));
        }

        public static void Stop<TActor>(this IProcessManager manager, string actorname) where TActor : IActor
        {
            manager.SendAsync(new ProcessStopped(actorname, typeof(TActor)));
        }

        public static void ReStart<TActor>(this IProcessManager manager, string actorname) where TActor : IActor
        {
            manager.SendAsync(new ProcessReStart(actorname, typeof(TActor)));
        }

    }*/
   
}
