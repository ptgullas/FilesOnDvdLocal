using FilesOnDvdLocal.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOnDvdLocal {
    public class Importer {
        private IDiscRepository discRepository;
        private IPerformerRepository performerRepository;

        public Importer(IDiscRepository discRepository, IPerformerRepository performerRepository) {
            this.discRepository = discRepository;
            this.performerRepository = performerRepository;
        }

        public void Import(DvdFolderToImport dvdFolder) {
            dvdFolder.DatabaseId = discRepository.Add(dvdFolder);
            dvdFolder.SetFilesDiscId();
            // add each file to tblFilenames
            // associate performers with filenames (PerformerRepository.JoinPerformerToFile)
        }


    }
}
