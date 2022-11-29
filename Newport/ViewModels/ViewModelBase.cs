using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Newport.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        private bool _isBusy;
        private string _text;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChangedHelper(propertyName);
        }


        private void OnPropertyChangedHelper(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(storage, value))
            {
                storage = value;
                OnPropertyChangedHelper(propertyName);
            }
        }

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                SetProperty(ref _text, value, nameof(Text));
            }
        }

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                SetProperty(ref _isBusy, value, nameof(IsBusy));
            }
        }

        public BusyScope BusyScope()
        {
            return new BusyScope(this);
        }
    }
}

