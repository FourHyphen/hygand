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
using System.Windows.Shapes;

namespace MyFileLauncher
{
    public partial class KeyCodeWindow : Window
    {
        public KeyCodeWindow()
        {
            InitializeComponent();
        }

        private void KeyCodeWindowKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // 単押しの場合                  → キー情報は e.Key に入る
            // System キーとの同時押しの場合 → キー情報は e.SystemKey に入る
            ShowKeyCode(e.Key, e.SystemKey, e.KeyboardDevice.Modifiers, e.ImeProcessedKey);
        }

        /// <summary>
        /// キーコードを表示する
        /// </summary>
        private void ShowKeyCode(Key key, Key systemKey, ModifierKeys modifier, Key imeProcessedKey)
        {
            ModifierKeyLabel.Content = ((int)modifier).ToString();
            SystemKeyLabel.Content = ((int)systemKey).ToString();
            KeyLabel.Content = ((int)key).ToString();
            ImeKeyLabel.Content = ((int)imeProcessedKey).ToString();
        }
    }
}
