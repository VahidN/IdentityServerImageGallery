dotnet restore
cd "%~dp0src\IDP\DNT.IDP\"
start _0-restore.bat
cd "%~dp0src\MvcClient\ImageGallery.MvcClient.WebApp\"
start _0-restore.bat
cd "%~dp0src\WebApi\ImageGallery.WebApi.WebApp\"
start _0-restore.bat