using System;
using System.Reflection;
using Frame.Core.Extensions;

namespace Frame.Core.Reflection.Fast
{
    public class RemoteLoader : MarshalByRefObject
    {
        private Assembly _assembly;

        public void LoadAssembly(string assemblyFile)
        {
            try
            {
                this._assembly = Assembly.LoadFrom(assemblyFile);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public object Get(string assemblyFile, string typeName, string methodName, params object[] arguments)
        {
            LoadAssembly(assemblyFile);
            if (this._assembly == null)
                return null;
            Type type = this._assembly.GetType(typeName);

            if (type == null)
                return null;

            object obj = Activator.CreateInstance(type);
            if (obj == null)
                return null;

            MethodInfo method = obj.GetType().GetMethod(methodName);
            if (method == null)
                return null;
            return method.FastInvoke(obj, arguments);
        }

        public T GetInstance<T>(string assemblyFile, string typeName, string methodName, params object[] arguments)
        {
            object result = Get(assemblyFile, typeName, methodName, arguments);
            if (result == null)
                return default(T);
            return (T)result;
        }
    }
}
