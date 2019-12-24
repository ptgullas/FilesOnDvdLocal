using FilesOnDvdLocal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DvdImportClient {
    public class FolderToImportViewModel : INotifyPropertyChanged {
        private DvdFolderToImport folderToImport;
        public string FolderPath {
            get { return folderToImport.FolderPath; } 
            set {
                folderToImport = new DvdFolderToImport(FolderPath);
                folderToImport.GetFiles();
                OnPropertyChange("Files");
            } 
        }

        public List<FileToImport> Files {
            get { return folderToImport.Files; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChange(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
