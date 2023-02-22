using LegacyMediaFilesOnDvd.Data.Context;
using LegacyMediaFilesOnDvd.Data.Models;

namespace MigrateLegacy.Services {
    public class LegacyDiscService {
        private LegacyMediaFilesContext _legacyContext;
        public LegacyDiscService(LegacyMediaFilesContext legacyContext) {
            _legacyContext = legacyContext;
        }

        public List<LegacyDisc> Get() {
            var discs = _legacyContext.LegacyDiscs
                .OrderBy(discs => discs.Id)
                .ToList();
            return discs;
        }

        public List<LegacyDisc> FindByWalletId(int walletId) {
            var discs = _legacyContext.LegacyDiscs
                .Where(d => d.Wallet.Id == walletId)
                .OrderBy(discs => discs.Id)
                .ToList();
            return discs;
        }

    }
}