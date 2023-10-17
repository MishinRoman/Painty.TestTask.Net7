using Microsoft.Extensions.FileProviders;

namespace Painty.TestTask.Net7.Data.Models
{
    
    public class MyImage : IFileInfo 
    {

        private readonly string _rootPath;
        private readonly FileInfo _fileInfo;
        public MyImage(IConfiguration configuration)
        {
            var FilePathFromSettigs = configuration.GetValue<string>("FilePath") ?? throw new Exception("Not found settings");
            _rootPath = Path.Combine(Directory.GetCurrentDirectory(), FilePathFromSettigs);
            _fileInfo = new FileInfo(_rootPath+"1.jpg");
        }

        

        public bool Exists => _fileInfo.Exists;

        public bool IsDirectory => false;

        public DateTimeOffset LastModified =>DateTimeOffset.Now;

        public long Length => _fileInfo.Length;

        public string Name => _fileInfo.Name;

        public string? PhysicalPath =>_fileInfo.FullName;

        public Stream CreateReadStream()
        {
            if(PhysicalPath == null)
            {
                throw new Exception("Directory not exist");
            }
            return new FileStream(PhysicalPath, FileMode.Open);
            
               
            
        }
    }
}