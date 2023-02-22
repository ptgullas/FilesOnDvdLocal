using LegacyMediaFilesOnDvd.Data.Context;
using LegacyMediaFilesOnDvd.Data.Models;
using Microsoft.EntityFrameworkCore;
using MediaFilesOnDvd.Data.Entities;

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

        public static IEnumerable<Wallet> MigrateToNewWallets(IEnumerable<LegacyWallet> legacyWallets) {
            List<Wallet> newWallets = new();
            foreach (var legacyWallet in legacyWallets) {
                Wallet wallet = new() {
                    Name = legacyWallet.Name,
                };
                newWallets.Add(wallet);
            }
            return newWallets;
        }
    }
}
