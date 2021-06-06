using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SWE2_Tourplanner.Dialogs;

namespace SWE2_Tourplanner
{
    /// <summary>
    /// Interaction logic for ManeuverManagementWindow
    /// </summary>
    public partial class ManeuverManagementWindow : Window, IDialog
    {
        public ManeuverManagementWindow()
        {
            InitializeComponent();
        }
    }
}
