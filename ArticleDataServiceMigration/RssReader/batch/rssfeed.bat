set mydate=%date:~10,4%%date:~4,2%%date:~7,2%
c:\apps\rssreader.exe -c %1 -s %mydate% -e %mydate% 
pause