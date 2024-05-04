using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyFileLauncher
{
    // INotifyPropertyChanged は変更を画面に反映するのに必要
    public partial class FileListDisplay : UserControl, INotifyPropertyChanged
    {
        private const int DisplayingNum = 20;

        private MainWindow _mainWindow;

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
            _mainWindow = mainWindow;
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

        /// <summary>
        /// キーダウンイベント
        /// </summary>
        private void KeyDowned(object sender, KeyEventArgs e)
        {
            // 単押しの場合                  → キー情報は e.Key に入る
            // System キーとの同時押しの場合 → キー情報は e.SystemKey に入る
            KeyDowned(e.Key, e.SystemKey, e.KeyboardDevice.Modifiers);
        }

        /// <summary>
        /// キーダウン時の処理
        /// </summary>
        private void KeyDowned(Key key, Key systemKey, ModifierKeys modifier)
        {
            AppKeys.KeyEventType keyEventType = AppKeys.ToKeyEventType(key, systemKey, modifier);
            if (keyEventType == AppKeys.KeyEventType.EnterKey)
            {
                DoKeyEventFileOpen();
            }
        }

        /// <summary>
        /// キーイベントによるファイルオープンを実行
        /// </summary>
        private void DoKeyEventFileOpen()
        {
            ListViewItem? focused = GetListViewItemFocused();
            if (focused != null)
            {
                OpenFile((string)focused!.Content);

                // ファイルを開いたら用は済んだのでメインウィンドウを非表示化
                _mainWindow.Hide();
            }
        }

        /// <summary>
        /// フォーカスが当てられている ListViewItem を返す、何も当たっていなければ null を返す
        /// </summary>
        private ListViewItem? GetListViewItemFocused()
        {
            // 参考: https://threeshark3.com/binding-listbox-focus/
            for (int i = 0; i < DisplayFileList.Items.Count; i++)
            {
                var obj = DisplayFileList.ItemContainerGenerator.ContainerFromIndex(i);
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
    }
}
