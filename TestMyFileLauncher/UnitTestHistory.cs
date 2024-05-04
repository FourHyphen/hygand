using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMyFileLauncher
{
    [TestClass]
    public class UnitTestHistory
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
        /// 履歴一覧をファイルから取得する
        /// </summary>
        [TestMethod]
        public void GetHistoryFromHistoryInfoFile()
        {
            MyFileLauncher.History history = CreateHistory();

            Assert.IsTrue(history.Files.Contains(@"C:\with space"));
            Assert.IsTrue(history.Files.Contains(@"D:\日本語\"));

            Assert.AreEqual(expected: 2, actual: history.Files.Count);
        }

        private MyFileLauncher.History CreateHistory()
        {
            // 設定ファイルフォーマットが変わった場合は他テストに影響出る前提、このテストでは共通の設定ファイルを使用する
            return MyFileLauncher.History.CreateInstance(@"TestData\UnitTestHistory\History.info");
        }
    }
}
