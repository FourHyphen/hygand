using System.ComponentModel;

namespace MyFileLauncher
{
    public class FileDisplaying : INotifyPropertyChanged
    {
        // コレクションの要素を動的にバインド通知する場合、要素の方に PropertyChanged を実装する必要あり
        public event PropertyChangedEventHandler? PropertyChanged;

        public string FilePath { get; } = string.Empty;

        private bool _isSelected = false;

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                NotifyPropertyChanged(nameof(IsSelected));
            }
        }

        /// <summary>
        /// プロパティ更新を通知
        /// </summary>
        private void NotifyPropertyChanged(string propertyName)
        {
            var e = new PropertyChangedEventArgs(propertyName);
            PropertyChanged?.Invoke(this, e);
        }

        public FileDisplaying(string filePath)
        {
            FilePath = filePath;
            IsSelected = false;
        }

        public FileDisplaying(string filePath, bool isSelected)
        {
            FilePath = filePath;
            IsSelected = isSelected;
        }
    }
}