using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SWE2_Tourplanner.Dialogs
{
    /// <summary>
    /// IDialog only defines necessary properties / methods for windows to be opened as dialogs. 
    /// </summary>
    public interface IDialog
    {
        /// <value>
        /// DataContext of the dialog
        /// </value>
        object DataContext { get; set; }
        /// <value>
        /// Result of the dialog
        /// </value>
        bool? DialogResult { get; set; }
        /// <value>
        /// Owner of the dialog
        /// </value>
        Window Owner { get; set; }
        /// <summary>
        /// Closes the dialog
        /// </summary>
        void Close();
        /// <summary>
        /// Open a Dialog of the specific IDialog class and return result of Dialog
        /// </summary>
        /// <returns>Result of the dialog. True if successful, otherwise false. Can return null if Dialog just gets closed like any other window</returns>
        bool? ShowDialog();
    }
}
