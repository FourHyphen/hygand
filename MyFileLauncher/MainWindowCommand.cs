using System.Windows.Controls;
using System;

namespace MyFileLauncher
{
    internal abstract class MainWindowCommand
    {
        internal abstract void Execute();

        /// <summary>
        /// 登録されたプログラムでファイルを開く
        /// </summary>
        protected void OpenFile(string filePath)
        {
            Type? type = Type.GetTypeFromProgID("Shell.Application");
            if (type == null)
            {
                return;
            }

            // 参照に Microsoft Shell Controls And Automation を追加することで Shell32 を参照できる
            Shell32.Shell? shell = (Shell32.Shell?)Activator.CreateInstance(type!);
            if (shell == null)
            {
                return;
            }

            shell!.Open(filePath);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(shell!);
        }

        /// <summary>
        /// ディレクトリの情報で画面を更新
        /// </summary>
        protected void UpdateOfDirectoryInfo(MainWindow mainWindow, string dirPath)
        {
            // テキストボックスにディレクトリセット
            mainWindow.SearchText.Text = dirPath;

            // 検索結果には当該ディレクトリ内のファイルをセット
            // TDOO: アクセス権がなかった場合の System.UnauthorizedAccessException への対応
            mainWindow.FileListDisplaying.UpdateOfDirectory(dirPath);

            // ファイルパスが長い時に真に見たいのはファイル / ディレクトリ名のため横スクロールを右端に設定
            mainWindow.DisplayFileListScrollViewer.ScrollToRightEnd();
        }

        /// <summary>
        /// フォーカスが当てられている ListViewItem の Content を返す、何も当たっていなければ null を返す
        /// </summary>
        protected string? GetListViewItemStringFocused(MainWindow mainWindow)
        {
            ListViewItem? focused = GetListViewItemFocused(mainWindow);
            if (focused == null)
            {
                return null;
            }

            return (string)focused.Content;
        }

        /// <summary>
        /// フォーカスが当てられている ListViewItem を返す、何も当たっていなければ null を返す
        /// </summary>
        protected ListViewItem? GetListViewItemFocused(MainWindow mainWindow)
        {
            // 参考: https://threeshark3.com/binding-listbox-focus/
            for (int i = 0; i < mainWindow.DisplayFileList.Items.Count; i++)
            {
                var obj = mainWindow.DisplayFileList.ItemContainerGenerator.ContainerFromIndex(i);
                if (obj is ListViewItem target)
                {
                    if (target.IsFocused)
                    {
                        return target;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// ListView の特定の Item にフォーカスを当てる
        /// </summary>
        protected void SetFocusListViewItem(MainWindow mainWindow, string filePath)
        {
            // ListView 更新直後だと ListView の Item が空になったため、ListView に Item がセットされるようにする
            mainWindow.UpdateLayout();

            for (int i = 0; i < mainWindow.DisplayFileList.Items.Count; i++)
            {
                var obj = mainWindow.DisplayFileList.ItemContainerGenerator.ContainerFromIndex(i);
                if (obj is ListViewItem target)
                {
                    if ((string)target.Content == filePath)
                    {
                        target.Focus();
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// テキストボックスにフォーカスを当てる
        /// </summary>
        protected void SetFocusTextBox(TextBox textBox)
        {
            // Dispatcher 使わないと DisplayFileList ->  TextBox 方向にフォーカス移動できなかった
            textBox.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                textBox.Focus();
                System.Windows.Input.Keyboard.Focus(textBox);
            }), null);
        }

        /// <summary>
        /// テキストボックスの末尾にキーカーソルをセットする
        /// </summary>
        protected void SetKeyCursolEndOfTextBox(TextBox textBox)
        {
            textBox.Select(textBox.Text.Length, 0);
        }
    }
}
