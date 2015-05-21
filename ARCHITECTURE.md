Why Does Everything Happen Through the Web Application?
==

My current understanding of OpenZWave, somewhat validated by some experience, is that the ZWManager
allocates some system resources in unmanaged code. It appears that ZWManager and its underlying
implementation should be a singleton at the system level -- not just the process level. Therefore,
the web application maintains the reference to the manager.

At one point I thought, why not just reference the ZBuildLights services directly from the Windows
service so that the Windows service does not need to make a web request? Then I realized that the
manager needs to be a system singleton. Based on the current architecture, the web site needs a
manager to read the devices on the network.

The (invasive) option would be to move all interactions with the Z-Wave network into the service.
Given that the web site needs some synchronous interaction with the Z-Wave network, this would
likely be a difficult undertaking for very little benefit.

If someone comes along with more knowledge of OpenZWave to improve my understanding of how this
should work, I am all ears, and I review pull requests.
