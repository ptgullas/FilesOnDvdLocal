﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFilesOnDvd.Data.Entities {
    public class Performer {
        public int Id { get; set; }
        public string Name { get; set; }
        // comment this out until ready
        // public PerformerType Type { get; set; }

        // Aliases should probably be in its own table, if we're going to query on it
        // public List<string> Aliases { get; set; } = new List<string>();
        public List<ScreenshotUrl> ImageUrls { get; set; } = new List<ScreenshotUrl>();
        // comment this out until I'm ready
        public virtual ICollection<MediaFile>? MediaFiles { get; set; } = new List<MediaFile>();
    }
}
