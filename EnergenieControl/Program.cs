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
        public static extern int GetSocketState(IntPtr device, int socket, IntPtr state, IntPtr voltage);

        [DllImport("PMDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CloseDevice(IntPtr device);

        static void Main(string[] args)
        {
            if (args.Length != 2 || !int.TryParse(args[1], out int socketNum) || socketNum < 1 || socketNum > 4)
            {
                PrintUsage();
                return;
            }

            string action = args[0].ToLower();
            int socketIndex = socketNum - 1;

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

            if (action == "on" || action == "off")
            {
                bool turnOn = action == "on";
                int setResult = SetSocketState(device, socketIndex, turnOn);
                Console.WriteLine(setResult == 1
                    ? $"Socket {socketNum} turned {(turnOn ? "ON" : "OFF")}"
                    : $"Failed to switch socket {socketNum}");
            }
            else if (action == "status")
            {
                bool? isOn = GetSocketStatus(device, socketIndex);
                if (isOn.HasValue)
                    Console.WriteLine($"Socket {socketNum} is {(isOn.Value ? "ON" : "OFF")}");
                else
                    Console.WriteLine($"Failed to get status for socket {socketNum}");
            }
            else
            {
                PrintUsage();
            }

            CloseDevice(device);
            Marshal.FreeHGlobal(devCount);
        }

        static void PrintUsage()
        {
            Console.WriteLine("Usage: EnergenieControl.exe [on|off|status] <socket (1-4)>");
        }

        static bool? GetSocketStatus(IntPtr device, int socket)
        {
            IntPtr statePtr = Marshal.AllocHGlobal(1);   // 1 byte
            IntPtr voltPtr = Marshal.AllocHGlobal(1);    // 1 byte

            try
            {
                int result = GetSocketState(device, socket, statePtr, voltPtr);
                if (result == 1)
                {
                    byte state = Marshal.ReadByte(statePtr);
                    return state == 1;
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(statePtr);
                Marshal.FreeHGlobal(voltPtr);
            }
        }
    }
}