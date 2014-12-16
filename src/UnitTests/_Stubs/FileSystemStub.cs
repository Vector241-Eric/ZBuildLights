using System.Collections.Generic;
using ZBuildLights.Core.Wrappers;

namespace UnitTests._Stubs
{
    public class FileSystemStub : IFileSystem
    {
        private readonly Dictionary<string, string> _allWrites = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _stubbedContent = new Dictionary<string, string>();
        private readonly List<string> _directories = new List<string>();
        private readonly List<string> _createdDirectories = new List<string>();

        public void WriteAllText(string path, string contents)
        {
            _allWrites[path] = contents;
        }

        public string GetLastWriteTo(string filePath)
        {
            return _allWrites[filePath];
        }

        public FileSystemStub StubContentForPath(string path, string content)
        {
            _stubbedContent[path] = content;
            return this;
        }

        public string ReadAllText(string path)
        {
            return _stubbedContent[path];
        }

        public bool FileExists(string path)
        {
            return _stubbedContent.ContainsKey(path);
        }

        public bool DirectoryExists(string path)
        {
            return _directories.Contains(path);
        }

        public void CreateDirectory(string path)
        {
            _directories.Add(path);
            _createdDirectories.Add(path);
        }

        public void AssumeDirectoryExists(string path)
        {
            _directories.Add(path);
        }

        public string[] CreatedDirectories
        {
            get { return _createdDirectories.ToArray(); }
        }
    }
}