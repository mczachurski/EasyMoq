using System.Collections.Concurrent;

namespace SunLine.EasyMoq.Core
{
    public class Interceptor
    {
        private ConcurrentDictionary<string, MethodInformation> _methodInformation;            
        public Interceptor()
        {
            _methodInformation = new ConcurrentDictionary<string, MethodInformation>();
        }
            
        public void AddMethodInformation(MethodInformation methodInformation)
        {
            _methodInformation.AddOrUpdate(methodInformation.Hash, methodInformation, (oldkey, oldvalue) => methodInformation);
        }
            
        public object GetReturnObject(string key)
        {
            if(_methodInformation.ContainsKey(key))
            {
                return _methodInformation[key].ReturnedObject;
            }

            return null;
        }
        
        public void MethodWasExecuted(string key)
        {
            if(_methodInformation.ContainsKey(key))
            {
                _methodInformation[key].MethodWasExecuted();
            }
        }
        
        public MethodInformation GetMethodInformation(CallInfo callInfo)
        {
            return null;
        }
    }
}