using System;
using System.Windows;
using Windows.Management.Deployment;
using Microsoft.Win32;

namespace DesktopClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            try
            {
                var currentPackage = Windows.ApplicationModel.Package.Current;
                var version = currentPackage.Id.Version;

                VersionTextBlock.Text = $"{version.Major}.{version.Minor}.{version.Revision}.{version.Build}";
            }
            catch (InvalidOperationException)
            {

            }
        }

        private async void UpdateButton_OnClick(object sender, RoutedEventArgs e)
        {
            var filePicker = new OpenFileDialog { Filter = "MSIX bundle|*.msixbundle" };
            if (filePicker.ShowDialog() != true)
                return;
            
            var bundlePath = filePicker.FileName;
            var packageManager = new PackageManager();
            
            // Scenario #1
            await packageManager.AddPackageByUriAsync(new Uri(bundlePath, UriKind.Absolute), new AddPackageOptions
            {
                DeferRegistrationWhenPackagesAreInUse = true,
                ForceTargetAppShutdown = false
            });
            
            // Scenario #2
            // await packageManager.AddPackageByUriAsync(new Uri(bundlePath, UriKind.Absolute), new AddPackageOptions
            // {
            //     DeferRegistrationWhenPackagesAreInUse = false,
            //     ForceTargetAppShutdown = true
            // });

            ResultTextBlock.Text = "Ready to restart";
        }
    }
}
