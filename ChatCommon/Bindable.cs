using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ChatCommon
{
    public abstract class Bindable : INotifyDataErrorInfo, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

        public bool HasErrors
        {
            get { return errors.Count > 0; }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return errors.Values;
            }

            if (!errors.ContainsKey(propertyName))
            {
                return Enumerable.Empty<string>();
            }

            return errors[propertyName];
        }

        public void AddError(string error, [CallerMemberName] string propertyName = "")
        {
            RemoveErrors(propertyName);
            AddErrors(new List<string> { error }, propertyName);
        }

        public void AddErrors(List<string> list, [CallerMemberName] string propertyName = "")
        {
            errors[propertyName] = list;
            OnErrorsChanged(propertyName);
        }

        public void RemoveErrors([CallerMemberName] string propertyName = "")
        {
            errors.Remove(propertyName);
            OnErrorsChanged(propertyName);
        }

        protected void SetValue<T>(ref T property, T value, [CallerMemberName] string propertyName = "")
        {
            if (Equals(property, value))
            {
                return;
            }

            property = value;
            OnPropertyChanged(propertyName);
        }

        private void OnErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
            {
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
