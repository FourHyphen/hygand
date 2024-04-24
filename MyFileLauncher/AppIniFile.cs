using System;
using System.Windows.Input;

namespace MyFileLauncher
{
    internal class AppIniFile
    {
        private readonly string _iniPath;

        internal AppIniFile(string iniPath)
        {
            this._iniPath = iniPath;
        }

        internal (ModifierKeys, Key) TogglingDisplayOnOff()
        {
            // TODO: ini から設定読み込む
            return (ModifierKeys.Shift, Key.CapsLock);
        }
    }
}