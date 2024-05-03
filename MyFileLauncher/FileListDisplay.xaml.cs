using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

namespace MyFileLauncher
{
    // INotifyPropertyChanged は変更を画面に反映するのに必要
    public partial class FileListDisplay : UserControl, INotifyPropertyChanged
    {
        private const int DisplayingNum = 20;

        // internal では画面に反映されなかったため public
        public HashSet<string> FileList { get; private set; } = new HashSet<string>();

        public event PropertyChangedEventHandler? PropertyChanged;

        internal FileListDisplay(MainWindow mainWindow)
        {
            // DisplayFileList の初期化に必要
            InitializeComponent();

            // データバインドに必要
            DataContext = this;

            mainWindow.FileListArea.Children.Add(this);
        }

        internal void Update(HashSet<string> files)
        {
            // 全件表示は時間がかかり過ぎるため一部を表示する
            FileList = Slice(files, DisplayingNum);
            NotifyPropertyChanged(nameof(FileList));
        }

        /// <summary>
        /// Slice(list.Count >= 20, 20) -> return list.ToArray()[0..20]
        /// Slice(list.Count <  20, 20) -> return list.ToArray()
        /// </summary>
        private HashSet<string> Slice(HashSet<string> list, int end)
        {
            if (list.Count < end)
            {
                return list;
            }

            string[] sliced = list.ToArray();
            return sliced[0..end].ToHashSet();
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            var e = new PropertyChangedEventArgs(propertyName);
            PropertyChanged?.Invoke(this, e);
        }
    }
}
