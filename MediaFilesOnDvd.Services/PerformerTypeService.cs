using MediaFilesOnDvd.Data;
using MediaFilesOnDvd.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaFilesOnDvd.Services {
    public class PerformerTypeService {
        private readonly MediaFilesContext _context;

        public PerformerTypeService(MediaFilesContext context) {
            _context = context;
        }

        public IEnumerable<PerformerType> Get() {
            return _context.PerformerTypes.OrderBy(pt => pt.Name);
        }

        public OperationResult Add(PerformerType performerType) {
            if (_context.PerformerTypes.Any(pt => pt.Name.ToLower() == performerType.Name.ToLower())) {
                return new(false, $"PerformerType {performerType.Name} already exists in database");
            }
            try {
                _context.PerformerTypes.Add(performerType);
                _context.SaveChanges();
                return new(true);
            }
            catch (Exception e) {
                return new(false, "Error adding PerformerType to database");
            }
        }
    }
}
