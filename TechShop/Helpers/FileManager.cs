using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TechShop.Helpers
{
    public class FileManager
    {
        public static string Save(string rootPath, string folder, IFormFile file)
        {
            string filename = Guid.NewGuid().ToString() + file.FileName;
            string path = Path.Combine(rootPath, folder, filename);
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return filename;
        }
        public static bool Delete(string rootPath, string folder, string filename)
        {
            string path = Path.Combine(rootPath, folder, filename);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
                return true;
            }
            return false;
        }
    }
}
