using System;
using System.Windows.Input;

namespace Newport.Commands
{

        public class GenericActionCommand<T> : ICommand
        {
            public GenericActionCommand()
            {
            }

            public GenericActionCommand(Action<T> action)
            {
                Action = action;
            }

            public GenericActionCommand(Action<T> action, Func<T, bool> isEnabled)
            {
                Action = action;
                IsEnabled = isEnabled;
            }

            public event EventHandler CanExecuteChanged;


            public bool CanExecute(object parameter)
            {
                return (IsEnabled == null) || IsEnabled((T)parameter);
            }

            public void Execute()
            {
                Execute(default(T));
            }

            public void Execute(object parameter)
            {
                if (Action != null)
                {
                    Action((T)parameter);
                }
            }

            public Action<T> Action { get; set; }

            public Func<T, bool> IsEnabled { get; set; }

            public void Trigger()
            {
                var handler = CanExecuteChanged;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        public class ActionCommand : GenericActionCommand<object>
        {
            public ActionCommand()
            {
            }

            public ActionCommand(Action<object> action)
              : base(action)
            {
            }

            public ActionCommand(Action<object> action, Func<object, bool> isEnabled)
              : base(action, isEnabled)
            {
            }
        }
    }

