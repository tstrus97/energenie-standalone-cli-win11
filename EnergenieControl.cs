using System;
using System.Runtime.InteropServices;

namespace EnergenieControl
{
    class Program
    {
        [DllImport("PMDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetDeviceList([Out] IntPtr[] devices, IntPtr devcount, IntPtr[] landevs, int landevcount, int interface_type_to_search);

        [DllImport("PMDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetSocketState(IntPtr device, int socket, bool state);

        [DllImport("PMDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CloseDevice(IntPtr device);

        static void Main(string[] args)
        {
            if (args.Length != 2 || !int.TryParse(args[1], out int socket))
            {
                Console.WriteLine("Usage: EnergenieControl.exe [on|off] <socket (0-3)>");
                return;
            }

            string action = args[0].ToLower();
            bool turnOn = action == "on";
            if (socket < 0 || socket > 3)
            {
                Console.WriteLine("Socket must be 0 to 3.");
                return;
            }

            IntPtr[] devices = new IntPtr[10];
            IntPtr devCount = Marshal.AllocHGlobal(sizeof(int));
            Marshal.WriteInt32(devCount, devices.Length);

            int result = GetDeviceList(devices, devCount, null, 0, 1); // 1 = USB only

            if (result != 1 || Marshal.ReadInt32(devCount) < 1)
            {
                Console.WriteLine("No Energenie device found.");
                Marshal.FreeHGlobal(devCount);
                return;
            }

            IntPtr device = devices[0];

            int setResult = SetSocketState(device, socket, turnOn);
            Console.WriteLine(setResult == 1
                ? $"Socket {socket + 1} turned {(turnOn ? "ON" : "OFF")}"
                : $"Failed to switch socket {socket + 1}");

            CloseDevice(device);
            Marshal.FreeHGlobal(devCount);
        }
    }
}
