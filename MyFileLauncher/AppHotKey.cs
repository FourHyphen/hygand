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
        private HotKey _hotKey;

        internal AppHotKey(Window window)
        {
            _iniFile = new IniFile(HotKeyIniFileName);
            _hotKey = new HotKey(window);
        }

        internal bool RegisterTogglingDisplayOnOff(EventHandler handler)
        {
            (ModifierKeys, Key) keys = GetHotKeyTogglingDisplayOnOff();
            return _hotKey.Register(keys.Item1, keys.Item2, handler);
        }

        private (ModifierKeys, Key) GetHotKeyTogglingDisplayOnOff()
        {
            string modKeyCode = _iniFile.GetValue(section: "TogglingDisplayOnOff", key: "ModifierKey");
            string keyCode = _iniFile.GetValue(section: "TogglingDisplayOnOff", key: "Key");

            ModifierKeys modKey = ToModifierKeys(modKeyCode);
            Key key = ToKey(keyCode);

            Debug.WriteLine($@"TogglingDisplayOnOff: ModifierKeys: {modKey}");
            Debug.WriteLine($@"TogglingDisplayOnOff: Key         : {key}");
            return (modKey, key);
        }

        private ModifierKeys ToModifierKeys(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return ModifierKeys.None;
            }

            try
            {
                return (ModifierKeys)Convert.ToInt32(str, 10);
            }
            catch
            {
                return ModifierKeys.None;
            }
        }

        private Key ToKey(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return Key.None;
            }

            try
            {
                // return KeyInterop.KeyFromVirtualKey(Convert.ToInt32(str, 16));    // 16 進数仮想キーコードから変換する場合
                return (Key)Convert.ToInt32(str, 10);
            }
            catch
            {
                return Key.None;
            }
        }

        public void Dispose()
        {
            _hotKey?.Dispose();
        }
    }
}
