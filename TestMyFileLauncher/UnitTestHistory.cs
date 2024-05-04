using MyFileLauncher;
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
            MyFileLauncher.History history = MyFileLauncher.History.CreateInstance(@"TestData\UnitTestHistory\GetHistoryFromHistoryInfoFile.info");

            Assert.IsTrue(history.Files.Contains(@"C:\with space"));
            Assert.IsTrue(history.Files.Contains(@"D:\日本語\"));

            Assert.AreEqual(expected: 2, actual: history.Files.Count());
        }

        /// <summary>
        /// 履歴に追加する
        /// </summary>
        [TestMethod]
        public void AddHistory()
        {
            string historyPath = @"TestData\UnitTestHistory\AddHistory.info";
            MyFileLauncher.History history = PrepareTestHistory(historyPath, 19);

            // 準備: 初期値の確認
            Assert.IsTrue(history.Files[0] == @"C:\0");
            Assert.IsTrue(history.Files[18] == @"C:\18");
            Assert.AreEqual(expected: 19, actual: history.Files.Count());

            // 追加
            history.Add(@"C:\19");

            // 確認: 追加したものが先頭にあること
            Assert.IsTrue(history.Files[0] == @"C:\19");
            Assert.AreEqual(expected: 20, actual: history.Files.Count());

            // 確認: 履歴ファイルに追加が反映されていること
            MyFileLauncher.History newHistory = MyFileLauncher.History.CreateInstance(historyPath);
            Assert.IsTrue(history.Files[0] == @"C:\19");
            Assert.AreEqual(expected: 20, actual: history.Files.Count());

            // 後始末
            System.IO.File.Delete(historyPath);
        }

        /// <summary>
        /// 履歴に追加した際、上限を超えていたら上限を保つ
        /// </summary>
        [TestMethod]
        public void AddHistoryKeepingMaxNum()
        {
            string historyPath = @"TestData\UnitTestHistory\AddHistoryKeepingMaxNum.info";
            MyFileLauncher.History history = PrepareTestHistory(historyPath, 20);

            // 準備: これから消えるものがあることと追加しようとしてるものがないことの確認
            Assert.IsTrue(history.Files[19] == @"C:\19");
            Assert.IsFalse(history.Files.Contains(@"C:\20"));
            Assert.AreEqual(expected: 20, actual: history.Files.Count());

            // 追加
            history.Add(@"C:\20");

            // 確認: 追加したものが先頭にあることと上限を超えたので 1 つ削除されていること
            Assert.IsTrue(history.Files[0] == @"C:\20");
            Assert.IsFalse(history.Files.Contains(@"C:\19"));
            Assert.AreEqual(expected: 20, actual: history.Files.Count());

            // 確認: 履歴ファイルに追加が反映されていること
            MyFileLauncher.History newHistory = MyFileLauncher.History.CreateInstance(historyPath);
            Assert.IsTrue(history.Files[0] == @"C:\20");
            Assert.IsFalse(history.Files.Contains(@"C:\19"));
            Assert.AreEqual(expected: 20, actual: history.Files.Count());

            // 後始末
            System.IO.File.Delete(historyPath);
        }

        /// <summary>
        /// すでに履歴に存在するファイルを追加する場合、代わりに履歴の先頭に移動する
        /// </summary>
        [TestMethod]
        public void HistoryFileMoveFirstAndNotAppendIfFileAlreadyExists()
        {
            string historyPath = @"TestData\UnitTestHistory\HistoryFileMoveFirstAndNotAppendIfFileAlreadyExists.info";
            MyFileLauncher.History history = PrepareTestHistory(historyPath, 10);

            // 準備: 事前条件の確認
            Assert.IsFalse(history.Files[0] == @"C:\5");
            Assert.IsTrue(history.Files.Contains(@"C:\5"));
            Assert.AreEqual(expected: 10, actual: history.Files.Count());

            // すでに履歴に存在するものを追加
            history.Add(@"C:\5");

            // 確認: 先頭にあることとすでに存在するので追加ではないこと
            Assert.IsTrue(history.Files[0] == @"C:\5");
            Assert.AreEqual(expected: 10, actual: history.Files.Count());

            // 後始末
            System.IO.File.Delete(historyPath);
        }

        private History PrepareTestHistory(string historyPath, int historyNum)
        {
            // もし前回の結果が残っていれば削除
            if (System.IO.File.Exists(historyPath))
            {
                System.IO.File.Delete(historyPath);
            }

            // History ファイルを作成
            string contents = "";
            for (int i = 0; i < historyNum; i++)
            {
                contents += $"C:\\{i}\r\n";
            }
            System.IO.File.WriteAllText(historyPath, contents);

            // History インスタンスを返す
            return MyFileLauncher.History.CreateInstance(historyPath);
        }
    }
}
