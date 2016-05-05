using System;

namespace Frame.OS.WPF.Regions
{
    public class RegionNavigationJournalEntry : IRegionNavigationJournalEntry
    {
        public Uri Uri { get; set; }

        public override string ToString()
        {
            if (this.Uri != null)
            {
                return string.Format("RegionNavigationJournalEntry:'{0}'", this.Uri.ToString());
            }

            return base.ToString();
        }
    }
}
