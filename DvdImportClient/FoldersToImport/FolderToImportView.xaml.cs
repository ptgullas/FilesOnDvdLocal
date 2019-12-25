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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DvdImportClient.FoldersToImport {
    /// <summary>
    /// Interaction logic for FolderToImportView.xaml
    /// </summary>
    public partial class FolderToImportView : UserControl {
        public FolderToImportView() {
            InitializeComponent();
            DataContext = new FolderToImportViewModel();

        }
    }
}
