using System;
using System.Collections.Generic;

namespace Frame.Core.Extensions
{
    public static class ExceptionExtensions
    {
        private static List<Type> frameworkExceptionTypes = new List<Type>();

        public static void RegisterFrameworkExceptionType(Type frameworkExceptionType)
        {
            if (frameworkExceptionType == null) throw new ArgumentNullException("frameworkExceptionType");

            if (!frameworkExceptionTypes.Contains(frameworkExceptionType))
                frameworkExceptionTypes.Add(frameworkExceptionType);
        }

        public static bool IsFrameworkExceptionRegistered(Type frameworkExceptionType)
        {
            return frameworkExceptionTypes.Contains(frameworkExceptionType);
        }

        public static Exception GetRootException(this Exception exception)
        {
            Exception rootException = exception;

            try
            {
                while (true)
                {
                    if (rootException == null)
                    {
                        rootException = exception;
                        break;
                    }

                    if (!IsFrameworkException(rootException))
                    {
                        break;
                    }
                    rootException = rootException.InnerException;
                }
            }
            catch (Exception)
            {
                rootException = exception;
            }
            return rootException;
        }

        private static bool IsFrameworkException(Exception exception)
        {
            bool isFrameworkException = frameworkExceptionTypes.Contains(exception.GetType());
            bool childIsFrameworkException = false;

            if (exception.InnerException != null)
            {
                childIsFrameworkException = frameworkExceptionTypes.Contains(exception.InnerException.GetType());
            }

            return isFrameworkException || childIsFrameworkException;
        }
    }
}
