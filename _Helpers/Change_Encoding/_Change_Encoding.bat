SET SEARCH=%cd%\..\..\*.cs

for /f "tokens=*" %%G in ('dir %SEARCH% /b /s /a-d') do iconv.exe -f UTF-8 -t ISO-8859-1 "%%G"