using System;

namespace Frame.OS
{
     [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ViewSortHintAttribute : Attribute
    {
        public ViewSortHintAttribute(string hint)
        {
            this.Hint = hint;
        }

        public string Hint { get; private set; }
    }
}
