using SWE2_Tourplanner.Dialogs;
using SWE2_Tourplanner.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SWE2_Tourplanner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// OnStartup registers ViewModels and their associated Views. It also opens a new MainWindow instance
        /// </summary>
        /// <param name="e">EventArgs related to the startup of the application</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            IDialogService dialogService = new DialogService(MainWindow);
            dialogService.Register<CreateUpdateTourLogViewModel, TourLogCreateUpdateView>();
            dialogService.Register<ImportViewModel,ImportView>();
            dialogService.Register<CreateUpdateTourViewModel, CreateUpdateTourView>();
            dialogService.Register<ErrorViewModel, ErrorView>();
            dialogService.Register<ManeuversViewModel, ManeuverManagementWindow>();
            dialogService.Register<ImportConfigViewModel, ImportConfigView>();

            var viewModel = new MainViewModel(dialogService);
            var view = new MainWindow { DataContext = viewModel };
            view.ShowDialog();
        }
    }
}
