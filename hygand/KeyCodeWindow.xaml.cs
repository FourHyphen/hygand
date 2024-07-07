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
            Key0Hex.Content = "";
            Key0Decimal.Content = "";
            Key0Str.Content = "";

            Key1Hex.Content = "";
            Key1Decimal.Content = "";
            Key1Str.Content = "";

            Key2Hex.Content = "";
            Key2Decimal.Content = "";
            Key2Str.Content = "";

            Key3Hex.Content = "";
            Key3Decimal.Content = "";
            Key3Str.Content = "";

            Key4Hex.Content = "";
            Key4Decimal.Content = "";
            Key4Str.Content = "";

            Key5Hex.Content = "";
            Key5Decimal.Content = "";
            Key5Str.Content = "";

            Key6Hex.Content = "";
            Key6Decimal.Content = "";
            Key6Str.Content = "";
        }

        /// <summary>
        /// キー入力内容をセット
        /// </summary>
        private void SetShowingKeyCode(int index, short keyCodeDecimal, string keyContent)
        {
            string keyCodeHex = "0x" + keyCodeDecimal.ToString("X2");

            // TODO: 入力キー数に応じて表示数を可変にする
            switch (index)
            {
                case 0:
                    Key0Hex.Content = keyCodeHex;
                    Key0Decimal.Content = keyCodeDecimal;
                    Key0Str.Content = keyContent;
                    break;
                case 1:
                    Key1Hex.Content = keyCodeHex;
                    Key1Decimal.Content = keyCodeDecimal;
                    Key1Str.Content = keyContent;
                    break;
                case 2:
                    Key2Hex.Content = keyCodeHex;
                    Key2Decimal.Content = keyCodeDecimal;
                    Key2Str.Content = keyContent;
                    break;
                case 3:
                    Key3Hex.Content = keyCodeHex;
                    Key3Decimal.Content = keyCodeDecimal;
                    Key3Str.Content = keyContent;
                    break;
                case 4:
                    Key4Hex.Content = keyCodeHex;
                    Key4Decimal.Content = keyCodeDecimal;
                    Key4Str.Content = keyContent;
                    break;
                case 5:
                    Key5Hex.Content = keyCodeHex;
                    Key5Decimal.Content = keyCodeDecimal;
                    Key5Str.Content = keyContent;
                    break;
                case 6:
                    Key6Hex.Content = keyCodeHex;
                    Key6Decimal.Content = keyCodeDecimal;
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
