using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE2_Tourplanner.Dialogs
{
    /// <summary>
    /// IDialogRequestClose defines an event for implementing classes, to ensure that the associated dialog gets closed with the according result.
    /// </summary>
    public interface IDialogRequestClose
    {
        /// <summary>
        /// Close event of the dialog. 
        /// </summary>
        event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
    }
}
