using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyFileLauncher
{
    internal class MainWindowCommandMoveFocusOnFileList : MainWindowCommand
    {
        private MainWindow _mainWindow;
        private KeyEventArgs _e;

        internal MainWindowCommandMoveFocusOnFileList(MainWindow mainWindow, KeyEventArgs e)
        {
            _mainWindow = mainWindow;
            _e = e;
        }

        internal override void Execute()
        {
            MoveFocusOnFileList();

            // 処理済みにしないとフォーカス移動後に下キー押下時の既定処理が走ってしまう
            _e.Handled = true;
        }

        /// <summary>
        /// フォーカスをファイルリストに移動する
        /// </summary>
        private void MoveFocusOnFileList()
        {
            // _fileListDisplay.DisplayFileList.Focus() ではファイルリストの末尾などにフォーカス移動した
            if (_mainWindow.FileListDisplaying.FileList.Count() == 0)
            {
                return;
            }

            var obj = _mainWindow.DisplayFileList.ItemContainerGenerator.ContainerFromIndex(0);
            if (obj is ListViewItem target)
            {
                target.Focus();
            }
        }
    }
}