using Newport.Commands;
using Newport.ViewModels;

namespace SimpleOcr2.ViewModels
{
    public class CellViewModel : ViewModelBase
    {
        private bool _isChecked;

        public CellViewModel()
        {
            TapCommand = new ActionCommand(p =>
            {
                IsChecked = true;
            });
        }

        public override string ToString()
        {
            return IsChecked.ToString();
        }

        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                SetProperty(ref _isChecked, value);
            }
        }

        public ActionCommand TapCommand { get; private set; }
    }
}