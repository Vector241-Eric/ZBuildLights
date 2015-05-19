ZBuildLights
============

ZBuildLights controls Z-Wave devices to provide continuous monitoring of build servers. ZBuildLights handles complex configuration scenarios allowing devices within the same Z-Wave network to be connected to multiple builds from multiple servers. So, if team A wants their own red/yellow/green lights connected to one set of builds, and team B wants a [fake fire to turn on when any of their builds fail](http://amzn.to/1R0f5uZ), both teams can monitor their builds using the same Z-Wave network and a single ZBuildLights installation.

ZBuildLights works with any Continuous Integration (CI) server that produces CCTray XML. Servers that produce this format of status XML include all variants of Cruise Control, TeamCity, Hudson, Jenkins, and Go to name a few. 

A ZBuildLights installation includes two pieces of software -- an ASP.Net MVC application that handles all of the configuration, and a tiny Windows service that triggers periodic status updates.

Suggested Hardware
==================

ZBuildLights currently supports a single Z-Wave Controller and multiple Z-Wave switches. Any Z-Wave switch device will work with ZBuildLights. These switches can be switched lamp socket modules, wall switches, or switched power outlets. Here are a couple of hardware recommendations that work great for me:

* [Aeon Labs Z-Stick Controller](http://bitly.com/1GGmRVW). This USB Dongle controller works very well.
* [Aeon Labs Smart Power Strip](http://bitly.com/1DL4lIK). This power strip includes four individually switchable outlets and two always on outlets. This is a tidy package if you want to install multiple lights (different colors) in the same location.
* [LED Strip Lighting](http://bitly.com/1PZjxtp). These plug into the Z-Wave power strip and give off plenty of light for the whole area to notice when the color changes.
* [A dimmer for the lights](http://amzn.to/1R0hTIE). The LED strips that I use are dimmable, and this little thing is awesome for cutting down the power in small spaces.


Installing ZBuildLights
=======================

ZBuildLights installation follows this basic flow:

* Prepare the Host Machine
* Install the ZBuildLights web application
* Install the ZBuildLights Windows service 

Fortunately, ZBuildLights includes some installation scripts to make these steps much easier. The installation scripts assume that certain components such as IIS have already been installed on the host.

Prepare the Host Machine
------------------------

The Windows host machine should have the following components installed:

* Full IIS should be installed
* .Net Framework 4.5 should be installed
* .Net 4.5 should be registered with IIS

A quick web search should turn up installation instructions for these components on your particular version of Windows. Note that registering ASP.Net with IIS has changed for newer versions of Windows.  See this article for more details: [https://support.microsoft.com/en-us/kb/2736284](https://support.microsoft.com/en-us/kb/2736284)

Running the Installation Script
-------------------------------

1. Make sure the host machine has been prepared as detailed in "Prepare the Host Machine."
2. Download the latest release from Github at [https://github.com/Vector241-Eric/ZBuildLights/releases](https://github.com/Vector241-Eric/ZBuildLights/releases)
3. Unzip the release
4. Modify the configuration settings in DeploymentSettings.ps1. Specific instructions for each configuration value are included in this file.
5. Execute the Install-ZBuildLights.ps1 script using Administrator permissions. This script will perform the following actions
	1. Create a new IIS Application pool
	2. Install the ASP.Net application running under the new app pool
	3. Configure the application based on the settings in DeploymentSettings.ps1
	4. Install a local Windows service, also based on the settings in DeploymentSettings.ps1

After executing the installation script, you should be ready to configure ZBuildLights

Configuring the Network and Build Servers
-----------------------------------------

Once ZBuildLights is installed, you are ready to setup the Z-Wave network and assign the relationships between devices and builds. Follow these steps to get your monitoring devices up and running:

1. Follow the manufacturer's instructions to pair the Z-Wave devices with the Z-Wave controller.
2. Add your build servers using the page under (Admin --> Manage Cruise Servers).
3. Setup project groups under (Admin --> Projects and Lights).
	1. Add a project using the "Add Project" button
	2. Use the "Toggle Status" link to figure out which switch is connected to which device, then assign lights to your new project.
	3. Set the color of each light. "Red" lights will turn on if any associated build is failing, "Green" lights will turn on if all associated builds are passing, and "Yellow" lights will turn on if any build is currently running.   


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


