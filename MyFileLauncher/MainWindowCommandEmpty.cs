namespace MyFileLauncher
{
    /// <summary>
    /// DisplayFileList への空コマンド定義クラス
    /// </summary>
    internal class MainWindowCommandEmpty : MainWindowCommand
    {
        internal override Result Execute()
        {
            // nothing
            return Result.NoProcess;
        }
    }
}