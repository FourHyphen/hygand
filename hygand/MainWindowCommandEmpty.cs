namespace hygand
{
    /// <summary>
    /// DisplayFileList への空コマンド定義クラス
    /// </summary>
    internal class MainWindowCommandEmpty : MainWindowCommand
    {
        private protected override Result ExecuteCore()
        {
            // nothing
            return Result.NoProcess;
        }
    }
}