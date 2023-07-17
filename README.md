# msix-services
This repository contains an MSIX packaging project that ships a desktop app and a Windows service.

The test `Installer_TemporaryKey.pfx` certificate is used to sign the MSIX installer (password `Test@123`).

## Scenario #1
Potential security vulnerability. A service was added to the new version of the MSIX package and the package is updated using [AddPackageByUriAsync](https://learn.microsoft.com/en-us/uwp/api/windows.management.deployment.packagemanager.addpackagebyuriasync?view=winrt-22621) and [DeferRegistrationWhenPackagesAreInUse](https://learn.microsoft.com/en-us/uwp/api/windows.management.deployment.registerpackageoptions.deferregistrationwhenpackagesareinuse?view=winrt-22621) = true:
* Installation is postponed until the next launch, a user closes the app, a user attempts to open the app, the app fails to open (nothing happens), event viewer contains MSIX errors regarding admin privileges required to install the app.
* User logs out and logs in to Windows or restarts the PC.
* The new package version is automatically installed including the new Windows service. The SMART screen was never shown, the new service is now running with elevated privileges.

How to reproduce:
1. Clone the https://github.com/lezzi/msix-services
2. Publish the Installer (use your own certificate or the attached test certificate with the Test@123 password).
3. Uncomment all service lines in the Package.appxmanifest.
4. Publish the Installer again, but with the incremented package version.
5. Install the old version from step #2.
6. Launch the app and click the "Update" button.
7. Select the new .msixbundle file from step #4.
8. Wait for the "Ready to restart" text to appear.
9. Close the app, attempt to launch the app again.
10. Observe the app not launching.
11. Log the current user out and log in to Windows again, or restart the PC.
12. Observe TestWindowsService running (`sc query TestWindowsService`). Launching the app shows the new version from step #4.

## Scenario #2
Update failure. A service added to the new version of the MSIX package and the package is updated using [AddPackageByUriAsync](https://learn.microsoft.com/en-us/uwp/api/windows.management.deployment.packagemanager.addpackagebyuriasync?view=winrt-22621), [ForceTargetAppShutdown](https://learn.microsoft.com/en-us/uwp/api/windows.management.deployment.registerpackageoptions.forcetargetappshutdown?view=winrt-22621) = true and under the elevated process.
* After the update, when the app is restarted, the old version of the app is opened, event viewer contains MSIX errors related to the user being logged off.

How to reproduce:
1. Clone the https://github.com/lezzi/msix-services
2. In the MainWindow.xaml.cs, comment the Scenario #1 lines and uncomment the Scenario #2. 
3. Publish the Installer (use your own certificate or the attached test certificated with the Test@123 password).
4. Uncomment all service lines in the Package.appxmanifest.
5. Publish the Installer again, but with the incremented package version.
6. Install the old version from step #3 (don't automatically launch it).
7. Launch the app as administrator and click the "Update" button.
8. Select the new .msixbundle file from step #5.
9. Wait for the app to automatically close.
10. Launch the app again.
11. Observe the app being on the old version from step #3.
