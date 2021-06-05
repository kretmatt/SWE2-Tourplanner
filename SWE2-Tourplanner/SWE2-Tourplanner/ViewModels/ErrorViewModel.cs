using SWE2_Tourplanner.Commands;
using SWE2_Tourplanner.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SWE2_Tourplanner.ViewModels
{
    public class ErrorViewModel:BaseViewModel, IDialogRequestClose
    {
        private string errorHandlingMessage;
        private string errorType;

        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;

        public ICommand CloseCommand { get; }
        public string ErrorHandlingMessage
        {
            get { return errorHandlingMessage; }
        }

        public string ErrorType
        {
            get { return errorType; }
        }

        public ErrorViewModel(string errorHandlingMessage, string errorType)
        {
            this.errorHandlingMessage = errorHandlingMessage;
            this.errorType = errorType;
            CloseCommand = new RelayCommand(
                (_) => CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true)),
                (_) => { return true; }
            );
        }
    }
}
