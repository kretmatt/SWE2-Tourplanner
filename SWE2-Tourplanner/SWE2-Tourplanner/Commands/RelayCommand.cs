using System;
using System.Windows.Input;

namespace SWE2_Tourplanner.Commands
{
    /// <summary>
    /// RelayCommand is a concrete implementation of ICommand and allows us to create new commands, without having to create new concrete classes.
    /// </summary>
    public class RelayCommand:ICommand
    {
        /// <summary>
        /// Action that gets executed by the command. Can be anonymous.
        /// </summary>
        private Action<object> _execute;
        /// <summary>
        /// Condition, whether the command can be executed or not.
        /// </summary>
        private Predicate<object> _canExecute;
        
        /// <summary>
        /// EventHandler that manages Requery if CanExecute changes
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        /// <summary>
        /// Default constructor of RelayCommand.
        /// </summary>
        /// <param name="execute">Action of the command</param>
        /// <param name="canExecute">Condition, whether a command can be executed or not.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute=null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }
        /// <summary>
        /// Checks if command can be executed or not.
        /// </summary>
        /// <param name="parameter">Parameter for the condition</param>
        /// <returns>True, if _canExecute is null or if condition is fulfilled, otherwise false</returns>
        public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;
        /// <summary>
        /// Executes the Action of the command.
        /// </summary>
        /// <param name="parameter">Parameter for the action</param>
        public virtual void Execute(object parameter)=>_execute.Invoke(parameter);
    }
}
