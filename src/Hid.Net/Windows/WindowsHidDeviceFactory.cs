﻿using Device.Net;
using Device.Net.Windows;
using Microsoft.Win32.SafeHandles;
using System;
using static Hid.Net.Windows.HidAPICalls;

namespace Hid.Net.Windows
{
    public class WindowsHidDeviceFactory : WindowsDeviceFactoryBase, IDeviceFactory
    {
        #region Public Override Properties
        public override DeviceType DeviceType => DeviceType.Hid;
        #endregion

        public byte? DefaultReportId { get; }

        public WindowsHidDeviceFactory(byte? defaultReportId)
        {
            DefaultReportId = defaultReportId;
        }

        #region Protected Override Methods
        protected override ConnectedDeviceDefinition GetDeviceDefinition(string deviceId)
        {
            try
            {
                using (var safeFileHandle = APICalls.CreateFile(deviceId, APICalls.GenericRead | APICalls.GenericWrite, APICalls.FileShareRead | APICalls.FileShareWrite, IntPtr.Zero, APICalls.OpenExisting, 0, IntPtr.Zero))
                {
                    return GetDeviceDefinition(deviceId, safeFileHandle);
                }
            }
            catch (Exception ex)
            {
                Logger?.Log($"{nameof(GetDeviceDefinition)} error. Device Id: {deviceId}", nameof(WindowsHidDeviceFactory), ex, LogLevel.Error);
                return null;
            }
        }

        protected override Guid GetClassGuid()
        {
            return GetHidGuid();
        }

        #endregion

        #region Constructor
        public WindowsHidDeviceFactory(ILogger logger, ITracer tracer) : base(logger, tracer)
        {

        }
        #endregion

        #region Public Methods
        public IDevice GetDevice(ConnectedDeviceDefinition deviceDefinition)
        {
            if (deviceDefinition == null) throw new ArgumentNullException(nameof(deviceDefinition));

            return deviceDefinition.DeviceType != DeviceType ? null : new WindowsHidDevice(deviceDefinition.DeviceId, Logger, Tracer);
        }
        #endregion

        #region Private Static Methods
        public static ConnectedDeviceDefinition GetDeviceDefinition(string deviceId, SafeFileHandle safeFileHandle)
        {
            var hidAttributes = GetHidAttributes(safeFileHandle);
            var hidCollectionCapabilities = GetHidCapabilities(safeFileHandle);
            var manufacturer = GetManufacturer(safeFileHandle);
            var serialNumber = GetSerialNumber(safeFileHandle);
            var product = GetProduct(safeFileHandle);

            return new ConnectedDeviceDefinition(deviceId)
            {
                WriteBufferSize = hidCollectionCapabilities.OutputReportByteLength,
                ReadBufferSize = hidCollectionCapabilities.InputReportByteLength,
                Manufacturer = manufacturer,
                ProductName = product,
                ProductId = (ushort)hidAttributes.ProductId,
                SerialNumber = serialNumber,
                Usage = hidCollectionCapabilities.Usage,
                UsagePage = hidCollectionCapabilities.UsagePage,
                VendorId = (ushort)hidAttributes.VendorId,
                VersionNumber = (ushort)hidAttributes.VersionNumber,
                DeviceType = DeviceType.Hid
            };
        }
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Register the factory for enumerating Hid devices on UWP. 
        /// </summary>
        public static void Register(ILogger logger, ITracer tracer)
        {
            DeviceManager.Current.DeviceFactories.Add(new WindowsHidDeviceFactory(logger, tracer));
        }
        public static void Register(ILogger logger, byte? defaultReportId)
        #endregion
    }
}
