using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SWE2_Tourplanner.Dialogs
{
    /// <summary>
    /// Concrete implementation of IDialogService
    /// </summary>
    public class DialogService : IDialogService
    {
        /// <summary>
        /// Owner of DialogService
        /// </summary>
        private readonly Window owner;
        /// <summary>
        /// Default constructor of DialogService
        /// </summary>
        /// <param name="owner">Owner of the DialogService instance</param>
        public DialogService(Window owner)
        {
            this.owner = owner;
            Mappings = new Dictionary<Type, Type>();
        }
        /// <summary>
        /// Registered View, ViewModel mappings
        /// </summary>
        public IDictionary<Type, Type> Mappings {get;}

        /// <summary>
        /// Registers a viewmodel together with a view
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel type to be registered</typeparam>
        /// <typeparam name="TView">View to be registered</typeparam>
        /// <exception cref="ArgumentException">Thrown, when ViewModel is already registered</exception>
        public void Register<TViewModel, TView>()
            where TViewModel : IDialogRequestClose
            where TView : IDialog
        {
            if (Mappings.ContainsKey(typeof(TViewModel)))
            {
                throw new ArgumentException($"Type {typeof(TViewModel)} is already mapped to type {typeof(TView)}");
            }
            Mappings.Add(typeof(TViewModel), typeof(TView));
        }
        /// <summary>
        /// Opens a dialog view for the registered ViewModel
        /// </summary>
        /// <typeparam name="TViewModel">Type of the ViewModel</typeparam>
        /// <param name="viewModel">ViewModel that needs the registered View to open as a Dialog</param>
        /// <returns>Result of the dialog. Can be null if the dialog gets closed</returns>
        public bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : IDialogRequestClose
        {
            Type viewType = Mappings[typeof(TViewModel)];
            IDialog dialog = (IDialog)Activator.CreateInstance(viewType);
            EventHandler<DialogCloseRequestedEventArgs> handler = null;

            handler = (sender, e) =>
            {
                viewModel.CloseRequested -= handler;
                if (e.DialogResult.HasValue)
                {
                    dialog.DialogResult = e.DialogResult;
                }
                else
                {
                    dialog.Close();
                }
            };
            viewModel.CloseRequested += handler;

            dialog.DataContext = viewModel;
            dialog.Owner = owner;

            return dialog.ShowDialog();
        }
    }
}
