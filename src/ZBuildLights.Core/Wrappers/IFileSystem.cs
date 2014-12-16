﻿namespace ZBuildLights.Core.Wrappers
{
    public interface IFileSystem
    {
        void WriteAllText(string path, string contents);
        string ReadAllText(string path);
        bool FileExists(string path);
    }
}