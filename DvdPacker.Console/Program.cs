using FilesOnDvdLocal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DvdPacker {
    class Program
    {
        static void Main(string[] args) {
            string folderPath = args[0];
            BinPacker binPacker = new BinPacker(folderPath, "P");
            binPacker.ProcessFolder();
            string binListing = binPacker.GetBinListing();
            Console.WriteLine(binListing);
        }
    }
}
