﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFilesOnDvd.Data.Entities {
    public class SeriesPublisher {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Series> Series { get; set; } = new List<Series>();
    }
}
