using Microsoft.Win32;
using SWE2_Tourplanner.Commands;
using SWE2_Tourplanner.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SWE2_Tourplanner.ViewModels
{
    public class ImportConfigViewModel:BaseViewModel, IDialogRequestClose
    {
        private string configPath;

        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;

        public string ConfigPath
        {
            get { return configPath; }
            set
            {
                if (value != configPath)
                {
                    configPath = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SelectConfigCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand ConfirmCommand { get; }

        public ImportConfigViewModel()
        {
            ExitCommand = new RelayCommand(
                (_) =>
                {
                    CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(false));
                },
                (_) => { return true; }
            );

            ConfirmCommand = new RelayCommand(
                (_) =>
                {
                    CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true));
                },
                (_) => { return !string.IsNullOrWhiteSpace(ConfigPath) && File.Exists(ConfigPath); }
            );

            SelectConfigCommand = new RelayCommand(
                (_) =>
                {
                    OpenFileDialog dialog = new OpenFileDialog();
                    dialog.Filter = "Json files (*.json)|*.json";
                    dialog.RestoreDirectory = true;
                    if (dialog.ShowDialog() ?? false)
                    {
                        ConfigPath = dialog.FileName;
                    }
                },
                (_) => { return true; }
            );
        }
    }
}
