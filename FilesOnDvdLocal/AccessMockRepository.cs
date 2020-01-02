using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FilesOnDvdLocal.LocalDbDtos;

namespace FilesOnDvdLocal {
    public class AccessMockRepository : IDataRepository {
        public PerformerLocalDto GetPerformerByName(string performerName) {
            if (performerName.Last().ToString().ToLower() == "i") {
                return new PerformerLocalDto() {
                    Id = 9999,
                    Name = performerName
                };
            }
            else {
                return new PerformerLocalDto() {
                    Id = -1,
                    Name = performerName
                };
            }
        }

        public List<PerformerLocalDto> GetPerformers() {
            throw new NotImplementedException();
        }
    }
}
