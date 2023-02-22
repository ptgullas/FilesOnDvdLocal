using LegacyMediaFilesOnDvd.Data.Context;
using LegacyMediaFilesOnDvd.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MigrateLegacy.Services {
    public class LegacyWalletService {
        private LegacyMediaFilesContext _legacyContext;

        public LegacyWalletService(LegacyMediaFilesContext legacyContext) {
            _legacyContext = legacyContext;
        }

        public IEnumerable<LegacyWallet> Get() {
            return _legacyContext.LegacyWallets
                .OrderBy(lw => lw.Id);
        }
    }
}
