using MediaFilesOnDvd.Data;
using MediaFilesOnDvd.Data.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFilesOnDvd.Services {
    public class DiscService {
        private readonly MediaFilesContext _context;

        public DiscService(MediaFilesContext context) {
            _context = context;
        }

        public IEnumerable<Disc> Get() {
            return _context.Discs.OrderBy(d => d.Id);
        }

        public Disc? Get(int id) {
            return _context.Discs.FirstOrDefault(d => d.Id == id);
        }

        public Disc? Get(string name) {
            return _context.Discs.FirstOrDefault(d => d.Name.ToLower() == name.ToLower());
        }

        public IEnumerable<Disc> GetByWallet(int? walletId) {
            if (walletId is null) { 
                return Enumerable.Empty<Disc>(); 
            }
            return _context.Discs.Where(d => d.WalletId == walletId);
        }

        public IEnumerable<Disc> GetByWallet(string walletName) {
            WalletService walletService = new(_context);
            var wallet = walletService.Get(walletName);
            if (wallet is null) {
                return Enumerable.Empty<Disc>();
            }
            return GetByWallet(wallet.Id);
        }

        public OperationResult AddToWallet(Disc disc, Wallet wallet) {
            var result = _context.Discs.FirstOrDefault(d => d.Name == disc.Name);
            if (result is not null) {
                return new(false, $"Disc {disc.Name} already exists in database.");
            }
            wallet.Discs.Add(disc);
            _context.SaveChanges();
            return new(true, string.Empty);
        }

        public OperationResult AddToWallet(Disc[] discs, Wallet wallet) {
            // should we check each disc to see if it already exists?
            foreach (Disc disc in discs) {
                if (_context.Discs.Any(d => d.Name == disc.Name)) {
                    return new(false, $"disc {disc.Name} already exists in database.");
                }
            }
            wallet.Discs.AddRange(discs);
            _context.SaveChanges();
            return new(true, string.Empty);
        }

        public bool AddToWallet(Disc disc, string walletName) {
            Wallet? wallet = GetWallet(walletName);
            if (wallet is null) {
                return false;
            }
            wallet.Discs.Add(disc);
            _context.SaveChanges();
            return true;
        }

        private Wallet? GetWallet(string walletName) {
            WalletService walletService = new(_context);
            return walletService.Get(walletName);
        }

        public bool AddToWallet(Disc[] discs, string walletName) {
            Wallet? wallet = GetWallet(walletName);
            if (wallet is null) {
                return false;
            }
            wallet.Discs.AddRange(discs);
            _context.SaveChanges();
            return true;
        }

        public bool Add(Disc disc) {
            var discFromDb = Get(disc.Name);
            if (discFromDb is null) {
                _context.Discs.Add(disc);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool Add(Disc[] discs) {
            try {
                _context.Discs.AddRange(discs);
                _context.SaveChanges();
                return true;
            }
            catch (Exception e) {
                // Log.Exception(e, "Error adding discs to database);
                return false;
            }
        }

    }
}
