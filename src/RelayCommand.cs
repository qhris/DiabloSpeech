// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using System;
using System.Windows.Input;

namespace DiabloSpeech
{
    public class RelayCommand : ICommand
    {
        Action<object> action;

#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67

        public bool CanExecute(object parameter) => true;

        public RelayCommand(Action<object> action)
        {
            this.action = action;
        }

        public void Execute(object parameter) =>
            action?.Invoke(parameter);
    }
}
