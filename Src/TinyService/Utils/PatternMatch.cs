using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TinyService.Utils
{
    public static class PatternMatch
    {

         
        public static Case Match(this object target)
        {
            return new Case(target);
        }

       
        public static Case<T> Match<T>(this object target)
        {
            return new Case<T>(target);
        }
    }

 
    public interface IMatchResult
    {
      
        bool WasHandled { get; }
    }

  
    public class Case : IMatchResult
    {
        
        private readonly object _message;
        
        private bool _handled;
 
        public bool WasHandled { get { return _handled; } }

      
        public Case(object message)
        {
            _message = message;
        }

       
        public Case With<TMessage>(Action action)
        {
            if (!_handled && _message is TMessage)
            {
                action();
                _handled = true;
            }

            return this;
        }

         
        public Case With<TMessage>(Action<TMessage> action)
        {
            if (!_handled && _message is TMessage)
            {
                action((TMessage)_message);
                _handled = true;
            }

            return this;
        }

         
        public IMatchResult Default(Action<object> action)
        {
            if (!_handled)
            {
                action(_message);
                _handled = true;
            }
            return AlwaysHandled.Instance;
        }
 
        private class AlwaysHandled : IMatchResult
        {
            
            public static readonly AlwaysHandled Instance = new AlwaysHandled();
         
            private AlwaysHandled() { }
         
            public bool WasHandled { get { return true; } }
        }
    }

     
    public class Case<T> : IMatchResult
    {
        
        private readonly object _message;
       
        private bool _handled;

      
        private T _result;
 
        public bool WasHandled { get { return _handled; } }

         
        public Case(object message)
        {
            _message = message;
        }

         
        public Case<T> With<TMessage>(Func<T> function)
        {
            if (!_handled && _message is TMessage)
            {
                _result = function();
                _handled = true;
            }

            return this;
        }
 
        public Case<T> With<TMessage>(Func<TMessage, T> function)
        {
            if (!_handled && _message is TMessage)
            {
                _result = function((TMessage)_message);
                _handled = true;
            }

            return this;
        }

       
        public T ResultOrDefault(Func<object, T> function)
        {
            if (!_handled)
            {
                _result = function(_message);
                _handled = true;
            }

            return _result;
        }
    }
}
