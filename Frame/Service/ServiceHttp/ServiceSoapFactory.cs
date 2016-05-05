using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//-------------
using System.Diagnostics;
using System.ServiceModel;
using System.Configuration;
using System.CodeDom.Compiler;
using System.ServiceModel.Channels;

namespace Frame.Service.ServiceHttp
{
    /// <summary>
    /// WebService通道工厂。
    /// </summary>
    /// <typeparam name="T">由通道工厂生成的通道类型,即契约接口。</typeparam>
    public class ServiceSoapFactory<T> : IDisposable where T : class
    {
        #region 构造函数

        public ServiceSoapFactory()
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            EndpointAddress address = new EndpointAddress(ConfigurationManager.AppSettings[_Key]);
            this._Factory = new ServiceSoap<T>(binding, address);
            this._Channel = this._Factory.ChannelFactory.CreateChannel();
        }

        public ServiceSoapFactory(string key)
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            EndpointAddress address = new EndpointAddress(ConfigurationManager.AppSettings[key]);
            this._Factory = new ServiceSoap<T>(binding, address);
            this._Channel = this._Factory.ChannelFactory.CreateChannel();
        }

        #endregion

        #region 属性

        public T Channel
        {
            get { return _Channel; }
            set { _Channel = value; }
        }

        #endregion

        #region 字段

        private T _Channel = null;
        private const string _Key = "Http";
        private ClientBase<T> _Factory = null;

        #endregion

        #region 方法

        ~ServiceSoapFactory()
        {
            Dispose(false);
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isClose)
        {
            if (isClose)
            {
                Close();
            }
        }

        public void Close()
        {
            Close(TimeSpan.FromSeconds(10));
        }

        public void Close(TimeSpan timeSpan)
        {
            if (this._Factory != null)
            {
                if (this._Factory.State != CommunicationState.Opened)
                {
                    return;
                }
                try
                {
                    this._Factory.Close();
                    this._Factory = null;
                    this._Channel = null;
                }
                catch (CommunicationException)
                {
                    this._Factory.Abort();
                    this._Channel = null;
                }
                catch (TimeoutException)
                {
                    this._Factory.Abort();
                    this._Channel = null;
                }
                catch (Exception)
                {
                    this._Factory.Abort();
                    this._Channel = null;
                    throw;
                }
            }
        }

        #endregion
    }
}
