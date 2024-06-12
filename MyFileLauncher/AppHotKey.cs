using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace MyFileLauncher
{
    internal class AppHotKey : IDisposable
    {
        private const string HotKeyIniFileName = "HotKey.ini";

        private IniFile _iniFile;
        private ToggleDisplayOnOff _toggleDisplayOnOff;

        internal AppHotKey()
        {
            _iniFile = new IniFile(HotKeyIniFileName);
            _toggleDisplayOnOff = new ToggleDisplayOnOff();
        }

        /// <summary>
        /// 画面の表示非表示を切り替える処理を呼び出すキーコードを返す
        /// </summary>
        internal short GetKeyCodeTogglingDisplayOnOff()
        {
            string keyCode = _iniFile.GetValue(section: "TogglingDisplayOnOff", key: "Key");

            Debug.WriteLine($@"TogglingDisplayOnOff: Key: {keyCode}");
            return short.Parse(keyCode);
        }

        /// <summary>
        /// 画面の表示非表示を切り替える処理を登録する
        /// </summary>
        internal bool RegisterTogglingDisplayOnOff(ToggleDisplayOnOff.KeyEventHandler func)
        {
            _toggleDisplayOnOff.KeyDownEvent += func;
            _toggleDisplayOnOff.Hook();
            return true;
        }

        public void Dispose()
        {
            _toggleDisplayOnOff?.Dispose();
        }
    }
}
