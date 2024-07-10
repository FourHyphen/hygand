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

// �e�X�g�v���W�F�N�g�ɂ̂݌��J����
[assembly: InternalsVisibleTo("Testhygand")]

// GitInfo �p�b�P�[�W�ɂ��o�[�W�������� Git �̏���ݒ�
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
