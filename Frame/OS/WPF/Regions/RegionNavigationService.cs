using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;

namespace Frame.OS.WPF.Regions
{
    public class RegionNavigationService : IRegionNavigationService
    {
        private readonly IServiceLocator _ServiceLocator;
        private readonly IRegionNavigationContentLoader _RegionNavigationContentLoader;
        private IRegionNavigationJournal _Journal;
        private NavigationContext _CurrentNavigationContext;

        public RegionNavigationService(IServiceLocator serviceLocator, IRegionNavigationContentLoader regionNavigationContentLoader, IRegionNavigationJournal journal)
        {
            if (serviceLocator == null)
            {
                throw new ArgumentNullException("serviceLocator");
            }

            if (regionNavigationContentLoader == null)
            {
                throw new ArgumentNullException("regionNavigationContentLoader");
            }

            if (journal == null)
            {
                throw new ArgumentNullException("journal");
            }

            this._ServiceLocator = serviceLocator;
            this._RegionNavigationContentLoader = regionNavigationContentLoader;
            this._Journal = journal;
            this._Journal.NavigationTarget = this;
        }

        #region 实现接口IRegionNavigationService

        public IRegion Region { get; set; }

        public IRegionNavigationJournal Journal
        {
            get { return this._Journal; }
        }

        public event EventHandler<RegionNavigationEventArgs> Navigating;

        public event EventHandler<RegionNavigationEventArgs> Navigated;

        public event EventHandler<RegionNavigationFailedEventArgs> NavigationFailed;

        public void RequestNavigate(Uri target, Action<NavigationResult> navigationCallback)
        {
            if (navigationCallback == null) 
                throw new ArgumentNullException("navigationCallback");

            try
            {
                this.DoNavigate(target, navigationCallback);
            }
            catch (Exception e)
            {
                this.NotifyNavigationFailed(new NavigationContext(this, target), navigationCallback, e);
            }
        }

        #endregion
        
        private void RaiseNavigating(NavigationContext navigationContext)
        {
            if (this.Navigating != null)
            {
                this.Navigating(this, new RegionNavigationEventArgs(navigationContext));
            }
        }

        private void RaiseNavigated(NavigationContext navigationContext)
        {
            if (this.Navigated != null)
            {
                this.Navigated(this, new RegionNavigationEventArgs(navigationContext));
            }
        }

        private void RaiseNavigationFailed(NavigationContext navigationContext, Exception error)
        {
            if (this.NavigationFailed != null)
            {
                this.NavigationFailed(this, new RegionNavigationFailedEventArgs(navigationContext, error));
            }
        }

        private void DoNavigate(Uri source, Action<NavigationResult> navigationCallback)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (this.Region == null)
            {
                throw new InvalidOperationException("Navigation cannot proceed until a region is set for the RegionNavigationService.");
            }

            this._CurrentNavigationContext = new NavigationContext(this, source);

