using MediaFilesOnDvd.Data;
using MediaFilesOnDvd.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFilesOnDvd.Services {
    public class FileTagService {
        private readonly MediaFilesContext _context;

        public FileTagService(MediaFilesContext context) {
            _context = context;
        }

        public IEnumerable<FileTag> Get() {
            return _context.FileTags.OrderBy(t => t.Name);
        }

        public FileTag? Get(string name) {
            return _context.FileTags.FirstOrDefault(t => t.Name.ToLower() == name.ToLower());
        }

        public OperationResult Add(string tagName) {
            if (string.IsNullOrWhiteSpace(tagName)) {
                return new(false, "Tag name cannot be empty.");
            }

            if (_context.FileTags.Any(t => t.Name.ToLower() == tagName.ToLower())) {
                return new(false, $"Tag '{tagName}' already exists.");
            }

            try {
                _context.FileTags.Add(new FileTag(tagName));
                _context.SaveChanges();
                return new(true);
            }
            catch (Exception ex) {
                // Log exception if possible
                return new(false, "An error occurred while adding the tag.");
            }
        }
    }
}
