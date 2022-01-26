using BlankApp.Service.Model;

namespace BlankApp.Service
{
    public interface IArchiveService
    {

        Archive Read(string archivesPath);
        bool IsArchiveDirectory(string str);
    }
}
