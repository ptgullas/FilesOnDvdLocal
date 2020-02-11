# FilesOnDvdLocal
I have a Microsoft Access database that I've used years to catalog various media files that I archive to DVD. These files generally have most or all of their metadata fields in their filenames. Until now, I've imported each DVD's worth of file data into the Access database using a somewhat tedious process involving an Excel spreadsheet, and then manually entering data into Access forms. This WPF application is intended to speed up this process, by reading the data from the filenames, and importing everything directly into Access.

Built for .NET Framework 4.6 because the database is on a Windows Vista laptop (and I don't want to move it just yet), but now that it's mostly finished, I still have issues running this on Vista (for various reasons).

This is my first real WPF application, and part of why I made this was to teach myself a little WPF & the MVVM pattern. Yes, maybe one day, I'll move that Access DB to SQL Server, but it still serves its purpose, and I still like all the custom forms & stuff I've made, so that day is not today.

This solution has several projects:
## DvdImportClient.Wpf
## FilesOnDvdLocal
## FilesOnDvdLocal.Data
## FilesOnDvdLocal.Tests
## DvdPacker.Console
Console app that uses the Bin Packer algorithm to split a directory containing multiple GBs of files into folders that will (close to optimally) fit on a 4.7GB DVD
## AttachmentsSaver.Console
Console app used for extracting attachments from the Access DB
