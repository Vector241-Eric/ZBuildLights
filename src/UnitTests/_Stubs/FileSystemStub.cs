using System.Collections.Generic;
using ZBuildLights.Core.Wrappers;

namespace UnitTests._Stubs
{
    public class FileSystemStub : IFileSystem
    {
        private readonly Dictionary<string, string> _allWrites = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _stubbedContent = new Dictionary<string, string>();

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
    }
}