@echo off
set version=0.38.1

set zipfile=..\..\Releases\AlbumArtDownloaderXUI-%version%.zip
del %zipfile%
"%ProgramFiles%\7-Zip\7z.exe" a -tzip -mx9 -bd %zipfile% @ziplist.txt -x@zipexcludelist.txt
set version=
set zipfile=
