using System;

namespace Testhygand
{
    class Common
    {
        public static string GetEnvironmentDirPath()
        {
            if (System.IO.Directory.Exists(Environment.CurrentDirectory + "/TestData"))
            {
                // テストスイートの場合、2回目以降？はすでに設定済み
                return Environment.CurrentDirectory;
            }

            string master = System.IO.Path.GetFullPath(Environment.CurrentDirectory + "/../../../");  // テストの単体実行時
            if (!System.IO.Directory.Exists(master + "/TestData"))
            {
                // テストスイートによる全テスト実行時
                master = System.IO.Path.GetFullPath(Environment.CurrentDirectory + "/../../../../hygand");
            }

            return master;
        }

        public static string GetFilePathOfDependentEnvironment(string filePath)
        {
            return System.IO.Path.GetFullPath(filePath);
        }
    }
}
