﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesOnDvdLocal.Repositories {
    public interface IDiscRepository {
        int Add(DvdFolderToImport disc);
    }
}
