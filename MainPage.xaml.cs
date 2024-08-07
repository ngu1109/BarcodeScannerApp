// MainPage.xaml.cs
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BarcodeScannerApp
{
    public sealed partial class MainPage : Page
    {
        private DeviceInformationCollection deviceCollection;
        private DeviceWatcher deviceWatcher;
        private HidDevice selectedDevice;

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Start device watcher to list HID devices (including barcode scanners)
            deviceWatcher = DeviceInformation.CreateWatcher(DeviceClass.HumanInterfaceDevice);
            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Start();
        }

        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                DeviceListBox.Items.Add(args);
            });
        }

        private async void SelectDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            if (DeviceListBox.SelectedItem is DeviceInformation deviceInfo)
            {
                selectedDevice = await HidDevice.FromIdAsync(deviceInfo.Id, Windows.Storage.FileAccessMode.Read);
                if (selectedDevice != null)
                {
                    selectedDevice.InputReportReceived += SelectedDevice_InputReportReceived;
                }
            }
        }

        private async void SelectedDevice_InputReportReceived(HidDevice sender, HidInputReportReceivedEventArgs args)
        {
            var buffer = args.Report.Data;
            string barcodeData = ""; // Decode buffer to string as per your barcode scanner's encoding

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                BarcodeTextBox.Text += barcodeData;
            });
        }
    }
}
