using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//------------
using System.Diagnostics;
using System.ServiceModel;
using System.CodeDom.Compiler;
using System.ServiceModel.Channels;

namespace Frame.Service.ServiceHttp
{
    [GeneratedCode("System.ServiceModel", "4.0.0.0"), DebuggerStepThrough]
    internal class ServiceSoap<TChannel> : ClientBase<TChannel> where TChannel : class
    {
        private ServiceSoap()
        {

        }

        public ServiceSoap(string endpointConfigurationName)
            : base(endpointConfigurationName)
        {
        }

        public ServiceSoap(Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        {
        }

        public ServiceSoap(string endpointConfigurationName, EndpointAddress remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        {
        }

        public ServiceSoap(string endpointConfigurationName, string remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        {
        }
    }
}
