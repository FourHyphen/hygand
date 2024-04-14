using System;
using System.Collections.Generic;
using System.Linq;

namespace MyFileLauncher
{
    internal class FileIndex
    {
        private static readonly string IndexFileName = "Index.info";

        private string IndexFilePath { get; }

        public static FileIndex CreateInstance()
        {
            string indexFilePath = System.IO.Path.GetFullPath(IndexFileName);
            return CreateInstance(indexFilePath);
        }

        public static FileIndex CreateInstance(string indexFilePath)
        {
            return new FileIndex(indexFilePath);
        }

        private FileIndex(string indexFilePath)
        {
            IndexFilePath = indexFilePath;
        }

        /// <summary>
        /// Index がすでに存在するかを返す
        /// </summary>
        public bool Exists()
        {
            return System.IO.File.Exists(IndexFilePath);
        }

        /// <summary>
        /// インデックスファイルを作成する
        /// </summary>
        public void CreateIndexFile(ScanInfo scanInfo)
        {
            HashSet<string> all = GetFilePathes(scanInfo.ScanDirectories);
            HashSet<string> files = ExcludeNotScanDirectoriesFromFileList(all, scanInfo.NotScanDirectories);
            CreateIndexFile(files);
        }

        /// <summary>
        /// 検索ディレクトリ内のサブディレクトリを含む全ファイル＆ディレクトリのフルパスを返す
        /// </summary>
        private HashSet<string> GetFilePathes(IReadOnlyCollection<string> searchDirs)
        {
            IEnumerable<string> pathes = searchDirs.SelectMany(dirPath => GetAllFilesIn(dirPath));
            return pathes.ToHashSet();
        }

        /// <summary>
        /// ディレクトリ内のサブディレクトリを含む全てのファイル＆ディレクトリ一覧を取得する
        /// </summary>
        private string[] GetAllFilesIn(string dirPath)
        {
            return System.IO.Directory.GetFileSystemEntries(dirPath, "*", System.IO.SearchOption.AllDirectories);
        }

        /// <summary>
        /// ファイルリストから検索しないディレクトリのファイルを除外したリストを返す
        /// </summary>
        private HashSet<string> ExcludeNotScanDirectoriesFromFileList(IReadOnlyCollection<string> fileList, IReadOnlyCollection<string> notScanDirs)
        {
            // 検索しないディレクトリのファイルではないものを一覧化する
            return fileList.Where(path => !DoStrStartWith(path, notScanDirs)).ToHashSet();
        }

        /// <summary>
        /// 文字列がリスト内の文字列のいずれか 1 つ以上と前方一致するかを返す
        /// </summary>
        private bool DoStrStartWith(string str, IReadOnlyCollection<string> list)
        {
            return list.Where(s => str.StartsWith(s)).Count() >= 1;
        }

        /// <summary>
        /// list を改行付き平坦化した文字列を中身としたインデックスファイルを作成する
        /// </summary>
        private void CreateIndexFile(HashSet<string> list)
        {
            string contents = string.Join("\r\n", list) + "\r\n";
            System.IO.File.WriteAllText(IndexFilePath, contents);
        }
    }
}