            // 开始查询活动中的视图
            RequestCanNavigateFromOnCurrentlyActiveView(this._CurrentNavigationContext, navigationCallback, this.Region.ActiveViews.ToArray(), 0);
        }
        private void RequestCanNavigateFromOnCurrentlyActiveView(NavigationContext navigationContext,Action<NavigationResult> navigationCallback,object[] activeViews,int currentViewIndex)
        {
            if (currentViewIndex < activeViews.Length)
            {
                var vetoingView = activeViews[currentViewIndex] as IConfirmNavigationRequest;
                if (vetoingView != null)
                {
                    // 数据模型对当前活动视图实现IConfirmNavigationRequest,请求确认提供一个回调继续导航的要求
                    vetoingView.ConfirmNavigationRequest(
                        navigationContext,
                        canNavigate =>
                        {
                            if (this._CurrentNavigationContext == navigationContext && canNavigate)
                            {
                                RequestCanNavigateFromOnCurrentlyActiveViewModel(
                                    navigationContext,
                                    navigationCallback,
                                    activeViews,
                                    currentViewIndex);
                            }
                            else
                            {
                                this.NotifyNavigationFailed(navigationContext, navigationCallback, null);
                            }
                        });
                }
                else
                {
                    RequestCanNavigateFromOnCurrentlyActiveViewModel(
                        navigationContext,
                        navigationCallback,
                        activeViews,
                        currentViewIndex);
                }
            }
            else
            {
                ExecuteNavigation(navigationContext, activeViews, navigationCallback);
            }
        }
        private void RequestCanNavigateFromOnCurrentlyActiveViewModel(NavigationContext navigationContext,Action<NavigationResult> navigationCallback,object[] activeViews,int currentViewIndex)
        {
            var frameworkElement = activeViews[currentViewIndex] as FrameworkElement;

            if (frameworkElement != null)
            {
                var vetoingViewModel = frameworkElement.DataContext as IConfirmNavigationRequest;

                if (vetoingViewModel != null)
                {
                    // 数据模型对当前活动视图实现IConfirmNavigationRequest,请求确认提供一个回调继续导航的要求
                    vetoingViewModel.ConfirmNavigationRequest(
                        navigationContext,
                        canNavigate =>
                        {
                            if (this._CurrentNavigationContext == navigationContext && canNavigate)
                            {
                                RequestCanNavigateFromOnCurrentlyActiveView(
                                    navigationContext,
                                    navigationCallback,
                                    activeViews,
                                    currentViewIndex + 1);
                            }
                            else
                            {
                                this.NotifyNavigationFailed(navigationContext, navigationCallback, null);
                            }
                        });

                    return;
                }
            }

            RequestCanNavigateFromOnCurrentlyActiveView(
                navigationContext,
                navigationCallback,
                activeViews,
                currentViewIndex + 1);
        }
        private void ExecuteNavigation(NavigationContext navigationContext, object[] activeViews, Action<NavigationResult> navigationCallback)
        {
            try
            {
                NotifyActiveViewsNavigatingFrom(navigationContext, activeViews);

                object view = this._RegionNavigationContentLoader.LoadContent(this.Region, navigationContext);

                // 正在激活视图前提升导航事件
                this.RaiseNavigating(navigationContext);

                this.Region.Activate(view);

                // 通知其他导航之前更新导航分类
                IRegionNavigationJournalEntry journalEntry = this._ServiceLocator.GetInstance<IRegionNavigationJournalEntry>();
                journalEntry.Uri = navigationContext.Uri;
                this._Journal.RecordNavigation(journalEntry);

                // 通过视图通知导航
                InvokeOnNavigationAwareElement(view, (n) => n.OnNavigatedTo(navigationContext));

                navigationCallback(new NavigationResult(navigationContext, true));

                // 当导航完成时提示导航事件
                this.RaiseNavigated(navigationContext);
            }
            catch (Exception e)
            {
                this.NotifyNavigationFailed(navigationContext, navigationCallback, e);
            }
        }
        private static void NotifyActiveViewsNavigatingFrom(NavigationContext navigationContext, object[] activeViews)
        {
            InvokeOnNavigationAwareElements(activeViews, (n) => n.OnNavigatedFrom(navigationContext));
        }
        private static void InvokeOnNavigationAwareElements(IEnumerable<object> items, Action<INavigationAware> invocation)
        {
            foreach (var item in items)
            {
                InvokeOnNavigationAwareElement(item, invocation);
            }
        }
        private static void InvokeOnNavigationAwareElement(object item, Action<INavigationAware> invocation)
        {
            var navigationAwareItem = item as INavigationAware;
            if (navigationAwareItem != null)
            {
                invocation(navigationAwareItem);
            }

            FrameworkElement frameworkElement = item as FrameworkElement;
            if (frameworkElement != null)
            {
                INavigationAware navigationAwareDataContext = frameworkElement.DataContext as INavigationAware;
                if (navigationAwareDataContext != null)
                {
                    invocation(navigationAwareDataContext);
                }
            }
        }

        private void NotifyNavigationFailed(NavigationContext navigationContext, Action<NavigationResult> navigationCallback, Exception e)
        {
            var navigationResult =
                e != null ? new NavigationResult(navigationContext, e) : new NavigationResult(navigationContext, false);

            navigationCallback(navigationResult);
            this.RaiseNavigationFailed(navigationContext, e);
        }
    }
}
