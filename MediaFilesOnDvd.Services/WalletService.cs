using MediaFilesOnDvd.Data;
using MediaFilesOnDvd.Data.Entities;

namespace MediaFilesOnDvd.Services {
    public class WalletService {

        private MediaFilesContext _context;

        public WalletService(MediaFilesContext context) {
            _context = context;
        }

        public IEnumerable<Wallet> Get() {
            return _context.Wallets.OrderBy(w => w.Id);
        }

        /// <summary>
        /// Get Wallet by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Wallet, or null if not found</returns>
        public Wallet? Get(string name) {
            return _context.Wallets.FirstOrDefault(w => w.Name.ToLower() == name.ToLower());
        }

        public Wallet? Get(int id) {
            return _context.Wallets.FirstOrDefault(w => w.Id == id);
        }

        public bool Add(Wallet wallet) {
            var walletFromDb = Get(wallet.Name);
            if (walletFromDb == null) {
                _context.Wallets.Add(wallet);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public void AddDiscsToWallet(Wallet wallet, Disc[] discs) {
            wallet.Discs.AddRange(discs);
            _context.SaveChanges();
        }
    }
}