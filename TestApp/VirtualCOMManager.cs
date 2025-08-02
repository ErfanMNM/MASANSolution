using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;

namespace TestApp
{
    public class VirtualCOMManager
    {
        // Windows API cho Device Management
        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern IntPtr SetupDiCreateDeviceInfoList(
            ref Guid ClassGuid,
            IntPtr hwndParent);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool SetupDiCreateDeviceInfo(
            IntPtr DeviceInfoSet,
            string DeviceName,
            ref Guid ClassGuid,
            string DeviceDescription,
            IntPtr hwndParent,
            uint CreationFlags,
            ref SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiCallClassInstaller(
            uint InstallFunction,
            IntPtr DeviceInfoSet,
            ref SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

        [StructLayout(LayoutKind.Sequential)]
        private struct SP_DEVINFO_DATA
        {
            public uint cbSize;
            public Guid ClassGuid;
            public uint DevInst;
            public IntPtr Reserved;
        }

        // GUID for Ports (COM & LPT) class
        private static readonly Guid GUID_DEVCLASS_PORTS = new Guid("4D36E978-E325-11CE-BFC1-08002BE10318");

        public static class VirtualCOMRegistry
        {
            public static bool CreateVirtualCOMPair(string com1, string com2)
            {
                try
                {
                    // Method 1: Create using Registry entries
                    if (CreateRegistryEntries(com1, com2))
                    {
                        return true;
                    }

                    // Method 2: Create using Loop back driver
                    return CreateLoopbackDriver(com1, com2);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error creating virtual COM pair: {ex.Message}");
                    return false;
                }
            }

            private static bool CreateRegistryEntries(string com1, string com2)
            {
                try
                {
                    // Tạo registry entries cho virtual COM ports
                    string regPath = @"SYSTEM\CurrentControlSet\Enum\Root\PORTS";
                    
                    using (RegistryKey key = Registry.LocalMachine.CreateSubKey(regPath))
                    {
                        CreateCOMRegistryEntry(key, com1, "Virtual COM Port 1");
                        CreateCOMRegistryEntry(key, com2, "Virtual COM Port 2");
                    }

                    // Refresh device list
                    RefreshDeviceList();
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            private static void CreateCOMRegistryEntry(RegistryKey parentKey, string portName, string description)
            {
                string deviceId = $"VCP_{portName}_{Guid.NewGuid():N}";
                
                using (RegistryKey deviceKey = parentKey.CreateSubKey(deviceId))
                {
                    deviceKey.SetValue("DeviceDesc", description);
                    deviceKey.SetValue("Class", "Ports");
                    deviceKey.SetValue("ClassGUID", "{4D36E978-E325-11CE-BFC1-08002BE10318}");
                    deviceKey.SetValue("FriendlyName", $"{description} ({portName})");
                    
                    using (RegistryKey paramKey = deviceKey.CreateSubKey("Device Parameters"))
                    {
                        paramKey.SetValue("PortName", portName);
                    }
                }
            }

            private static bool CreateLoopbackDriver(string com1, string com2)
            {
                try
                {
                    // Tạo file driver đơn giản
                    string driverPath = CreateSimpleLoopbackDriver();
                    
                    if (!string.IsNullOrEmpty(driverPath))
                    {
                        // Install driver
                        return InstallLoopbackDriver(driverPath, com1, com2);
                    }
                    
                    return false;
                }
                catch
                {
                    return false;
                }
            }

            private static string CreateSimpleLoopbackDriver()
            {
                try
                {
                    string tempPath = Path.GetTempPath();
                    string infFile = Path.Combine(tempPath, "VirtualCOM.inf");
                    
                    string infContent = @"[Version]
Signature=""$Windows NT$""
Class=Ports
ClassGuid={4D36E978-E325-11CE-BFC1-08002BE10318}
Provider=VirtualCOM
DriverVer=01/01/2024,1.0.0.0

[Manufacturer]
VirtualCOM=VirtualCOM.Mfg,NTx86,NTamd64

[VirtualCOM.Mfg.NTx86]
%VirtualCOM.DeviceDesc%=VirtualCOM_Device,VirtualCOM\Port

[VirtualCOM.Mfg.NTamd64]
%VirtualCOM.DeviceDesc%=VirtualCOM_Device,VirtualCOM\Port

[VirtualCOM_Device.NT]
CopyFiles=VirtualCOM.CopyFiles

[VirtualCOM.CopyFiles]

[VirtualCOM_Device.NT.Services]
AddService=VirtualCOM,0x00000002,VirtualCOM_Service_Inst

[VirtualCOM_Service_Inst]
DisplayName=%VirtualCOM.SvcDesc%
ServiceType=1
StartType=3
ErrorControl=1
ServiceBinary=%12%\null.sys

[Strings]
VirtualCOM.DeviceDesc=""Virtual COM Port""
VirtualCOM.SvcDesc=""Virtual COM Port Service""
";
                    
                    File.WriteAllText(infFile, infContent);
                    return infFile;
                }
                catch
                {
                    return null;
                }
            }

            private static bool InstallLoopbackDriver(string infPath, string com1, string com2)
            {
                try
                {
                    // Sử dụng PnPUtil để install driver
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "pnputil.exe",
                        Arguments = $"/add-driver \"{infPath}\" /install",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        Verb = "runas" // Require admin
                    };

                    using (Process process = Process.Start(psi))
                    {
                        process.WaitForExit();
                        return process.ExitCode == 0;
                    }
                }
                catch
                {
                    return false;
                }
            }

            private static void RefreshDeviceList()
            {
                try
                {
                    // Refresh device manager
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "devcon.exe",
                        Arguments = "rescan",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using (Process process = Process.Start(psi))
                    {
                        process?.WaitForExit();
                    }
                }
                catch
                {
                    // Fallback: Use WMI to refresh
                    try
                    {
                        ManagementClass mc = new ManagementClass("Win32_PnPEntity");
                        foreach (ManagementObject mo in mc.GetInstances())
                        {
                            mo.InvokeMethod("Reset", null);
                        }
                    }
                    catch { }
                }
            }

            public static bool RemoveVirtualCOMPair(string com1, string com2)
            {
                try
                {
                    // Remove registry entries
                    string regPath = @"SYSTEM\CurrentControlSet\Enum\Root\PORTS";
                    
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(regPath, true))
                    {
                        if (key != null)
                        {
                            foreach (string subKeyName in key.GetSubKeyNames())
                            {
                                if (subKeyName.StartsWith("VCP_" + com1) || subKeyName.StartsWith("VCP_" + com2))
                                {
                                    key.DeleteSubKeyTree(subKeyName);
                                }
                            }
                        }
                    }

                    RefreshDeviceList();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        // Simpler approach: Memory-based virtual COM emulation
        public static class MemoryBasedCOM
        {
            private static Dictionary<string, VirtualCOMPortMemory> _virtualPorts = 
                new Dictionary<string, VirtualCOMPortMemory>();

            public static bool CreateVirtualCOMPair(string com1, string com2)
            {
                try
                {
                    var port1 = new VirtualCOMPortMemory(com1);
                    var port2 = new VirtualCOMPortMemory(com2);

                    // Cross-connect the ports
                    port1.ConnectedPort = port2;
                    port2.ConnectedPort = port1;

                    _virtualPorts[com1] = port1;
                    _virtualPorts[com2] = port2;

                    return true;
                }
                catch
                {
                    return false;
                }
            }

            public static VirtualCOMPortMemory GetPort(string portName)
            {
                return _virtualPorts.ContainsKey(portName) ? _virtualPorts[portName] : null;
            }

            public static void RemoveVirtualCOMPair(string com1, string com2)
            {
                _virtualPorts.Remove(com1);
                _virtualPorts.Remove(com2);
            }

            public static List<string> GetAvailableVirtualPorts()
            {
                return new List<string>(_virtualPorts.Keys);
            }
        }

        public class VirtualCOMPortMemory
        {
            public string PortName { get; private set; }
            public VirtualCOMPortMemory ConnectedPort { get; set; }
            public bool IsOpen { get; private set; }

            private Queue<byte[]> _dataQueue = new Queue<byte[]>();
            private readonly object _lock = new object();

            public event EventHandler<byte[]> DataReceived;

            public VirtualCOMPortMemory(string portName)
            {
                PortName = portName;
            }

            public bool Open()
            {
                IsOpen = true;
                return true;
            }

            public void Close()
            {
                IsOpen = false;
            }

            public void Write(byte[] data)
            {
                if (!IsOpen || ConnectedPort == null) return;

                // Send data to connected port
                ConnectedPort.ReceiveData(data);
            }

            private void ReceiveData(byte[] data)
            {
                lock (_lock)
                {
                    _dataQueue.Enqueue(data);
                }
                
                DataReceived?.Invoke(this, data);
            }

            public byte[] Read()
            {
                lock (_lock)
                {
                    return _dataQueue.Count > 0 ? _dataQueue.Dequeue() : null;
                }
            }
        }

        // Registry-less approach: Create virtual ports in user space
        public static List<string> CreateRegistrylessVirtualCOM(int count = 4)
        {
            List<string> createdPorts = new List<string>();

            try
            {
                for (int i = 1; i <= count; i += 2)
                {
                    string com1 = $"VCOM{i}";
                    string com2 = $"VCOM{i + 1}";

                    if (MemoryBasedCOM.CreateVirtualCOMPair(com1, com2))
                    {
                        createdPorts.Add(com1);
                        createdPorts.Add(com2);
                    }
                }

                return createdPorts;
            }
            catch
            {
                return createdPorts;
            }
        }

        public static bool IsAdministrator()
        {
            try
            {
                var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
                var principal = new System.Security.Principal.WindowsPrincipal(identity);
                return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }
    }
}