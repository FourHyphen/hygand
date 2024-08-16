using System;

namespace Testhygand
{
    class Common
    {
        /// <summary>
        /// TestData フォルダを直下に持つフォルダを環境フォルダとする
        /// </summary>
        public static string GetEnvironmentDirPath()
        {
            // テストスイートの場合、2回目以降？はすでに設定済み
            if (System.IO.Directory.Exists(Environment.CurrentDirectory + "/TestData"))
            {
                return Environment.CurrentDirectory;
            }

            // GitHub Actions では Testhygand フォルダ直下の out フォルダを作業フォルダ指定する
            string master = System.IO.Path.GetFullPath(Environment.CurrentDirectory + "/../");
            if (System.IO.Directory.Exists(master + "/TestData"))
            {
                return master;
            }

            // ローカルでのテスト単体実行 or テストスイート実行時
            master = System.IO.Path.GetFullPath(Environment.CurrentDirectory + "/../../../");  // テストの単体実行時
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
