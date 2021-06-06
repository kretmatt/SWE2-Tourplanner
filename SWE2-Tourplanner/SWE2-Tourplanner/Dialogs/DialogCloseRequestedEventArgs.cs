using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE2_Tourplanner.Dialogs
{
    /// <summary>
    /// DialogCloseRequestedEventArgs is an EventArgs used for abstracting / closing Dialogs
    /// </summary>
    public class DialogCloseRequestedEventArgs:EventArgs
    {
        /// <summary>
        /// Default constructor of DialogCloseRequestedEventArgs
        /// </summary>
        /// <param name="dialogResult">Result of the dialog: true if user clicks OK-, commit-buttons etc., false if user exits the dialog</param>
        public DialogCloseRequestedEventArgs(bool? dialogResult)
        {
            DialogResult = dialogResult;
        }
        /// <summary>
        /// Result of the dialog. Can be null if used just closes the dialog
        /// </summary>
        public bool? DialogResult { get; }
    }
}
