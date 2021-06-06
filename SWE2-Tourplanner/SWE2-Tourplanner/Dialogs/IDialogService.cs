using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE2_Tourplanner.Dialogs
{
    /// <summary>
    /// IDialogService defines the necessary methods for managing dialogs and the associated ViewModels
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Registers a ViewModel and the associated View
        /// </summary>
        /// <typeparam name="TViewModel">Type of the ViewModel</typeparam>
        /// <typeparam name="TView">Type of the View</typeparam>
        void Register<TViewModel, TView>() where TViewModel : IDialogRequestClose
                                            where TView : IDialog;
        /// <summary>
        /// Opens a dialog and returns the result of the dialog
        /// </summary>
        /// <typeparam name="TViewModel">Type of the ViewModel</typeparam>
        /// <param name="viewModel">ViewModel that needs the registered View to open</param>
        /// <returns>Result of the dialog.</returns>
        bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : IDialogRequestClose;
    }
}
