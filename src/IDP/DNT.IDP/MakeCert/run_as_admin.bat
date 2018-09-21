%~dp0makecert.exe -r -pe -n "CN=DntIdpSigningCert" -b 01/01/2018 -e 01/01/2025 -eku 1.3.6.1.5.5.7.3.3 -sky signature -a sha256 -len 2048 -ss my -sr LocalMachine
pause