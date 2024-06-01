using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyFileLauncher
{
    internal class MainWindowProcessKeyDowned
    {
        private MainWindow _mainWindow;

        private History _history;

        internal MainWindowProcessKeyDowned(MainWindow mainWindow, History history)
        {
            _mainWindow = mainWindow;
            _history = history;
        }

        /// <summary>
        /// DisplayFileList 押下キーに応じた処理を実行する
        /// </summary>
        internal void Execute(Key key, Key systemKey, ModifierKeys modifier)
        {
            AppKeys.KeyEventType keyEventType = AppKeys.ToKeyEventType(key, systemKey, modifier);
            if (keyEventType == AppKeys.KeyEventType.FileOpen)
            {
                DoKeyEventFileOpen();
            }
            else if (keyEventType == AppKeys.KeyEventType.BackDirectory)
            {
                DoKeyEventBackDirectory();
            }
            else if (keyEventType == AppKeys.KeyEventType.IntoDirectory)
            {
                DoKeyEventIntoDirectory();
            }
            else if (keyEventType == AppKeys.KeyEventType.FocusOnSearchTextBox)
            {
                DoKeyEventFocusOnSearchTextBox();
            }
            else if (keyEventType == AppKeys.KeyEventType.ShowPrograms)
            {
                DoKeyEventShowPrograms();
            }
        }

        /// <summary>
        /// キーイベントによるファイルオープンを実行
        /// </summary>
        private void DoKeyEventFileOpen()
        {
            // フォーカスの当たっているファイルパスを取得
            string? focusedFilePath = GetListViewItemStringFocused();
            if (focusedFilePath == null)
            {
                return;
            }

            // ファイルを開く
            OpenFile(focusedFilePath);

            // 履歴に追加
            _history.Add(focusedFilePath);

            // ファイルを開いたら用は済んだのでメインウィンドウを非表示化
            _mainWindow.Hide();
        }

        /// <summary>
        /// フォーカスが当てられている ListViewItem の Content を返す、何も当たっていなければ null を返す
        /// </summary>
        private string? GetListViewItemStringFocused()
        {
            ListViewItem? focused = GetListViewItemFocused();
            if (focused == null)
            {
                return null;
            }

            return (string)focused.Content;
        }

        /// <summary>
        /// フォーカスが当てられている ListViewItem を返す、何も当たっていなければ null を返す
        /// </summary>
        private ListViewItem? GetListViewItemFocused()
        {
            // 参考: https://threeshark3.com/binding-listbox-focus/
            for (int i = 0; i < _mainWindow.DisplayFileList.Items.Count; i++)
            {
                var obj = _mainWindow.DisplayFileList.ItemContainerGenerator.ContainerFromIndex(i);
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
        /// 登録されたプログラムでファイルを開く
        /// </summary>
        private void OpenFile(string filePath)
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
        /// キーイベントによるディレクトリ階層を 1 つ戻る処理を実行
        /// </summary>
        private void DoKeyEventBackDirectory()
        {
            // フォーカスされているファイルパスを取得
            string? focusedFilePath = GetListViewItemStringFocused();
            if (focusedFilePath == null)
            {
                return;
            }

            // 戻り先ディレクトリパス取得
            string? dirPath = GetBackDirectoryPath(focusedFilePath, _mainWindow.SearchText.Text);
            if (dirPath == null)
            {
                return;
            }

            // 更新
            UpdateOfDirectoryInfo(dirPath);
        }

        /// <summary>
        /// 1 階層戻る先のディレクトリパスを返す
        /// </summary>
        private string? GetBackDirectoryPath(string focusedFilePath, string searchText)
        {
            // 今のテキストボックスがディレクトリのパスであればこれの 1 階層上を返す
            if (System.IO.Directory.Exists(searchText))
            {
                return System.IO.Path.GetDirectoryName(searchText)!;
            }

            // 選択されているファイルのディレクトリパスを返す
            return System.IO.Path.GetDirectoryName(focusedFilePath)!;
        }

        /// <summary>
        /// ディレクトリの情報で画面を更新
        /// </summary>
        private void UpdateOfDirectoryInfo(string dirPath)
        {
            // テキストボックスにディレクトリセット
            _mainWindow.SearchText.Text = dirPath;

            // 検索結果には当該ディレクトリ内のファイルをセット
            _mainWindow.FileListDisplaying.Update(dirPath);

            // ファイルパスが長い時に真に見たいのはファイル / ディレクトリ名のため横スクロールを右端に設定
            _mainWindow.DisplayFileListScrollViewer.ScrollToRightEnd();
        }

        /// <summary>
        /// キーイベントによるディレクトリに入る処理を実行
        /// </summary>
        private void DoKeyEventIntoDirectory()
        {
            // フォーカスされているファイルパスを取得
            string? focusedFilePath = GetListViewItemStringFocused();
            if (focusedFilePath == null)
            {
                return;
            }

            // フォーカスされているパスがディレクトリでない場合は入れないのでここで終了
            if (!System.IO.Directory.Exists(focusedFilePath))
            {
                return;
            }

            // 更新
            UpdateOfDirectoryInfo(focusedFilePath);
        }

        /// <summary>
        /// キーイベントによるフォーカスを検索テキストボックスに移動する処理を実行
        /// </summary>
        private void DoKeyEventFocusOnSearchTextBox()
        {
            // フォーカスされている場所がファイルリストの先頭(一番上)でなければ移動しない
            ListViewItem? focused = GetListViewItemFocused();
            if (focused == null)
            {
                return;
            }

            string? focusedStr = GetListViewItemStringFocused();
            if (focusedStr != _mainWindow.FileListDisplaying.FileList[0])
            {
                return;
            }

            // フォーカス移動
            _mainWindow.SearchText.Focus();
        }

        /// <summary>
        /// キーイベントによるプログラム一覧表示を実行
        /// </summary>
        private void DoKeyEventShowPrograms()
        {
            // フォーカスされているファイルパスを取得
            string? focusedFilePath = GetListViewItemStringFocused();
            if (focusedFilePath == null)
            {
                return;
            }

            // コンテキストメニュー一覧ウィンドウを表示
            FileContextMenuWindow window = new FileContextMenuWindow(focusedFilePath);
            window.Owner = _mainWindow;
            window.Show();

            // コンテキストメニュー一覧ウィンドウが閉じるまでメイン画面を無効化
            _mainWindow.IsEnabled = false;    // FileContextMenuWindow に最初からキーボードフォーカスを当てるための無効化
            window.Closed += FileContextMenuWindowClosed;
        }

        private void FileContextMenuWindowClosed(object? sender, EventArgs e)
        {
            _mainWindow.IsEnabled = true;    // 無効化からの復帰

            // ウィンドウ閉じただけだとフォーカスは宙ぶらりんになったので明示的に指定
            _mainWindow.SearchText.Focus();
            System.Windows.Input.Keyboard.Focus(_mainWindow.SearchText);
        }
    }
}
