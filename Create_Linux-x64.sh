FRAMEWORK=netcoreapp3.1
RUNTIME=linux-x64 

export PATH='/bin':'/home/mgr/.dotnet/tools':'/mnt/c/Oracle/product/12.2.0/client_1/bin':'/mnt/c/Oracle/product/12.2.0/dbhome_1/bin':'/mnt/c/Program Files (x86)/Common Files/Oracle/Java/javapath_target_123026531':'/mnt/c/Program Files (x86)/Groovy/bin':'/mnt/c/Program Files (x86)/Intel/Intel(R) Management Engine Components/DAL':'/mnt/c/Program Files (x86)/LINQPad5':'/mnt/c/Program Files (x86)/Microsoft SDKs/Windows/v10.0A/bin/NETFX 4.7.2 Tools':'/mnt/c/Program Files (x86)/NArrange 0.2.9':'/mnt/c/Program Files (x86)/NuGet':'/mnt/c/Program Files (x86)/Sennheiser/SoftphoneSDK':'/mnt/c/Program Files (x86)/WinMerge':'/mnt/c/Program Files (x86)/dotnet':'/mnt/c/Program Files/Common Files/Intel/WirelessCommon':'/mnt/c/Program Files/ConEmu':'/mnt/c/Program Files/ConEmu/ConEmu':'/mnt/c/Program Files/ConEmu/ConEmu/Scripts':'/mnt/c/Program Files/Git/cmd':'/mnt/c/Program Files/Intel/Intel(R) Management Engine Components/DAL':'/mnt/c/Program Files/Intel/WiFi/bin':'/mnt/c/Program Files/Java/jdk1.8.0_231/bin':'/mnt/c/Program Files/Microsoft SQL Server/130/Tools/Binn':'/mnt/c/Program Files/Microsoft SQL Server/Client SDK/ODBC/170/Tools/Binn':'/mnt/c/Program Files/Microsoft VS Code/bin':'/mnt/c/Program Files/Python/Python37':'/mnt/c/Program Files/Python/Python37/Scripts':'/mnt/c/Program Files/dotnet':'/mnt/c/Program Files/nodejs':'/mnt/c/Program Files/nvm':'/mnt/c/Windows':'/mnt/c/Windows/SysWOW64/Empirum':'/mnt/c/Windows/System32':'/mnt/c/Windows/System32/OpenSSH':'/mnt/c/Windows/System32/WindowsPowerShell/v1.0':'/mnt/c/Windows/System32/wbem':'/mnt/d/Users/mgr/.dotnet/tools':'/sbin':'/usr/bin':'/usr/games':'/usr/local/bin':'/usr/local/games':'/usr/local/sbin':'/usr/sbin'

cd /mnt/d/Users/mgr/Entwicklung/EBuEf2IVU

dotnet deb --configuration Release --runtime $RUNTIME --framework $FRAMEWORK /mnt/d/Users/mgr/Entwicklung/EBuEf2IVU/$1/$1.csproj