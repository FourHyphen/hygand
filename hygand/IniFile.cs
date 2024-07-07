using System.Runtime.InteropServices;
using System.Text;

namespace MyFileLauncher
{
    internal class IniFile
    {
        private readonly string _iniPath;

        [DllImport("KERNEL32.DLL", CharSet = CharSet.Unicode)]
        private static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);

        internal IniFile(string iniPath)
        {
            _iniPath = System.IO.Path.GetFullPath(iniPath);
        }

        internal string GetValue(string section, string key)
        {
            StringBuilder buffer = new(256);
            if (System.IO.File.Exists(_iniPath))
            {
                GetPrivateProfileString(section, key, "", buffer, (uint)buffer.Capacity, _iniPath);
            }

            return buffer.ToString();
        }
    }
}