using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using talk2.Models;
using talk2.Repositories;

namespace talk2.Services
{
    public interface IFileService
    {
        public int saveFile(File file);
    }

    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;

        public FileService(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public int saveFile(File file)
        {
            int fileNo = _fileRepository.GetNewFileNo();
            file.FileNo = fileNo;
            _fileRepository.saveFile(file);
            return fileNo;
        }
    }
}
