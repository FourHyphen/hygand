using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                                     //(used if a resource is not found in the page,
                                     // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                              //(used if a resource is not found in the page,
                                              // app, or any theme specific resource dictionaries)
)]

// テストプロジェクトにのみ公開する
[assembly: InternalsVisibleTo("Testhygand")]

// GitInfo パッケージによりバージョン情報に Git の情報を設定
[assembly: AssemblyFileVersionAttribute(
    ThisAssembly.Git.Tag
)]

[assembly: AssemblyInformationalVersionAttribute(
    ThisAssembly.Git.Tag + "-" +
    ThisAssembly.Git.Commit
)]

[assembly: AssemblyVersionAttribute(
    ThisAssembly.Git.Tag
)]
