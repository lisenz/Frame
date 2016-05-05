using System;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Frame.OS.WPF.ViewModel
{
    public class ErrorsContainer<T>
    {
        private static readonly T[] _NoErrors = new T[0];
        private readonly Action<string> _RaiseErrorsChanged;
        private readonly Dictionary<string, List<T>> _ValidationResults;

        public ErrorsContainer(Action<string> raiseErrorsChanged)
        {
            if (raiseErrorsChanged == null)
            {
                throw new ArgumentNullException("raiseErrorsChanged");
            }

            this._RaiseErrorsChanged = raiseErrorsChanged;
            this._ValidationResults = new Dictionary<string, List<T>>();
        }

        public bool HasErrors
        {
            get
            {
                return this._ValidationResults.Count != 0;
            }
        }

        public IEnumerable<T> GetErrors(string propertyName)
        {
            var localPropertyName = propertyName ?? string.Empty;
            List<T> currentValidationResults = null;
            if (this._ValidationResults.TryGetValue(localPropertyName, out currentValidationResults))
            {
                return currentValidationResults;
            }
            else
            {
                return _NoErrors;
            }
        }

        public void ClearErrors<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            var propertyName = PropertySupport.ExtractPropertyName(propertyExpression);
            this.ClearErrors(propertyName);
        }

        public void ClearErrors(string propertyName)
        {
            this.SetErrors(propertyName, new List<T>());
        }

        public void SetErrors<TProperty>(Expression<Func<TProperty>> propertyExpression, IEnumerable<T> propertyErrors)
        {
            var propertyName = PropertySupport.ExtractPropertyName(propertyExpression);
            this.SetErrors(propertyName, propertyErrors);
        }

        public void SetErrors(string propertyName, IEnumerable<T> newValidationResults)
        {
            var localPropertyName = propertyName ?? string.Empty;
            var hasCurrentValidationResults = this._ValidationResults.ContainsKey(localPropertyName);
            var hasNewValidationResults = newValidationResults != null && newValidationResults.Count() > 0;

            if (hasCurrentValidationResults || hasNewValidationResults)
            {
                if (hasNewValidationResults)
                {
                    this._ValidationResults[localPropertyName] = new List<T>(newValidationResults);
                    this._RaiseErrorsChanged(localPropertyName);
                }
                else
                {
                    this._ValidationResults.Remove(localPropertyName);
                    this._RaiseErrorsChanged(localPropertyName);
                }
            }
        }
    }
}
