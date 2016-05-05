using System;
using System.Collections.Generic;

namespace Frame.OS.WPF.Regions
{
    public class RegionNavigationJournal : IRegionNavigationJournal
    {
        private Stack<IRegionNavigationJournalEntry> _BackStack = new Stack<IRegionNavigationJournalEntry>();
        private Stack<IRegionNavigationJournalEntry> _ForwardStack = new Stack<IRegionNavigationJournalEntry>();
        private bool _IsNavigatingInternal;

        #region 实现接口IRegionNavigationJournal

        public bool CanGoBack
        {
            get { return this._BackStack.Count > 0; }
        }

        public bool CanGoForward
        {
            get { return this._ForwardStack.Count > 0; }
        }

        public INavigateAsync NavigationTarget { get; set; }

        public IRegionNavigationJournalEntry CurrentEntry { get; private set; }

        public void GoBack()
        {
            if (this.CanGoBack)
            {
                IRegionNavigationJournalEntry entry = this._BackStack.Peek();
                this.InternalNavigate(
                    entry,
                    result =>
                    {
                        if (result)
                        {
                            if (this.CurrentEntry != null)
                            {
                                this._ForwardStack.Push(this.CurrentEntry);
                            }

                            this._BackStack.Pop();
                            this.CurrentEntry = entry;
                        }
                    });
            }
        }

        public void GoForward()
        {
            if (this.CanGoForward)
            {
                IRegionNavigationJournalEntry entry = this._ForwardStack.Peek();
                this.InternalNavigate(
                    entry,
                    result =>
                    {
                        if (result)
                        {
                            if (this.CurrentEntry != null)
                            {
                                this._BackStack.Push(this.CurrentEntry);
                            }

                            this._ForwardStack.Pop();
                            this.CurrentEntry = entry;
                        }
                    });
            }
        }

        public void RecordNavigation(IRegionNavigationJournalEntry entry)
        {
            if (!this._IsNavigatingInternal)
            {
                if (this.CurrentEntry != null)
                {
                    this._BackStack.Push(this.CurrentEntry);
                }

                this._ForwardStack.Clear();
                this.CurrentEntry = entry;
            }
        }

        public void Clear()
        {
            this.CurrentEntry = null;
            this._BackStack.Clear();
            this._ForwardStack.Clear();
        }

        #endregion

        private void InternalNavigate(IRegionNavigationJournalEntry entry, Action<bool> callback)
        {
            this._IsNavigatingInternal = true;
            this.NavigationTarget.RequestNavigate(
                entry.Uri,
                nr =>
                {
                    this._IsNavigatingInternal = false;

                    if (nr.Result.HasValue)
                    {
                        callback(nr.Result.Value);
                    }
                });
        }
    }
}
