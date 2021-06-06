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
    /// <summary>
    /// ErrorViewModel is used for informing the user about exceptions / errors that occured during execution of other methods
    /// </summary>
    public class ErrorViewModel:BaseViewModel, IDialogRequestClose
    {
        /// <summary>
        /// Exception / Error message
        /// </summary>
        private string errorHandlingMessage;
        /// <summary>
        /// Type of error / exception
        /// </summary>
        private string errorType;
        /// <summary>
        /// Event used for closing the dialog
        /// </summary>
        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
        /// <value>
        /// Command used for closing the dialog
        /// </value>
        public ICommand CloseCommand { get; }
        /// <value>
        /// Exception / Error message
        /// </value>
        public string ErrorHandlingMessage
        {
            get { return errorHandlingMessage; }
        }
        /// <value>
        /// Exception / Error type
        /// </value>
        public string ErrorType
        {
            get { return errorType; }
        }
        /// <summary>
        /// Default constructor of ErrorViewModel
        /// </summary>
        /// <param name="errorHandlingMessage">Error message</param>
        /// <param name="errorType">Error type</param>
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
