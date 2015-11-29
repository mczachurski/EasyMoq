using System.Collections.Concurrent;

namespace SunLine.EasyMoq.Core
{
    public class Interceptor
    {
        private ConcurrentDictionary<string, object> _returnedObjects;            
        public Interceptor()
        {
            _returnedObjects = new ConcurrentDictionary<string, object>();
        }
            
        public void AddReturnedObject(string key, object value)
        {
            _returnedObjects.AddOrUpdate(key, value, (oldkey, oldvalue) => value);
        }
            
        public object GetReturnedObject(string key)
        {
            if(_returnedObjects.ContainsKey(key))
            {
                return _returnedObjects[key];
            }

            return null;
        }
    }
}