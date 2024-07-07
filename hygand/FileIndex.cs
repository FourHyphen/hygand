using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MyFileLauncher
{
    internal class FileIndex
    {
        private static readonly string IndexFileName = "Index.info";

        private string IndexFilePath { get; }

        private HashSet<string> Indexes { get; set; } = new HashSet<string>();

        internal static FileIndex CreateInstance()
        {
            string indexFilePath = System.IO.Path.GetFullPath(IndexFileName);
            return CreateInstance(indexFilePath);
        }

        internal static FileIndex CreateInstance(string indexFilePath)
        {
            return new FileIndex(indexFilePath);
        }

        private FileIndex(string indexFilePath)
        {
            IndexFilePath = indexFilePath;
            if (Exists())
            {
                Indexes = ReadIndexFile();
            }
        }

        /// <summary>
        /// Index がすでに存在するかを返す
        /// </summary>
        internal bool Exists()
        {
            return System.IO.File.Exists(IndexFilePath);
        }

        /// <summary>
        /// インデックスファイルを読む
        /// </summary>
        private HashSet<string> ReadIndexFile()
        {
            // 空行を除いた行毎にリスト化
            string[] contents = System.IO.File.ReadAllText(IndexFilePath).Replace("\r", "").Split("\n");
            return contents.Where(s => s != "").ToHashSet();
        }

        /// <summary>
        /// インデックスファイルを作成する
        /// </summary>
        internal void CreateIndexFile(ScanInfo scanInfo)
        {
            HashSet<string> files = GetFilePathes(scanInfo);
            CreateIndexFile(files);

            // 内部的にもインデックスを保持する
            Indexes = files;
        }

        /// <summary>
        /// 検索ディレクトリ内のサブディレクトリを含む全ファイル＆ディレクトリのフルパスを返す
        /// </summary>
        private HashSet<string> GetFilePathes(ScanInfo scanInfo)
        {
            HashSet<string> pathes = new();
            foreach (string searchDir in scanInfo.ScanDirectories)
            {
                pathes.UnionWith(GetAllFilesIn(searchDir, scanInfo.NotScanDirectories));
            }

            return pathes;
        }

        /// <summary>
        /// ディレクトリ内のサブディレクトリを含む全てのファイル＆ディレクトリ一覧を取得する
        /// </summary>
        private HashSet<string> GetAllFilesIn(string dirPath, IReadOnlyCollection<string> notScanDirectories)
        {
            try
            {
                var files = GetAllFilesInCore(dirPath, notScanDirectories);
                return files;
            }
            catch
            {
                return new HashSet<string>();
            }
        }

        private HashSet<string> GetAllFilesInCore(string dirPath, IReadOnlyCollection<string> notScanDirectories)
        {
            HashSet<string> files = new();
            System.IO.DirectoryInfo di = new(dirPath);

            // ディレクトリのサブディレクトリを全探索
            foreach (DirectoryInfo dir in di.EnumerateDirectories())
            {
                // 探索するサブディレクトリであれば再帰的に潜る
                if (IsScanDirectory(dir, notScanDirectories))
                {
                    files.UnionWith(GetAllFilesInCore(dir.FullName, notScanDirectories));
                }
                else
                {
                    Debug.WriteLine($@"not scan: {dir.FullName}");
                }
            }

            // 現階層に探索するサブディレクトリがなければファイルリスト取得
            files.UnionWith(System.IO.Directory.GetFileSystemEntries(dirPath, "*", System.IO.SearchOption.TopDirectoryOnly));

            // 検索しないディレクトリを結果に含めないよう除外
            HashSet<string> removings = GetRemovingFiles(files, notScanDirectories);
            files.ExceptWith(removings);

            return files;
        }

        /// <summary>
        /// スキャンするディレクトリかどうかを返す
        /// </summary>
        private bool IsScanDirectory(DirectoryInfo di, IReadOnlyCollection<string> notScanDirectories)
        {
            // 隠しディレクトリは除外
            if (((di.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden))
            {
                return false;
            }

            // 検索しないディレクトリ一覧の条件に合う場合は除外
            string dirPath = di.FullName;
            foreach (string notScanDirectory in notScanDirectories)
            {
                if (dirPath.Contains(notScanDirectory))
                {
                    return false;
                }
            }

            // アクセス不可ディレクトリは除外
            try
            {
                // 権限を見る
                // ダメなパス例: @"C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\Binn\Xtp"
                System.IO.FileInfo fi = new(dirPath);
                var fileSecurity = System.IO.FileSystemAclExtensions.GetAccessControl(fi);
                //AuthorizationRuleCollection rules = fileSecurity.GetAccessRules(true, true, typeof(NTAccount));
                //foreach (var rule in rules)
                //{
                //    FileSystemAccessRule fsar = (FileSystemAccessRule)rule;
                //    Debug.WriteLine($@"fsar.AccessControlType: {fsar.AccessControlType}");
                //    Debug.WriteLine($@"    (fsar.IdentityReference as NTAccount).Value: {(fsar.IdentityReference as NTAccount)?.Value}");
                //    Debug.WriteLine($@"    fsar.FileSystemRights: {fsar.FileSystemRights}");
                //    Debug.WriteLine($@"    fsar.IsInherited: {fsar.IsInherited}");
                //}

            }
            catch
            {
                return false;
            }

            return true;
        }

        private HashSet<string> GetRemovingFiles(HashSet<string> scanResults, IReadOnlyCollection<string> notScanDirectories)
        {
            HashSet<string> removings = new();
            foreach (string file in scanResults)
            {
                foreach (string notScan in notScanDirectories)
                {
                    if (file.Contains(notScan))
                    {
                        removings.Add(file);
                        Debug.WriteLine($@"remove: {file}");
                    }
                }
            }

            return removings;
        }

        /// <summary>
        /// list を改行付き平坦化した文字列を中身としたインデックスファイルを作成する
        /// </summary>
        private void CreateIndexFile(HashSet<string> list)
        {
            string contents = string.Join("\r\n", list) + "\r\n";
            System.IO.File.WriteAllText(IndexFilePath, contents);
        }

        /// <summary>
        /// インデックスを部分一致検索する
        /// </summary>
        internal HashSet<string> Search(string word)
        {
            if (Indexes.Count == 0)
            {
                return new HashSet<string>();
            }

            // 検証に使えるよう結果を直接 return しない
            var result = Indexes.Where(s => s.Contains(word)).ToHashSet();
            return result;
        }
    }
}