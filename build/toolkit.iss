; -- Toolkit.iss --
; This script is used to build the setup program.

#define MyAppName "Dicom toolkit"
#define MyAppVersion "1.4.7"
#define MyAppPublisher "kira"
#define MyAppURL "https://github.com/kira-96/dicom-toolkit/"
#define MyAppExeName "Simple DICOM Toolkit.exe"

#define SourceDir ".."
#define ReleaseDir "Desktop\x64\Release"

[Setup]
AppId={{40C8BAF6-82A4-4F7C-A927-E163F4F66E9C}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
VersionInfoVersion={#MyAppVersion}
DefaultDirName={autopf}\Dicom-toolkit
DisableProgramGroupPage=yes
PrivilegesRequiredOverridesAllowed=dialog
ArchitecturesAllowed=x64 ia64
ArchitecturesInstallIn64BitMode=x64 ia64
OutputDir=.\
OutputBaseFilename=dicom-toolkit
Compression=lzma2
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "chinesesimplified"; MessagesFile: "SimplifiedChinese\ChineseSimplified.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#SourceDir}\{#ReleaseDir}\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\{#ReleaseDir}\Config.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\{#ReleaseDir}\Dicom.Core.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\{#ReleaseDir}\Dicom.Native64.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\{#ReleaseDir}\FluentValidation.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\{#ReleaseDir}\LiteDB.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\{#ReleaseDir}\Microsoft.Xaml.Behaviors.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\{#ReleaseDir}\MQTTnet.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\{#ReleaseDir}\Nett.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\{#ReleaseDir}\NLog.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\{#ReleaseDir}\Ookii.Dialogs.Wpf.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\{#ReleaseDir}\Polly.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\{#ReleaseDir}\Stylet.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\{#ReleaseDir}\System.Net.Security.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\{#ReleaseDir}\System.Net.WebSockets.Client.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\{#ReleaseDir}\System.Net.WebSockets.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\{#ReleaseDir}\System.Security.Cryptography.Algorithms.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\{#ReleaseDir}\System.Security.Cryptography.Encoding.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\{#ReleaseDir}\System.Security.Cryptography.Primitives.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\{#ReleaseDir}\System.Security.Cryptography.X509Certificates.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\{#ReleaseDir}\System.Threading.Tasks.Extensions.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\{#ReleaseDir}\System.ValueTuple.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\{#ReleaseDir}\NLog.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\{#ReleaseDir}\Fate.dcm"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\LICENSE"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\docs\*"; DestDir: "{app}\docs"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: filesandordirs; Name: {app}
