using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TinyService.Utils
{
    /// <summary>
    /// 单例基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonBase<T> where T : class
    {
        protected SingletonBase() { }

        private static readonly Lazy<T> _instance = new Lazy<T>(() =>
        {
            var instance = (T)Activator.CreateInstance(typeof(T), true);
            var initializable = instance as IInitializable;
            if (initializable != null)
                initializable.Initialize();

            return instance;
        });

        public static T Instance
        {
            get { return _instance.Value; }
        }
    }

    public interface IInitializable
    {
        void Initialize();
    }

   
}
