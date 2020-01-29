﻿using FilesOnDvdLocal.LocalDbDtos;
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
        private IFileRepository fileRepository;

        public Importer(IDiscRepository discRepository, 
            IPerformerRepository performerRepository,
            IFileRepository fileRepository
            ) {
            this.discRepository = discRepository;
            this.performerRepository = performerRepository;
            this.fileRepository = fileRepository;
        }

        public void Import(DvdFolderToImport dvdFolder) {
            dvdFolder.DatabaseId = discRepository.Add(dvdFolder);
            dvdFolder.SetFilesDiscId();
            AddFilesToDatabase(dvdFolder);
            var fileDtosFromDatabase = fileRepository.GetByDisc((int) dvdFolder.DatabaseId);
            dvdFolder.SetFilesIdsFromDatabase(fileDtosFromDatabase);
            // associate performers with filenames (PerformerRepository.JoinPerformerToFile)
        }

        private void AddFilesToDatabase(DvdFolderToImport dvdFolder) {
            if (!dvdFolder.IsReadyToImport) { // throw a NotReadyToImportException or something 
            }
            fileRepository.Add(dvdFolder.Files);
        }
    }
}
