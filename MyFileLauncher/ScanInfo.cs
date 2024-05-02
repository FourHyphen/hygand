using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;

namespace MyFileLauncher
{
    internal class ScanInfo
    {
        private static readonly string ScanInfoFileName = "Scan.info";

        public IReadOnlyCollection<string> ScanDirectories { get; }

        public IReadOnlyCollection<string> NotScanDirectories { get; }

        public static ScanInfo CreateInstance()
        {
            string scanInfoFilePath = System.IO.Path.GetFullPath(ScanInfoFileName);
            return CreateInstance(scanInfoFilePath);
        }

        public static ScanInfo CreateInstance(string scanInfoFilePath)
        {
            return new ScanInfo(scanInfoFilePath);
        }

        private ScanInfo(string scanInfoFilePath)
        {
            string[] contents = System.IO.File.ReadAllLines(scanInfoFilePath);
            ScanDirectories = GetScanDirectories(contents);
            NotScanDirectories = GetNotScanDirectories(contents);
        }

        /// <summary>
        /// 検索ディレクトリの一覧を返す
        /// </summary>
        private HashSet<string> GetScanDirectories(string[] contents)
        {
            // 先頭が "-" ではないディレクトリパスを検索ディレクトリとする
            // 空行は対象外
            HashSet<string> scans = contents.Where(s => s != "" && s[0] != '-')
                                            .ToHashSet();

            // コメント行をはじく
            HashSet<string> comments = contents.Where(s => s.Length >= 2 && s[0..2] == "/*")
                                               .ToHashSet();
            scans.ExceptWith(comments);

            return scans;
        }

        /// <summary>
        /// 検索しないディレクトリの一覧を返す
        /// </summary>
        private HashSet<string> GetNotScanDirectories(string[] contents)
        {
            // 先頭が "-" のディレクトリパスを検索しないディレクトリととする
            // 空行は対象外
            return contents.Where(s => s != "" && s[0] == '-')
                           .Select(s => s.Substring(1))    // 先頭の "-" を除外
                           .ToHashSet();
        }
    }
}