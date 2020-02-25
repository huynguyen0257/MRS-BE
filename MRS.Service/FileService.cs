using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace MRS.Service
{
    public static class FileUtils
    {
        public static string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        public static Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".aac", "audio/aac"},
                {".abw", "application/x-abiword"},
                {".arc", "application/x-freearc"},
                {".avi", "video/x-msvideo"},
                {".azw", "application/vnd.amazon.ebook"},
                {".bin", "application/octet-stream"},
                {".bmp", "image/bmp"},
                {".bz", "application/x-bzip"},
                {".bz2", "application/x-bzip2"},
                {".csh", "application/x-csh"},
                {".css", "text/css"},
                {".csv", "text/csv"},
                {".doc", "application/msword"},
                {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                {".eot", "application/vnd.ms-fontobject"},
                {".epub", "application/epub+zip"},
                {".gif", "image/gif"},
                {".html", "text/html"},
                {".htm", "text/html"},
                {".ico", "image/vnd.microsoft.icon"},
                {".ics", "text/calendar"},
                {".jar", "application/java-archive"},
                {".jpeg", "image/jpeg"},
                {".jpg", "image/jpeg"},
                {".js", "text/javascript"},
                {".json", "application/json"},
                {".mid", "audio/x-midi"},
                {".midi", "audio/x-midi"},
                {".mjs", "text/javascript"},
                {".mp3", "audio/mpeg"},
                {".mpeg", "video/mpeg"},
                {".mpkg", "application/vnd.apple.installer+xml"},
                {".odp", "application/vnd.oasis.opendocument.presentation"},
                {".ods", "application/vnd.oasis.opendocument.spreadsheet"},
                {".odt", "application/vnd.oasis.opendocument.text"},
                {".oga", "audio/ogg"},
                {".ogv", "video/ogg"},
                {".ogx", "application/ogg"},
                {".otf", "font/otf"},
                {".png", "image/png"},
                {".pdf", "application/pdf"},
                {".ppt", "application/vnd.ms-powerpoint"},
                {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
                {".rar", "application/x-rar-compressed"},
                {".rtf", "application/rtf"},
                {".sh", "application/x-sh"},
                {".svg", "image/svg+xml"},
                {".swf", "application/x-shockwave-flash"},
                {".tar", "application/x-tar"},
                {".tif","image/tiff"},
                {" tiff", "image/tiff"},
                {".ttf", "font/ttf"},
                {".txt", "text/plain"},
                {".vsd", "application/vnd.visio"},
                {".wav", "audio/wav"},
                {".weba", "audio/webm"},
                {".webm", "video/webm"},
                {".webp", "image/webp"},
                {".woff", "font/woff"},
                {".woff2", "font/woff2"},
                {".xhtml", "application/xhtml+xml"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".xml", "application/xml "},
                {".zip", "application/zip"},
                {".3gp", "video/3gpp"},
                {".3g2", "video/3gpp2"},
                {".7z", "application/x-7z-compressed"}
            };
        }
    }
    public class SelectedImageVM : FileVM
    {
        public bool IsSelected { get; set; }
        public string Order { get; set; }
    }

    public class FileVM
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class FileSupport
    {
        public MemoryStream Stream { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }


    public interface IFileService
    {
        void CreateFolder(string folderName);
        string GenerateFileName(string fileName); // Random filename de khong trung
        string GetFileNameWithoutPrevious(string fileName); //Cat phan random de lay file ra
        Task<string> SaveFile(string pathFolder, IFormFile file);
        void ZipFiles(string pathFolder, string pathDestination, string fileName);
        Task<FileSupport> GetFileAsync(string pathFile);
        void DeleteFile(string pathFile);
    }

    public class FileService : IFileService
    {
        private const char CUT = '#';
        public void CreateFolder(string pathFolder)
        {
            System.IO.Directory.CreateDirectory(pathFolder);
        }

        public void DeleteFile(string pathFile)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), pathFile);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public string GenerateFileName(string fileName)
        {
            string result = new Random().Next(0, Int32.MaxValue) + DateTime.Now.ToString("dMyyyyhmmss") + CUT + fileName;
            return result;
        }

        public string GetFileNameWithoutPrevious(string fileName)
        {
            string result = fileName.Split(CUT)[fileName.Split(CUT).Length-1];
            return result;
        }

        public async Task<FileSupport> GetFileAsync(string pathFile)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), pathFile);
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return new FileSupport
            {
                Stream = memory,
                FileName = GetFileNameWithoutPrevious(Path.GetFileName(pathFile)),
                ContentType = FileUtils.GetContentType(path)
            };
        }



        public async Task<string> SaveFile(string pathFolder, IFormFile file)
        {
            string filename = GenerateFileName(file.FileName);
            string path = Path.Combine(Directory.GetCurrentDirectory(),
                               pathFolder, filename);
            try
            {
                using (var bits = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(bits);
                }
                return  Path.Combine(pathFolder,filename);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void ZipFiles(string pathFolder, string pathDestination, string fileName)
        {
            ZipFile.CreateFromDirectory(pathFolder, pathDestination + @"\" + fileName);
        }
    }
}
