using System.Collections.Concurrent;
using System.Linq;

namespace SunLine.EasyMoq.Core
{
    public class Interceptor
    {
        private readonly ConcurrentDictionary<string, MethodInformation> _methodInformation;
        public int AmountOfAccess { get; private set; }

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
            if (_methodInformation.ContainsKey(key))
            {
                return _methodInformation[key].ReturnedObject;
            }

            return null;
        }

        public void MethodWasExecuted(string key)
        {
            AmountOfAccess++;
            if (_methodInformation.ContainsKey(key))
            {
                _methodInformation[key].MethodWasExecuted();
            }
        }

        internal MethodInformation GetMethodInformation(CallInfo callInfo)
        {
            var methods = _methodInformation.ToArray();
            foreach (var method in methods)
            {
                if (method.Value.Name == callInfo.Method.Name)
                {
                    var parameters1 = callInfo.Method.GetParameters().Select(x => x.ParameterType).ToArray();
                    var parameters2 = method.Value.Parameters;

                    if (parameters1.Length != parameters2.Length)
                    {
                        continue;
                    }

                    bool sameParameters = true;
                    for (int i = 0; i < parameters1.Length; i++)
                    {
                        if (parameters1[i] != parameters2[i])
                        {
                            sameParameters = false;
                            break;
                        }
                    }

                    if (sameParameters)
                    {
                        return method.Value;
                    }
                }
            }

            return null;
        }
    }
}