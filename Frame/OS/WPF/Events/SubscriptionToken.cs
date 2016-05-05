using System;

namespace Frame.OS.WPF.Events
{
    /// <summary>
    /// 订阅令牌。
    /// </summary>
    public class SubscriptionToken : IEquatable<SubscriptionToken>, IDisposable
    {

        private readonly Guid _Token;
        private Action<SubscriptionToken> _UnsubscribeAction;

        public SubscriptionToken(Action<SubscriptionToken> unsubscribeAction)
        {
            this._UnsubscribeAction = unsubscribeAction;
            this._Token = Guid.NewGuid();
        }

        public bool Equals(SubscriptionToken other)
        {
            if (other == null) return false;
            return Equals(this._Token, other._Token);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as SubscriptionToken);
        }

        public override int GetHashCode()
        {
            return this._Token.GetHashCode();
        }

        public virtual void Dispose()
        {
            if (this._UnsubscribeAction != null)
            {
                this._UnsubscribeAction(this);
                this._UnsubscribeAction = null;
            }

            GC.SuppressFinalize(this);
        }
    }
}
