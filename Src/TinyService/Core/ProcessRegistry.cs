using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TinyService.ServiceBus;
using TinyService.Utils;

namespace TinyService.Core
{
    public class ProcessRegistry : SingletonBase<ProcessRegistry>, IInitializable
    {
        private const string NoHost = "nonhost";
        private readonly IList<Func<PID, Process>> _hostResolvers = new List<Func<PID, Process>>();
        private readonly HashedConcurrentDictionary _localActorRefs = new HashedConcurrentDictionary();
        private int _sequenceId;
 
        public string Address { get; set; }

        public void RegisterHostResolver(Func<PID, Process> resolver)
        {
            _hostResolvers.Add(resolver);
            this.Address = NoHost;
        }

        public Process Get(PID pid)
        {
            if (pid.Address != NoHost && pid.Address != Address)
            {
                foreach (var resolver in _hostResolvers)
                {
                    var reff = resolver(pid);
                    if (reff == null)
                    {
                        continue;
                    }
                    return reff;
                }
                throw new NotSupportedException("Unknown host");
            }

            Process process;

            if (_localActorRefs.TryGetValue(pid.Id, out process))
            {
                return process;
            }
            return DeadLetterProcess.Instance;

        }

        public Tuple<PID, bool> TryAdd(string id, Process process)
        {
            var pid = new PID(Address, id, process);

            var ok = _localActorRefs.TryAdd(pid.Id, process);


            return Tuple.Create<PID, bool>(pid, ok);
        }

        public Tuple<PID, bool> TryAdd(string id, string address, Process process)
        {

            var pid = new PID(address, id, process);

            var ok = _localActorRefs.TryAdd(pid.Id, process);


            return Tuple.Create<PID, bool>(pid, ok);
        }

        public Tuple<PID, bool> TryGet(string id)
        {
            Process process;
            return _localActorRefs.TryGetValue(id, out  process)
                       ? Tuple.Create<PID, bool>(new PID(Address, id, process), true)
                       : Tuple.Create<PID, bool>(null, false);
        }

        public Tuple<PID, bool> TryGet(string id, string address)
        {
            Process process;
            return _localActorRefs.TryGetValue(id, out  process)
                       ? Tuple.Create<PID, bool>(new PID(address, id, process), true)
                       : Tuple.Create<PID, bool>(null, false);
        }


        public void Remove(PID pid)
        {

            _localActorRefs.Remove(pid.Id);
        }

        public string NextId()
        {
            var counter = Interlocked.Increment(ref _sequenceId);
            return "$" + counter;
        }

        public void Print()
        {
            var nodes=this._localActorRefs.GetPartition().SelectMany(p => p)
                .ToList();

            foreach (var item in nodes)
            {
                Console.WriteLine(item.Key + "==" + item.Value);
            }
 
        }

        public void Initialize()
        {
            
        }
    }



    internal class HashedConcurrentDictionary
    {
        private const int HashSize = 1024;
        private readonly Partition[] _partitions = new Partition[HashSize];

        internal HashedConcurrentDictionary()
        {
            for (var i = 0; i < _partitions.Length; i++)
            {
                _partitions[i] = new Partition();
            }
        }

        static ulong CalculateHash(string read)
        {
            var hashedValue = 3074457345618258791ul;
            for (var i = 0; i < read.Length; i++)
            {
                hashedValue += read[i];
                hashedValue *= 3074457345618258799ul;
            }
            return hashedValue;
        }

        private Partition GetPartition(string key)
        {
            var hash = Math.Abs(key.GetHashCode()) % HashSize;
            var p = _partitions[hash];
            return p;
        }

        public bool TryAdd(string key, Process reff)
        {
            var p = GetPartition(key);
            lock (p)
            {
                if (p.ContainsKey(key))
                {
                    return false;
                }
                p.Add(key, reff);
                return true;
            }
        }

        public bool TryGetValue(string key, out Process aref)
        {
            var p = GetPartition(key);
            lock (p)
            {
                return p.TryGetValue(key, out aref);
            }
        }

        public void Remove(string key)
        {
            var p = GetPartition(key);
            lock (p)
            {
                p.Remove(key);
            }
        }

        public Partition[] GetPartition()
        {
            return this._partitions;
        }

        public class Partition : Dictionary<string, Process>
        { }
    }


    public class DeadLetterEvent
    {
        internal DeadLetterEvent(PID pid, object message, PID sender)
        {
            Pid = pid;
            Message = message;
            Sender = sender;
        }

        public PID Pid { get; private set; }
        public object Message { get; private set; }
        public PID Sender { get; private set; }
    }

    /// <summary>
    /// 死信消息处理器
    /// </summary>
    public class DeadLetterProcess : Process
    {
        public static readonly DeadLetterProcess Instance = new DeadLetterProcess();

        protected internal override void SendUserMessage(PID pid, object message)
        {
             EventBus.Instance.Publish(new DeadLetterEvent(pid, message, this.Sender));
        }

        protected internal override void SendSystemMessage(PID pid, object message)
        {
             EventBus.Instance.Publish(new DeadLetterEvent(pid, message, null));
        }
    }
}
