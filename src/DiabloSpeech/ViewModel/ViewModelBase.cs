// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using DiabloSpeech.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DiabloSpeech.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        #region INotifyPropertyChanged

        Dictionary<string, bool> changedProperties = new Dictionary<string, bool>();
        protected void RegisterChangedPropertyValid(string propertyName) =>
            changedProperties[propertyName] = false;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);

            // Keep track on dirty fields...
            if (changedProperties.ContainsKey(propertyName))
                changedProperties[propertyName] = true;

            // Validate data...
            ClearErrors(propertyName);
            dataValidators.ValueOrDefault(propertyName)?.Invoke(propertyName, value);
            OnPropertyChanged(nameof(IsValid));

            return true;
        }

        #endregion

        #region INotifyDataErrorInfo

        Dictionary<string, Action<string, object>> dataValidators =
            new Dictionary<string, Action<string, object>>();
        Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        protected void OnErrorsChanged(string propertyName) =>
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

        public bool IsAllPropertiesChanged => changedProperties.All(x => x.Value);
        public bool IsValid => !HasErrors && IsAllPropertiesChanged;
        public bool HasErrors => errors.Count > 0;
        public IEnumerable GetErrors(string propertyName) =>
            errors.ValueOrDefault(propertyName);

        protected void RegisterValidator(string propertyName, Action<string, object> validator) =>
            dataValidators[propertyName] = validator;

        protected void AddError(string propertyName, string error)
        {
            var errorList = errors.ValueOrDefault(propertyName) ?? new List<string>();
            errorList.Add(error);
            errors[propertyName] = errorList;
            OnErrorsChanged(propertyName);
        }

        protected void ClearErrors(string propertyName)
        {
            if (errors.ContainsKey(propertyName))
                errors.Remove(propertyName);
        }

        #endregion
    }
}
