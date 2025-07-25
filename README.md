# energenie-standalone
EnergenieControl.exe is a standalone cli tool to control energenie EG-PMS2. 
It is written in C# and compiled to .exe using dotnet. 
I used the energenie provided SDK (PMDll.dll). If you want to change the code, you can change the `EnergenieControl/Program.cs` file.

Note: This was a quick chatgpt coded solution which works fine for me. I am not a C# developer.
## Dependencies

- Install the Visual C++ 2008 Redistributable (x86) to satisfy its side-by-side dependencies. 
[Visual C++ 2008 Redistributable (x86)](https://www.microsoft.com/en-us/download/details.aspx?id=26368&msockid=3459b86b9e9c6b59158eae589f816a57)
- Add PMDll.dll to PATH or have it in the same folder as the .exe

## Usage
Note: Sockets are indexed from 0 to 3. The output socket number is the same as written on the device.
Tested with Energenie EG-PMS2 (PMS2-100) on Windows 11.
```
>EnergenieControl.exe
Usage: EnergenieControl.exe [on|off] <socket (0-3)>

>EnergenieControl.exe off 0
Socket 1 turned OFF

>EnergenieControl.exe  on 0
Socket 1 turned ON
```

## Rebuild code
```
cd EnergenieControl  
dotnet publish -c Release -r win-x86 --self-contained true -p:PublishSingleFile=true -o publish
```
