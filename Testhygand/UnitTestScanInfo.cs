namespace TestMyFileLauncher
{
    [TestClass]
    public class UnitTestScanInfo
    {
        private string BeforeEnvironment { get; set; } = Environment.CurrentDirectory;

        [TestInitialize]
        public void Init()
        {
            BeforeEnvironment = Environment.CurrentDirectory;
            Environment.CurrentDirectory = Common.GetEnvironmentDirPath();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Environment.CurrentDirectory = BeforeEnvironment;
        }

        /// <summary>
        /// 検索ディレクトリ一覧を設定ファイルから取得する
        /// </summary>
        [TestMethod]
        public void GetDirectoriesToScanFromScanInfoFile()
        {
            MyFileLauncher.ScanInfo si = CreateScanInfo();

            // 先頭が "-" でないディレクトリは検索する
            Assert.IsTrue(si.ScanDirectories.Contains(@"C:\with space"));
            Assert.IsTrue(si.ScanDirectories.Contains(@"D:\日本語\"));

            Assert.AreEqual(expected: 2, actual: si.ScanDirectories.Count);
        }

        /// <summary>
        /// 検索しないディレクトリ一覧を設定ファイルから取得する
        /// </summary>
        [TestMethod]
        public void GetDirectoriesNotScanFromScanInfoFile()
        {
            MyFileLauncher.ScanInfo si = CreateScanInfo();

            // 先頭が "-" のディレクトリは検索しない(先頭の "-" が消されていることも確認)
            Assert.IsTrue(si.NotScanDirectories.Contains(@"C:\Program Files"));
            Assert.IsTrue(si.NotScanDirectories.Contains(@"E:\"));

            Assert.AreEqual(expected: 2, actual: si.NotScanDirectories.Count);
        }

        /// <summary>
        /// 設定ファイルのコメント行を取得しない
        /// </summary>
        [TestMethod]
        public void DoNotGetCommentLineFromScanInfoFile()
        {
            MyFileLauncher.ScanInfo si = CreateScanInfo();

            // 先頭が "/*" なら取得しない
            foreach (string str in si.ScanDirectories)
            {
                Assert.IsFalse(str[0..2] == "/*");
            }
        }

        private MyFileLauncher.ScanInfo CreateScanInfo()
        {
            // 設定ファイルフォーマットが変わった場合は他テストに影響出る前提、このテストでは共通の設定ファイルを使用する
            return MyFileLauncher.ScanInfo.CreateInstance(@"TestData\UnitTestScanInfo\Scan.info");
        }
    }
}
