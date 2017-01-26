using System;

namespace Softstar
{
    [Serializable]
    public class FileHash
    {
        public string Path;
        public string SHA1;
        public int Length;
    }

    [Serializable]
    public class FileHashList
    {
        public FileHash[] List;
    }
}
