ZBuildLights
============

ZWaveControl for build status lights


Rebuilding the OpenZWave .Net Library
=====================================

1. Dowload the source from https://github.com/OpenZWave/open-zwave/releases
    1. These instructions were written against release 1.3
2. Unzip the source .zip file into a new folder **[zwave source]**
3. Open a command prompt and change directory into **[zwave source]\dotnet\examples\OZWForm**
4. Execute the command **CopyFilesVS2010.cmd**
5. Open the solution file at **[zwave source]\dotnet\examples\OZWForm\src\OZWForm.sln**
6. Allow newer versions of Visual Studio to upgrade the projects.
7. From the menu up top, click **Build --> Rebuild Solution**
8. Once compilation is complete, the OpenZWave .Net library will be at **[zwave source]\dotnet\examples\OZWForm\src\bin\x86\Debug\OpenZWaveDotNetd.dll**

Installing to IIS
=================

1. Make sure the host machine has IIS installed.
2. Register Asp.Net 4.5 with IIS
    1. Use the "Turn Windows Features on or off"
    2. Enable "Internet Information Services / World Wide Web Services / Application Development Features / ASP.Net 4.5"
    3. See https://support.microsoft.com/en-us/kb/2736284
