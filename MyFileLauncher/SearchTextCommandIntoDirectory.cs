using System.Windows.Controls;

namespace MyFileLauncher
{
    internal class SearchTextCommandIntoDirectory : MainWindowCommand
    {
        private MainWindow _mainWindow;

        internal SearchTextCommandIntoDirectory(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        /// <summary>
        /// ディレクトリの中に入る要求で、かつ前方一致検索の結果 1 件だけの場合、テキストボックス記載ディレクトリに入る
        /// </summary>
        internal override void Execute()
        {
            // 1 件のみでない場合は入るディレクトリを特定できない
            if (_mainWindow.DisplayFileList.Items.Count != 1)
            {
                return;
            }

            // DisplayFileList 先頭がディレクトリでない場合は何もしない
            string content = GetContentFromTopOfDisplayFileList();
            if (!System.IO.Directory.Exists(content))
            {
                return;
            }

            // DisplayFileList 先頭のディレクトリの情報で DisplayFileList を更新する
            UpdateOfDirectoryInfo(_mainWindow, content);

            // フォーカスをテキストボックスに当てる
            SetFocusTextBox(_mainWindow.SearchText);

            // キーカーソル位置をテキストボックスの末尾文字に移動することで引き続きディレクトリパスを入力可能にする
            SetKeyCursolEndOfTextBox(_mainWindow.SearchText);
        }

        /// <summary>
        /// DisplayFileList の先頭の要素の Content を返す
        /// 先頭が ListViewItem でない場合は string.Empty を返す
        /// </summary>
        private string GetContentFromTopOfDisplayFileList()
        {
            var obj = _mainWindow.DisplayFileList.ItemContainerGenerator.ContainerFromIndex(0);
            if (obj is ListViewItem target)
            {
                return (string)target.Content;
            }

            return string.Empty;
        }
    }
}