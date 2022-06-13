# Android-OEM-Bruteforce

Don't trust me and replace the tool folder files with your own files! 
But don't move the folder away!

# Setup 
## Get your IMEI
- Boot your phone as you normaly do it.
- Call ``*#06#`` save the IMEI

## Umlock OEM 
- Google for it!

## Get your Device-ID
- Open the Commandline/Terminal/CMD whatever
- Nav to the tool folder
- Run ``adb.exe devices``
- save the device ID

## Customize the source code 
- Open the Program.cs
- Replace the IMEI with your IMEI (https://github.com/Isa-0xNull/Android-OEM-Bruteforce/blob/main/Program.cs#L12)
- Replace the DEVICE_ID with yours (https://github.com/Isa-0xNull/Android-OEM-Bruteforce/blob/main/Program.cs#L13)

## Boot your device into bootloader
- Open the Commandline/Terminal/CMD whatever
- Nav to the tool folder
- Run ``adb.exe reboot bootloader``

## Bruteforce 
- Run the Programm now 

## hint
The device will reboot a lot and you have to calculate with a week to brute force
