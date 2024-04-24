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
    public partial class MainWindow : Window
    {
        private AppHotKey _appHotKey;

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
            _appHotKey = AppHotKey.CreateInstance(this);
            _appHotKey.RegisterTogglingDisplayOnOff((_, __) => { ToggleDisplayOnOff(); });
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

        protected override void OnClosed(EventArgs e)
        {
            _appHotKey.Dispose();
            base.OnClosed(e);
        }
    }
}
