using LegacyMediaFilesOnDvd.Data.Context;
using LegacyMediaFilesOnDvd.Data.Models;
using MediaFilesOnDvd.Data.Entities;
using MediaFilesOnDvd.Services;
using Microsoft.EntityFrameworkCore;

namespace MigrateLegacy.Services {
    public class LegacyDiscService {
        private LegacyMediaFilesContext _legacyContext;
        public LegacyDiscService(LegacyMediaFilesContext legacyContext) {
            _legacyContext = legacyContext;
        }

        public List<LegacyDisc> Get() {
            var discs = _legacyContext.LegacyDiscs
                .Include(d => d.Wallet)
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

        public IEnumerable<Disc> MigrateToNewDiscs(IEnumerable<LegacyDisc> legacyDiscs, WalletService newWalletService) {
            List<Disc> newDiscs = new();
            foreach (var legacyDisc in legacyDiscs) {
                var newParentWallet = newWalletService.Get(legacyDisc.Wallet.Name);
                if (newParentWallet is not null) {
                    Disc newDisc = new() {
                        Name = legacyDisc.Name,
                        Notes = legacyDisc.Notes
                    };
                    newDiscs.Add(newDisc);
                    newParentWallet.Discs.Add(newDisc);
                }
            }
            return newDiscs;
        }

    }
}