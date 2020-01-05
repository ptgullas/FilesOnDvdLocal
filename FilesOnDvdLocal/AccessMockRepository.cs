using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FilesOnDvdLocal.LocalDbDtos;

namespace FilesOnDvdLocal {
    public class AccessMockRepository : IDataRepository {

        private List<PerformerLocalDto> performers;

        public AccessMockRepository() {
            performers = GetMockPerformers();
        }

        public PerformerLocalDto GetPerformerByName(string performerName) {
            var performer = performers
                .FirstOrDefault(p => p.Name.ToLower() == performerName.ToLower());
            if (performer != null) {
                return performer;
            }
            else {
                return new PerformerLocalDto() {
                    Id = -1,
                    Name = performerName
                };
            }
        }

        public List<PerformerLocalDto> GetPerformers() {
            return performers.OrderBy(p => p.Name).ToList();
        }

        private List<PerformerLocalDto> GetMockPerformers() {
            List<PerformerLocalDto> mockPerformers = new List<PerformerLocalDto>() {
                new PerformerLocalDto() {Id = 1, Name = "Shiv Roy"},
                new PerformerLocalDto() {Id = 2, Name = "Roman Roy"},
                new PerformerLocalDto() {Id = 3, Name = "Kendall Roy"},
                new PerformerLocalDto() {Id = 4, Name = "Tandy Bowen"},
                new PerformerLocalDto() {Id = 5, Name = "Tyrone Johnson"},
                new PerformerLocalDto() {Id = 6, Name = "The Doctor"},
                new PerformerLocalDto() {Id = 7, Name = "Jeff Winger"},
                new PerformerLocalDto() {Id = 8, Name = "Annie Edison"},
                new PerformerLocalDto() {Id = 9, Name = "Britta Perry"},
                new PerformerLocalDto() {Id = 10, Name = "Matt Murdock"},
                new PerformerLocalDto() {Id = 11, Name = "Archie Andrews"},
                new PerformerLocalDto() {Id = 12, Name = "Toni Topaz"},
                new PerformerLocalDto() {Id = 13, Name = "Baby Yoda"},
                new PerformerLocalDto() {Id = 14, Name = "The Mandalorian"},
                new PerformerLocalDto() {Id = 15, Name = "Jon Osterman"},
                new PerformerLocalDto() {Id = 16, Name = "Laurie Blake"},
                new PerformerLocalDto() {Id = 17, Name = "Angela Abar"},
            };
            return mockPerformers;
        }
    }
}
