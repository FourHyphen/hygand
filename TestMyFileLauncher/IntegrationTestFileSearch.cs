using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMyFileLauncher
{
    [TestClass]
    public class IntegrationTestFileSearch
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
        /// 履歴が空の状態で履歴とインデックスを検索した際、インデックスの検索結果を返す
        /// </summary>
        [TestMethod]
        public void ReturnOnlyIndexWhenSearchHistoryAndIndexIfHistoryIsEmpty()
        {
            string testWorkDirPath = @"TestData\IntegrationTestFileSearch\ReturnOnlyIndexWhenSearchHistoryAndIndexIfHistoryIsEmpty";

            // 準備: 履歴取得(中身は空)
            string historyPath = $@"{testWorkDirPath}\History.info";
            MyFileLauncher.History history = MyFileLauncher.History.CreateInstance(historyPath);

            // 準備: インデックス取得
            string indexPath = $@"{testWorkDirPath}\Index.info";
            MyFileLauncher.FileIndex fi = MyFileLauncher.FileIndex.CreateInstance(indexPath);

            // 検索
            MyFileLauncher.FileSearch fs = new MyFileLauncher.FileSearch();
            string[] result = fs.Search(history, fi, "1");

            // 確認
            Assert.IsTrue(result.Contains(@"C:\1"));
            Assert.AreEqual(expected: 1, actual: result.Count()) ;
        }

        /// <summary>
        /// インデックスが空の状態で履歴とインデックスを検索した際、履歴の検索結果を返す
        /// </summary>
        [TestMethod]
        public void ReturnOnlyHistoryWhenSearchHistoryAndIndexIfIndexIsEmpty()
        {
            string testWorkDirPath = @"TestData\IntegrationTestFileSearch\ReturnOnlyHistoryWhenSearchHistoryAndIndexIfIndexIsEmpty";

            // 準備: 履歴取得
            string historyPath = $@"{testWorkDirPath}\History.info";
            MyFileLauncher.History history = MyFileLauncher.History.CreateInstance(historyPath);

            // 準備: インデックス取得(中身は空)
            string indexPath = $@"{testWorkDirPath}\Index.info";
            MyFileLauncher.FileIndex fi = MyFileLauncher.FileIndex.CreateInstance(indexPath);

            // 検索
            MyFileLauncher.FileSearch fs = new MyFileLauncher.FileSearch();
            string[] result = fs.Search(history, fi, "1");

            // 確認
            Assert.IsTrue(result.Contains(@"C:\1"));
            Assert.AreEqual(expected: 1, actual: result.Count());
        }

        /// <summary>
        /// 履歴とインデックスを検索した際、両者を合わせた結果を履歴先頭で返す
        /// </summary>
        [TestMethod]
        public void SearchHistoryAndIndex()
        {
            string testWorkDirPath = @"TestData\IntegrationTestFileSearch\SearchHistoryAndIndex";

            // 準備: 履歴取得
            string historyPath = $@"{testWorkDirPath}\History.info";
            MyFileLauncher.History history = MyFileLauncher.History.CreateInstance(historyPath);

            // 準備: インデックス取得
            string indexPath = $@"{testWorkDirPath}\Index.info";
            MyFileLauncher.FileIndex fi = MyFileLauncher.FileIndex.CreateInstance(indexPath);

            // 検索
            MyFileLauncher.FileSearch fs = new MyFileLauncher.FileSearch();
            string[] result = fs.Search(history, fi, "1");

            // 確認: 検索結果が履歴先頭のこと
            Assert.AreEqual(expected: @"C:\1", actual: result[0]);
            Assert.AreEqual(expected: @"C:\1_history", actual: result[1]);
            Assert.AreEqual(expected: @"C:\1_index", actual: result[2]);

            // 確認: 履歴とインデックス両方に含まれる場合、重複しないこと
            Assert.AreEqual(expected: 3, actual: result.Count());
        }
    }
}
