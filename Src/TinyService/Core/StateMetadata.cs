using System;
using System.Collections.Generic;
using System.Text;

namespace TinyService.Core
{
    public sealed class StateMetadata
    {
        public StateMetadata(object value, Type type, StateChangeAction changeAction)
        {
            this.Value = value;
            this.Type = type;
            this.ChangeAction = changeAction;
        }

        public object Value { get; set; }

        public StateChangeAction ChangeAction { get; set; }

        public Type Type { get; }

        public static StateMetadata Create<T>(T value, StateChangeAction changeKind)
        {
            return new StateMetadata(value, typeof(T), changeKind);
        }

        public static StateMetadata CreateForRemove()
        {
            return new StateMetadata(null, typeof(object), StateChangeAction.Remove);
        }
    }

    public enum  StateChangeAction
    {
        None=0,
        Add=2,
        Update=4,
        Remove=8
    }
}
