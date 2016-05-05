using System;
using System.IO;
using System.Collections.Generic;

namespace Frame.OS.Modularity
{
    public class FileModuleTypeLoader : IModuleTypeLoader, IDisposable
    {
        private const string RefFilePrefix = "file://";
        private readonly IAssemblyResolver assemblyResolver;
        private HashSet<Uri> downloadedUris = new HashSet<Uri>();
        public event EventHandler<LoadModuleCompletedEventArgs> LoadModuleCompleted;
        public event EventHandler<ModuleDownloadProgressChangedEventArgs> ModuleDownloadProgressChanged;
        
        public FileModuleTypeLoader()
            : this(new AssemblyResolver())
        {
        }

        public FileModuleTypeLoader(IAssemblyResolver assemblyResolver)
        {
            this.assemblyResolver = assemblyResolver;
        }

        #region 实现IModuleTypeLoader接口
        
        /// <summary>
        /// 验证模块的数据类型是否能够进行加载。
        /// </summary>
        /// <param name="moduleInfo">要进行验证的模块对象。</param>
        /// <returns>返回一个值，该值标识该模块类型是否能够进行加载。</returns>
        public bool CanLoadModuleType(ModuleInfo moduleInfo)
        {
            if (moduleInfo == null)
            {
                throw new System.ArgumentNullException("moduleInfo");
            }

            return moduleInfo.Ref != null && moduleInfo.Ref.StartsWith(RefFilePrefix, StringComparison.Ordinal);
        }

        public void LoadModuleType(ModuleInfo moduleInfo)
        {
            if (moduleInfo == null)
            {
                throw new System.ArgumentNullException("moduleInfo");
            }

            try
            {
                Uri uri = new Uri(moduleInfo.Ref, UriKind.RelativeOrAbsolute);

                if (this.IsSuccessfullyDownloaded(uri))
                {
                    this.RaiseLoadModuleCompleted(moduleInfo, null);
                }
                else
                {
                    string path = moduleInfo.Ref.Substring(RefFilePrefix.Length + 1);

                    long fileSize = -1L;
                    if (File.Exists(path))
                    {
                        FileInfo fileInfo = new FileInfo(path);
                        fileSize = fileInfo.Length;
                    }

                    this.RaiseModuleDownloadProgressChanged(moduleInfo, 0, fileSize);

                    this.assemblyResolver.LoadAssemblyFrom(moduleInfo.Ref);

                    this.RaiseModuleDownloadProgressChanged(moduleInfo, fileSize, fileSize);

                    this.RecordDownloadSuccess(uri);

                    this.RaiseLoadModuleCompleted(moduleInfo, null);
                }
            }
            catch (Exception ex)
            {
                this.RaiseLoadModuleCompleted(moduleInfo, ex);
            }
        }

        private bool IsSuccessfullyDownloaded(Uri uri)
        {
            lock (this.downloadedUris)
            {
                return this.downloadedUris.Contains(uri);
            }
        }

        private void RaiseLoadModuleCompleted(ModuleInfo moduleInfo, Exception error)
        {
            this.RaiseLoadModuleCompleted(new LoadModuleCompletedEventArgs(moduleInfo, error));
        }
        private void RaiseLoadModuleCompleted(LoadModuleCompletedEventArgs e)
        {
            if (this.LoadModuleCompleted != null)
            {
                this.LoadModuleCompleted(this, e);
            }
        }

        private void RaiseModuleDownloadProgressChanged(ModuleInfo moduleInfo, long bytesReceived, long totalBytesToReceive)
        {
            this.RaiseModuleDownloadProgressChanged(new ModuleDownloadProgressChangedEventArgs(moduleInfo, bytesReceived, totalBytesToReceive));
        }
        private void RaiseModuleDownloadProgressChanged(ModuleDownloadProgressChangedEventArgs e)
        {
            if (this.ModuleDownloadProgressChanged != null)
            {
                this.ModuleDownloadProgressChanged(this, e);
            }
        }
        
        private void RecordDownloadSuccess(Uri uri)
        {
            lock (this.downloadedUris)
            {
                this.downloadedUris.Add(uri);
            }
        }

        #endregion

        #region IDisposable接口实现

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            IDisposable disposableResolver = this.assemblyResolver as IDisposable;
            if (disposableResolver != null)
            {
                disposableResolver.Dispose();
            }
        }

        #endregion
    }
}
