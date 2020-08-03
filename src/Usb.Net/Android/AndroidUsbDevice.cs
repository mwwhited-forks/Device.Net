﻿using Device.Net;
using Microsoft.Extensions.Logging;
using System;

namespace Usb.Net.Android
{
    [Obsolete(Messages.ObsoleteMessagePlatformSpecificUsbDevice)]
    public class AndroidUsbDevice : UsbDevice
    {
        public AndroidUsbDevice(AndroidUsbInterfaceManager androidUsbInterfaceManager, ILogger logger, ITracer tracer) : base(androidUsbInterfaceManager.DeviceNumberId.ToString(), androidUsbInterfaceManager, logger, tracer)
        {
            if (androidUsbInterfaceManager == null) throw new ArgumentNullException(nameof(androidUsbInterfaceManager));
        }
    }
}