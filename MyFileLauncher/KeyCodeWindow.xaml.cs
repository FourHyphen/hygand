using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace MyFileLauncher
{
    public partial class KeyCodeWindow : Window
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern short GetKeyState(int nVirtKey);

        private DispatcherTimer _timer = new DispatcherTimer();

        public KeyCodeWindow()
        {
            InitializeComponent();
            StartShowingKeyboardInput();
        }

        /// <summary>
        /// キー入力表示を開始
        /// </summary>
        private void StartShowingKeyboardInput()
        {
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 50);    // [ms]
            _timer.Tick += new EventHandler(ShowKeyCode);
            _timer.Start();
        }

        /// <summary>
        /// キー入力を表示する
        /// </summary>
        private void ShowKeyCode(object? sender, EventArgs e)
        {
            ResetShowingKeyCode();

            // キーボードによっては押されてないボタンが押されてるように表示されてしまう
            SortedDictionary<short, string> inputed = VirtualKey.GetInputKeys();
            int keyIndex = 0;
            foreach (var key in inputed)
            {
                SetShowingKeyCode(keyIndex, key.Key, key.Value);
                keyIndex++;
            }
        }

        /// <summary>
        /// キー入力表示をリセット
        /// </summary>
        private void ResetShowingKeyCode()
        {
            Key0Num.Content = "";
            Key0Str.Content = "";
            Key1Num.Content = "";
            Key1Str.Content = "";
            Key2Num.Content = "";
            Key2Str.Content = "";
            Key3Num.Content = "";
            Key3Str.Content = "";
            Key4Num.Content = "";
            Key4Str.Content = "";
            Key5Num.Content = "";
            Key5Str.Content = "";
            Key6Num.Content = "";
            Key6Str.Content = "";
        }

        /// <summary>
        /// キー入力内容をセット
        /// </summary>
        private void SetShowingKeyCode(int index, short vkKeyCode, string keyContent)
        {
            string numContent = "0x" + vkKeyCode.ToString("X2");

            // TODO: 入力キー数に応じて表示数を可変にする
            switch (index)
            {
                case 0:
                    Key0Num.Content = numContent;
                    Key0Str.Content = keyContent;
                    break;
                case 1:
                    Key1Num.Content = numContent;
                    Key1Str.Content = keyContent;
                    break;
                case 2:
                    Key2Num.Content = numContent;
                    Key2Str.Content = keyContent;
                    break;
                case 3:
                    Key3Num.Content = numContent;
                    Key3Str.Content = keyContent;
                    break;
                case 4:
                    Key4Num.Content = numContent;
                    Key4Str.Content = keyContent;
                    break;
                case 5:
                    Key5Num.Content = numContent;
                    Key5Str.Content = keyContent;
                    break;
                case 6:
                    Key6Num.Content = numContent;
                    Key6Str.Content = keyContent;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 画面クローズ時イベント
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            _timer.Stop();
            base.OnClosed(e);
        }
    }
}
