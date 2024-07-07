namespace MyFileLauncher
{
    // 画面への Notify のため internal 不可
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