using System.Windows.Controls;
using System.Collections.Generic;

namespace Frame.OS.WPF.Regions
{
    public interface IRegionNavigationJournal
    {
        bool CanGoBack { get; }

        bool CanGoForward { get; }

        IRegionNavigationJournalEntry CurrentEntry { get; }

        INavigateAsync NavigationTarget { get; set; }

        void GoBack();

        void GoForward();

        void RecordNavigation(IRegionNavigationJournalEntry entry);

        void Clear();
    }
}
