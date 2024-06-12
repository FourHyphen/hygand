using System;

namespace MyFileLauncher
{
    internal class AppHotKey : IDisposable
    {
        private const string HotKeyIniFileName = "HotKey.ini";

        private IniFile _iniFile;
        private ToggleDisplayOnOff _toggleDisplayOnOff;

        internal AppHotKey(ToggleDisplayOnOff.KeyEventHandler keyDownEvent)
        {
            _iniFile = new IniFile(HotKeyIniFileName);
            _toggleDisplayOnOff = new ToggleDisplayOnOff(keyDownEvent, GetKeyCodeTogglingDisplayOnOff());
        }

        /// <summary>
        /// 画面の表示非表示を切り替える処理を呼び出すキーコードを返す
        /// </summary>
        private short GetKeyCodeTogglingDisplayOnOff()
        {
            string keyCode = _iniFile.GetValue(section: "TogglingDisplayOnOff", key: "Key");
            return short.Parse(keyCode);
        }

        /// <summary>
        /// ホットキー登録を解除する
        /// </summary>
        public void Dispose()
        {
            _toggleDisplayOnOff?.Dispose();
        }
    }
}
