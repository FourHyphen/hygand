using System.ComponentModel;

namespace MyFileLauncher
{
    public class FileDisplaying
    {
        public string FilePath { get; } = string.Empty;

        public bool IsSelected { get; } = false;

        public FileDisplaying(string filePath, bool isSelected)
        {
            FilePath = filePath;
            IsSelected = isSelected;
        }
    }
}