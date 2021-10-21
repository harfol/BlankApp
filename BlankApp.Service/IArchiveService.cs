using BlankApp.Service.Model;

namespace BlankApp.Service
{
    public interface IArchiveService
    {

        Archive Read(string archivesPath);

        void BuildGroup(Article[] articles);

/*        string GenerateArchivesCoverDocument(bool cover = false);*/

        bool IsArchiveDirectory(string str);

        string[] CreateSingleDirectories(string archPath, ref Article[] articles);
        string CreateGroupDirectories(string archPath, ref Article[] articles);
        string[] CreateDirectories(string archPath, ref Article[] article);
    }
}
