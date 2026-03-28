using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFilesOnDvd.Services {
    public class PathResolverService {
        private readonly IConfiguration _config;

        public PathResolverService(IConfiguration config) {
            _config = config;
        }

        public string GetPerformerHeadshotPath(string filename) {
            var basePath = _config["ImagePaths:PerformerHeadshots"] ?? "";
            return Path.Combine(basePath, filename);
        }

        public string GetMediaScreenshotPath(string filename) {
            var basePath = _config["ImagePaths:MediaScreenshots"] ?? "";
            return Path.Combine(basePath, filename);
        }

        public string GetPerformerGalleryPath(string filename) {
            var basePath = _config["ImagePaths:PerformerGallery"] ?? "";
            return Path.Combine(basePath, filename);
        }

        public string GetImageAsBase64(string physicalPath) {
            if (string.IsNullOrEmpty(physicalPath) || !File.Exists(physicalPath)) {
                return "";
            }

            try {
                byte[] imageBytes = File.ReadAllBytes(physicalPath);
                string extension = Path.GetExtension(physicalPath).ToLower().TrimStart('.');
                // Default to png if we can't determine extension
                string mimeType = extension switch {
                    "jpg" or "jpeg" => "image/jpeg",
                    "png" => "image/png",
                    "gif" => "image/gif",
                    "webp" => "image/webp",
                    _ => "image/png"
                };
                return $"data:{mimeType};base64,{Convert.ToBase64String(imageBytes)}";
            }
            catch {
                return "";
            }
        }
    }
}
