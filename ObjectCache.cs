using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Rol;
using AutoMapper;

namespace RolDoc
{
    public class ObjectCache<T> where T : class
    {
        private static IDictionary<string, T> cache;

        private object CloneObject(object o)
        {
            Type t = o.GetType();
            PropertyInfo[] properties = t.GetProperties();

            Object p = t.InvokeMember("", System.Reflection.BindingFlags.CreateInstance, null, o, null);

            foreach (PropertyInfo pi in properties)
            {
                if (pi.CanWrite)
                {
                    pi.SetValue(p, pi.GetValue(o, null), null);
                }
            }
            return p;
        }
        public ObjectCache() {
            cache = new Dictionary<string, T>();
        }
        
        
        
        public void Save(T obj)
        {
            T objToSave;
            var key = ToRedisKey<T>.Impl.Value(obj);
            if (!cache.ContainsKey(key))
            {
                throw new KeyNotFoundException();
            }
            if (cache.TryGetValue(key, out objToSave))
            {
                foreach (PropertyInfo pi in obj.GetType().GetProperties())
                {
                    if (pi.CanWrite)
                    {
                        pi.SetValue(objToSave, pi.GetValue(obj, null), null);
                    }
                }
            }
        }

        public T Load(string id)
        {
            var item = DataStore.Store.Get<T>(id);
            var key = ToRedisKey<T>.Impl.Value(item);
            cache.Add(key.ToString(), item);
            var objToLoad = CloneObject(item);
            return (T)objToLoad;

        }
        public T Load(int id)
        {
            return Load(id.ToString(CultureInfo.InvariantCulture));

        }
    }
}
