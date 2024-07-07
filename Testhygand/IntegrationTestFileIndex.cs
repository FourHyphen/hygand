using Microsoft.VisualStudio.TestTools.UnitTesting;
using hygand;
using System;
using System.Linq;

namespace Testhygand
{
    [TestClass]
    public class IntegrationTestFileIndex
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
        /// Index ファイルが存在するかを検知する
        /// </summary>
        [TestMethod]
        public void DetectWhetherExistsIndexFile()
        {
            string indexPath = @"TestData\IntegrationTestFileIndex\DetectWhetherExistsIndexFile\Index.info";

            // 準備：すでに存在するなら削除
            DeleteFileIfExists(indexPath);

            // 存在しないことを検知
            var fileIndex = hygand.FileIndex.CreateInstance(indexPath);
            Assert.IsFalse(fileIndex.Exists());

            // 存在することを検知(中身は問わない)
            CreateEmptyFile(indexPath);
            Assert.IsTrue(fileIndex.Exists());

            // 後始末
            DeleteFileIfExists(indexPath);
        }

        private void CreateEmptyFile(string filePath)
        {
            // 空ファイル作成のため処理なし
            using (System.IO.File.Create(filePath)) { }
        }

        private void DeleteFileIfExists(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            Assert.IsFalse(System.IO.File.Exists(filePath));
        }

        /// <summary>
        /// 検索するディレクトリ内の全ファイルを含め、検索しないディレクトリ内のいずれのファイルも記載しないインデックスファイルを作成する
        /// </summary>
        [TestMethod]
        public void CreateIndexFile()
        {
            string testWorkDirPath = @"TestData\IntegrationTestFileIndex\CreateIndexFile";
            string scanInfoPath = $@"{testWorkDirPath}\Scan.info";
            string indexPath = $@"{testWorkDirPath}\Index.info";
            string notScanDirPath = $@"{testWorkDirPath}\not_scan";

            // 準備：前回の残りがあるなら削除
            CleanupTestCreateIndexFile(scanInfoPath, indexPath, notScanDirPath);

            // 準備：テストディレクトリ設定と Scan.info ファイル作成
            InitTestCreateIndexFile(scanInfoPath, notScanDirPath);

            // インデックスファイル作成
            hygand.ScanInfo si = hygand.ScanInfo.CreateInstance(scanInfoPath);
            hygand.FileIndex fileIndex = hygand.FileIndex.CreateInstance(indexPath);
            fileIndex.CreateIndexFile(si);

            // 結果確認：今回作成した Scan.info があり、検索しないディレクトリを含んでいなければ OK
            string[] contents = System.IO.File.ReadAllLines(indexPath);
            Assert.IsTrue(contents.Where(s => s.Contains(scanInfoPath)).Count() == 1);
            Assert.IsFalse(contents.Where(s => s.Contains(notScanDirPath)).Any());

            // 後始末
            CleanupTestCreateIndexFile(scanInfoPath, indexPath, notScanDirPath);
        }

        private void CleanupTestCreateIndexFile(string scanInfoPath, string indexPath, string notScanDirPath)
        {
            DeleteFileIfExists(scanInfoPath);
            DeleteFileIfExists(indexPath);
            DeleteDirectoryIfExists(notScanDirPath);
        }

        private void DeleteDirectoryIfExists(string dirPath)
        {
            if (System.IO.Directory.Exists(dirPath))
            {
                System.IO.Directory.Delete(dirPath, recursive: true);
            }

            Assert.IsFalse(System.IO.Directory.Exists(dirPath));
        }

        private void InitTestCreateIndexFile(string scanInfoPath, string notScanDirPath)
        {
            // 検索しないディレクトリを作成
            System.IO.Directory.CreateDirectory(notScanDirPath);
            System.IO.File.WriteAllText($@"{notScanDirPath}\dummy.txt", "");

            // Scan.info ファイル作成開始
            using var stream = new System.IO.StreamWriter(scanInfoPath);

            // 検索するディレクトリ：検索ディレクトリパスが環境依存にならないよう、Scan.info には Scan.info 保存先ディレクトリパスを記載
            string? scanInfoDirFullPath = System.IO.Path.GetDirectoryName(Common.GetFilePathOfDependentEnvironment(scanInfoPath));
            stream.WriteLine(scanInfoDirFullPath);

            // 検索しないディレクトリ：先頭に "-" を付与
            stream.WriteLine($@"-{Common.GetFilePathOfDependentEnvironment(notScanDirPath)}");
        }

        [TestMethod]
        public void SearchIndex()
        {
            string testWorkDirPath = @"TestData\IntegrationTestFileIndex\SearchIndex";
            string indexPath = $@"{testWorkDirPath}\Index.info";

            // 準備：前回の残りがあるなら削除
            DeleteFileIfExists(indexPath);

            // 準備：インデックス読み込み
            InitTestSearchIndex(indexPath);
            hygand.FileIndex fileIndex = hygand.FileIndex.CreateInstance(indexPath);

            // 検索
            Assert.IsTrue(fileIndex.Search("should_find_2").Count == 2);
            Assert.IsTrue(fileIndex.Search("should_find_1").Count == 1);
            Assert.IsTrue(fileIndex.Search("should_find_0").Count == 0);

            // 後始末
            DeleteFileIfExists(indexPath);
        }

        private void InitTestSearchIndex(string indexPath)
        {
            string dirPath = System.IO.Path.GetDirectoryName(indexPath)!;

            // インデックスファイル作成開始
            using var stream = new System.IO.StreamWriter(indexPath);

            // 同一ファイルパスが複数存在しない前提のインデックスであることに注意
            stream.WriteLine(Common.GetFilePathOfDependentEnvironment($@"{dirPath}/should_find_2/file"));
            stream.WriteLine(Common.GetFilePathOfDependentEnvironment($@"{dirPath}/should_find_2"));
            stream.WriteLine(Common.GetFilePathOfDependentEnvironment($@"{dirPath}/should_find_1"));
        }
    }
}
