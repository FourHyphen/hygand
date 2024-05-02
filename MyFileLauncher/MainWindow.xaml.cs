using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyFileLauncher
{
    public partial class MainWindow : Window
    {
        private AppHotKey _appHotKey;

        private FileIndex _fileIndex;

        public MainWindow()
        {
            InitializeComponent();

            _appHotKey = new AppHotKey(this);
            SetHotKey(_appHotKey);

            InitFileIndex();
        }

        private void SetHotKey(AppHotKey appHotKey)
        {
            appHotKey.RegisterTogglingDisplayOnOff((_, __) => { ToggleDisplayOnOff(); });
        }

        private void ToggleDisplayOnOff()
        {
            if (Visibility == Visibility.Visible)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        private void InitFileIndex()
        {
            // インデックス読んでメモリに展開
            _fileIndex = FileIndex.CreateInstance();
            if (_fileIndex.Exists())
            {
                return;
            }

            // インデックスが存在しないなら作成して良いかを確認
            if (DoUserWantToCreateIndex())
            {
                ScanInfo si = ScanInfo.CreateInstance();
                _fileIndex.CreateIndexFile(si);
            }

            // TODO: テキストボックスの変化をキャッチ
            // TODO: テキストボックス内の文字列でインデックスを検索
            // TODO: 検索結果を表示
        }

        private bool DoUserWantToCreateIndex()
        {
            MessageBoxResult mbr = MessageBox.Show("インデックスを作成しますか？", "Index.info does not exist.", MessageBoxButton.YesNo);
            return (mbr == MessageBoxResult.Yes);
        }

        protected override void OnClosed(EventArgs e)
        {
            _appHotKey.Dispose();
            base.OnClosed(e);
        }
    }
}
