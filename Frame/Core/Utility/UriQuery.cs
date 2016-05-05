using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Frame.Core.Utility
{
    public class UriQuery : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly List<KeyValuePair<string, string>> _Entries = new List<KeyValuePair<string, string>>();

        public UriQuery()
        {
        }

        public UriQuery(string query)
        {
            if (null != query)
            {
                int num = query.Length;
                for (int i = ((num > 0 && query[0].Equals("?")) ? 1 : 0); i < num; i++)
                {
                    int start = i;
                    int flagIndex = -1;
                    while (i < num)
                    {
                        char ch = query[i];
                        if (ch.Equals("="))
                        {
                            if (flagIndex < 0)
                                flagIndex = i;
                        }
                        else if (ch.Equals("&"))
                            break;
                        i++;
                    }

                    string key = null;
                    string value = null;
                    if (flagIndex >= 0)
                    {
                        key = query.Substring(start, flagIndex - start);
                        value = query.Substring(flagIndex + 1, (i - flagIndex) - 1);
                    }
                    else
                        value = query.Substring(start, i - start);

                    this.Add(key != null ? Uri.UnescapeDataString(key) : null, Uri.UnescapeDataString(value));

                    if ((i == (num - 1)) && query[i] == '&')
                        this.Add(null, "");
                }
            }
        }

        public string this[string key]
        {
            get
            {
                foreach (KeyValuePair<string, string> kvp in this._Entries)
                {
                    if (string.Compare(kvp.Key, key, StringComparison.Ordinal) == 0)
                    {
                        return kvp.Value;
                    }
                }
                return null;
            }
        }

        #region 实现接口IEnumerable

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return this._Entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        public void Add(string key, string value)
        {
            this._Entries.Add(new KeyValuePair<string, string>(key, value));
        }

        public override string ToString()
        {
            StringBuilder queryBuilder = new StringBuilder();
            if (this._Entries.Count > 0)
            {
                queryBuilder.Append("?");
                bool isFirst = true;

                foreach (KeyValuePair<string, string> kvp in this._Entries)
                {
                    if (!isFirst)
                        queryBuilder.Append("&");
                    else
                        isFirst = false;

                    queryBuilder.Append(Uri.EscapeDataString(kvp.Key))
                        .Append("=")
                        .Append(Uri.EscapeDataString(kvp.Value));
                }
            }

            return queryBuilder.ToString();
        }

    }
}
