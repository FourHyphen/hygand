using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HotKey _hotKey;

        public MainWindow()
        {
            InitializeComponent();
            InitMainWindow();
        }

        private void InitMainWindow()
        {
            SetHotKey();
        }

        private void SetHotKey()
        {
            // TODO: AppHotKey クラスにラップする
            //_hotKey = new HotKey(this);
            //_hotKey.Register(ModifierKeys.Shift, Key.CapsLock, (_, __) => { MessageBox.Show("HotKey"); });
        }

        protected override void OnClosed(EventArgs e)
        {
            _hotKey.Dispose();
            base.OnClosed(e);
        }
    }
}
