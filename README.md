### Custom Program
 I created Custom Programs/binaries For my engagements. Most of these were created during my engagement period. Now I share a few for educational purposes.

### AMSI Patching Download Web Cradle without using powershell.exe
I originally published Download Web Cradle With AMSI Patching without powershell.exe on Medium.
(https://nyameeeain.medium.com/web-download-cradle-with-amsi-patching-without-powershell-exe-225c31f06e25).
This is a dotnet assembly (dotnet assembly can be either A DLL or en Exe)that program has two methods. First, patch AMSI in the PowerShell runtime to bypass string and signature base detection. Once AMSI is patched sequentially, a PowerShell script will be downloaded from GitHub.

### CommandRunner
The CommandRunner program sequentially conducts local enumeration (e.g., services, network interfaces, local host enumeration, and service permissively and saves the output in the public folder, including timestamps for each command. Timestamps are for reporting purposes. 

### UserAdd

This program creates a local user and adds them to the local administrator group.

### AMSI-Patch 2023
I inspired the following blog to come out of this Powershell script.
https://www.blazeinfosec.com/post/tearing-amsi-with-3-bytes/
