<p align="center">
  <img src="FolderWatchGUI/Images/cctvHeaderImage.png" height="500"/>
</p>

# What is it?
A Windows Service that is installed via a GUI. The Service monitors a user-specified folder for Excel files. When a new Excel file is placed in the folder, that file is automatically emailed to the specified list of recipients. Notifications via email about the Service starting or stopping are also automatically sent to the specified 'sender email address'.

# Requirements
A Windows 7/10 PC. It may work on other Windows OS's, but I have only tested it on 7 & 10. The Service also only works with NHS.NET addresses - it was made this way intentionally. 

# Installation
1. Clone the repo.

2. You may want to customise the notifications that are sent, which you can do by editing the following file (Place your text/html with the quotation marks):

<p align="middle">
  <img src="FolderWatchGUI/ScreenShots/NotificationCustomisation.PNG"/>
</p>

3. Following that, build and run!

**NOTE: If the Service has already been started, and you wish to change any of the settings, remember to restart the Service after the settings changes have been saved!**

# Screen Shots
<p align="center">
  <img src="FolderWatchGUI/ScreenShots/ss1.PNG"/>
</p>
<p align="center">
  <img src="FolderWatchGUI/ScreenShots/ss2.PNG"/>
</p>
<p align="center">
  <img src="FolderWatchGUI/ScreenShots/ss3.PNG"/>
</p>
<p align="center">
  <img src="FolderWatchGUI/ScreenShots/ss4.PNG"/>
</p>
