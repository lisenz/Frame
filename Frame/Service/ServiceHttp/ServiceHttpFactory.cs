using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//----------------
using System.Xml;
using System.ServiceModel;
using System.Configuration;

namespace Frame.Service.ServiceHttp
{
    /// <summary>
    /// WCF通道工厂。
    /// </summary>
    /// <typeparam name="T">由通道工厂生成的通道类型,即契约接口。</typeparam>
    public class ServiceHttpFactory<T> : IDisposable where T : class
    {
        #region 字段

        private T _Channel;
        private const string _Key = "Service";
        private ChannelFactory<T> _Factory;

        #endregion

        #region 属性

        public T Channel
        {
            get { return this._Channel; }
            set { this._Channel = value; }
        }

        #endregion

        #region 构造函数

        public ServiceHttpFactory()
        {
            WSHttpBinding binding = new WSHttpBinding();
            EndpointAddress address = new EndpointAddress(ConfigurationManager.AppSettings[_Key]);
            this._Factory = new ChannelFactory<T>(binding, address);
            this._Channel = this._Factory.CreateChannel();
        }

        public ServiceHttpFactory(string key)
        {
            WSHttpBinding binding = new WSHttpBinding();
            EndpointAddress address = new EndpointAddress(ConfigurationManager.AppSettings[key]);
            this._Factory = new ChannelFactory<T>(binding, address);
            this._Channel = this._Factory.CreateChannel();
        }

        ~ServiceHttpFactory()
        {
            Dispose(false);
        }

        #endregion

        #region 方法

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
                    this._Factory.Close(timeSpan);
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
