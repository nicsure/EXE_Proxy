This project is a tool to aid in debugging Windows GUI front-ends for console applications. The compiled .exe from this project replaces the console level exe being called by the GUI front-end and the original exe is renamed appending _orig to the filename (without extension)
For example, let's say your console application exe being called from the GUI is named: qemu-system-x86_64.exe

copy EXE_Proxy.exe to the folder containing qemu-system-x86_64.exe
rename qemu-system-x86_64.exe to qemu-system-x86_64_orig.exe
rename EXE_Proxy.exe to qemu-system-x86_64.exe

When the exe is called, it will call the original executable and record a _log.txt (qemu-system-x86_64_log.txt) file containing the full command-line used to run it and all console output that was generated allowing you to see what the GUI is doing, what parameters it's passing and review the console application's output for errors etc.

I wrote this primarily for debugging problems with the qtemu GUI front-end for qemu as it doesn't correctly log stuff.

C# .net 6.0 / Visual Studio 2022

-nicsure- 2022