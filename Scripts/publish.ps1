if ([string]::IsNullOrEmpty($Env:AndroidSigningPassword))
{
	Write-Host "Keystore secret password not set, exiting.";
	return;
}

$buildVersion="0.1.0";
$dotnetTarget="net8.0";
$buildConfiguration="Release";
$projectDirectory = (Get-Item (Split-Path -Path $PSScriptRoot -Parent));
$projectFolder = $projectDirectory.FullName;
$projectName = $projectDirectory.BaseName;
$projectAppName = "${projectName}";
$projectFile="${projectFolder}/${projectAppName}/${projectAppName}.csproj";

# restore the base project dependencies
Set-Location -Path "${projectFolder}";
dotnet restore "${projectFile}";
if (-not $?) {
    Write-Host "Project file not found: ${projectFile}";
    Exit 1;
}

$keystoreFile="android.keystore";
$androidSigningAlias="android-key";
$androidPackageFormats="apk"; # "aab;apk"
$androidTarget="${dotnetTarget}-android";
$publishOutputFolder="publish";
$kestoreFolder = [IO.Path]::Combine($Env:LOCALAPPDATA, "Android");
if (-Not (Test-Path -Path "${kestoreFolder}" -PathType Container))
{
    New-Item -ItemType Directory -Path "${kestoreFolder}";
}
$kestorePath = [IO.Path]::Combine($kestoreFolder, $keystoreFile);
if (-Not (Test-Path -Path "${kestorePath}" -PathType Leaf))
{
    # https://learn.microsoft.com/en-us/dotnet/maui/android/deployment/publish-cli?view=net-maui-8.0
    keytool -genkeypair -v -keystore "${kestorePath}" -alias "${androidSigningAlias}" -keyalg RSA -keysize 2048 -validity 10000;
    keytool -list -keystore "${kestorePath}";
}

dotnet publish "${projectFile}" -c $buildConfiguration --framework $androidTarget /p:Version=$buildVersion /p:AndroidPackageFormats=$androidPackageFormats /p:AndroidKeyStore=true /p:AndroidSigningKeyStore="${kestorePath}" /p:AndroidSigningKeyAlias="${androidSigningAlias}" /p:AndroidSigningKeyPass="${Env:AndroidSigningPassword}" /p:AndroidSigningStorePass="${Env:AndroidSigningPassword}" -o "${publishOutputFolder}" --no-restore --nologo;
