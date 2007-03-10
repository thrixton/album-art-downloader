!define PRODUCT_NAME "Album Art Downloader XUI"
!define PRODUCT_VERSION "0.1"
!define PRODUCT_WEB_SITE "https://sourceforge.net/projects/album-art"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\AlbumArt.exe"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"

SetCompressor lzma

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"

OutFile "../../Releases/AlbumArtDownloaderXUI-${PRODUCT_VERSION}.exe"
InstallDir "$PROGRAMFILES\AlbumArtDownloader"
Icon "${NSISDIR}\Contrib\Graphics\Icons\modern-install.ico"
UninstallIcon "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"
InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" ""
LicenseData "License.txt"
ShowInstDetails hide
ShowUnInstDetails show
XPStyle on

Page license
Page directory
Page components
Page instfiles

Function .oninit
  #Check for .net presence
  ReadRegStr $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.0\Setup" "InstallSuccess"
  StrCmp $0 "1" dotnetok
  MessageBox MB_ICONEXCLAMATION|MB_YESNO "The Microsoft .NET Framework version 3.0 is not installed.$\nPlease download and install the framework before installing ${PRODUCT_NAME}.$\n$\nWould you like to visit the download page now?" IDNO +2
  ExecShell "open" "http://www.microsoft.com/downloads/thankyou.aspx?familyId=10CC340B-F857-4A14-83F5-25634C3BF043&displayLang=en"
  Abort
  dotnetok:
FunctionEnd

Section "!Album Art Downloader"
  SetOutPath "$INSTDIR"
  CreateDirectory "$INSTDIR\Scripts"
  File "License.txt"
  File "..\AlbumArtDownloader\bin\Release\AlbumArt.exe"
  File "..\AlbumArtDownloader\bin\Release\*.dll"
  CreateDirectory "$SMPROGRAMS\Album Art Downloader"
  CreateShortCut "$SMPROGRAMS\Album Art Downloader\Album Art Downloader.lnk" "$INSTDIR\AlbumArt.exe"
SectionEnd

SectionGroup "Image Download Scripts"
Section -ScriptsPath
SetOutPath "$INSTDIR\Scripts"
SetOverwrite ifnewer
#delete ScriptCache, just to be sure. It should be updated automatically, though
Delete "$INSTDIR\Scripts\boo script cache.dll"
SectionEnd

Section "Amazon (US)"
  File "..\Scripts\Scripts\amazon.boo"
SectionEnd
Section "Google"
  File "..\Scripts\Scripts\google.boo"
SectionEnd
Section "Coveralia"
  File "..\Scripts\Scripts\coveralia.boo"
SectionEnd
Section "Cover-Paradies"
  File "..\Scripts\Scripts\cover-paradies.boo"
SectionEnd
Section "CD Universe"
  File "..\Scripts\Scripts\cduniverse.boo"
SectionEnd
Section "Discogs"
  File "..\Scripts\Scripts\discogs.boo"
SectionEnd
Section "Yes24"
  File "..\Scripts\Scripts\yes24.boo"
SectionEnd
Section "Juno Records"
  File "..\Scripts\Scripts\juno-records.boo"
SectionEnd
Section "CoverIsland"
  File "..\Scripts\Scripts\coverisland.boo"
SectionEnd
Section "PsyShop"
  File "..\Scripts\Scripts\psyshop.boo"
SectionEnd
Section "Artists.Trivialbeing (artist images)"
  File "..\Scripts\Scripts\artists.trivialbeing.boo"
SectionEnd

Section "-Other Scripts"
  File /nonfatal "..\Scripts\Scripts\*.boo"
SectionEnd

SectionGroupEnd

Section -AdditionalIcons
  CreateShortCut "$SMPROGRAMS\Album Art Downloader\Uninstall.lnk" "$INSTDIR\uninst.exe"
SectionEnd

Section -Post
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\*.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\*.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
SectionEnd

Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) was successfully removed from your computer."
FunctionEnd

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "Are you sure you want to completely remove $(^Name) and all of its components?" IDYES +2
  Abort
FunctionEnd

Section Uninstall
  Delete "$INSTDIR\uninst.exe"
  Delete "$INSTDIR\AlbumArt.exe"
  Delete "$INSTDIR\License.txt"
  Delete "$INSTDIR\*.dll"
  Delete "$INSTDIR\Scripts\*.boo"
  Delete "$INSTDIR\Scripts\boo script cache.dll"
  Delete "$INSTDIR\Scripts\boo script cache.pdb" #Shouldn't be created, but again, just in case
  RMDir "$INSTDIR\Scripts"
  RMDir "$INSTDIR"

  Delete "$SMPROGRAMS\Album Art Downloader\Uninstall.lnk"
  Delete "$SMPROGRAMS\Album Art Downloader\Album Art Downloader.lnk"
  RMDir "$SMPROGRAMS\Album Art Downloader"

  #delete local app data
  FindFirst $0 $1 "$LOCALAPPDATA\AlbumArtDownloader\AlbumArt.exe_*"
  loop:
    StrCmp $1 "" done
    RMDir /r "$LOCALAPPDATA\AlbumArtDownloader\$1"
    FindNext $0 $1
    Goto loop
  done:

  RMDir "$LOCALAPPDATA\AlbumArtDownloader"

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"
  SetAutoClose true
SectionEnd