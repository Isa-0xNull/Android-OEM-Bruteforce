using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Bootloader.Bruteforce;
class Program
{
    private static Queue<long> s_oems = new Queue<long>();

    private const long IMEI = 867906031392244;
    private const string DEVICE_ID = "89UDU18316002011";

    private const string TOOLS = "tools";
    private const string FASTBOOT = $"{TOOLS}\\fastboot.exe";
    private const string FASTBOOT_DEVICES = $"devices";
    private const string FASTBOOT_OEM_UNLOCK = "oem unlock";
    private const string FASTBOOT_REBOOT_BOOTLOADER = "reboot bootloader";

    private const string ABD = $"{TOOLS}\\adb.exe";
    private const string ADB_DEVICES = "devices";
    private const string ADB_REBOOT_BOOTLOADER = "reboot bootloader";

    static async Task Main(string[] args)
    {
        long checksum = CheckLuhn(IMEI.ToString());
        IncrementChecksum(1000000000000000, checksum, IMEI);

        while (!(await FastbootUnlockBootloader())) ;

        while(true)
        {
            Console.ReadKey(); 
        }
    }

    private static int CheckLuhn(string imei)
    {
        int sum = 0;
        bool alternate = false;
        for (int i = imei.Length - 1; i >= 0; i--)
        {
            char[] nx = imei.ToArray();
            int n = int.Parse(nx[i].ToString());

            if (alternate)
            {
                n *= 2;

                if (n > 9)
                {
                    n = (n % 10) + 1;
                }
            }
            sum += n;
            alternate = !alternate;
        }
        return (sum % 10);
    }
    private static void IncrementChecksum(long oemCode, in long checksum, in long imei)
    {
        while (oemCode is >= 1000000000000000 and < 10000000000000000)
        {
            oemCode += (long)(checksum + Math.Sqrt(imei) * 1024);
            s_oems.Enqueue(oemCode);
        }
    }

    private static async Task AdbRebootBootloader()
    {
        while (!ADB(ADB_DEVICES).Contains(DEVICE_ID))
        {
            await Task.Delay(200);
        }

        ADB(ADB_REBOOT_BOOTLOADER);
    }

    private static async Task<bool> FastbootUnlockBootloader()
    {
        while (!Fastboot(FASTBOOT_DEVICES).Contains(DEVICE_ID))
        {
            await Task.Delay(2000);
        }

        for (int i = 0; i < 4; i++)
        {
            long currentOem = s_oems.Dequeue();
            string stdout = Fastboot($"{FASTBOOT_OEM_UNLOCK} {currentOem}");
            if (!stdout.Contains("check password failed!"))
            {
                Console.WriteLine($"Success: {currentOem}");
                return true;
            }
            else
            {
                Console.WriteLine($"Faild: {currentOem}");
            }
        }

        Fastboot(FASTBOOT_REBOOT_BOOTLOADER);
        return false;
    }

    private static string ADB(string command)
    {
        using Process proc = new Process
        {
            StartInfo = {
                FileName = ABD,
                Arguments = command,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError  = false,
            }
        };

        proc.Start();
        proc.WaitForExit();

        string stdOut = proc.StandardOutput.ReadToEnd();

        proc.Close();

        return stdOut;
    }
    private static string Fastboot(string command)
    {
        using Process proc = new Process
        {
            StartInfo = {
                FileName = FASTBOOT,
                Arguments = command,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError  = true,
            }
        };

        proc.Start();
        proc.WaitForExit();

        string stdOut = proc.StandardOutput.ReadToEnd();
        string stdErr = proc.StandardError.ReadToEnd();

        proc.Close();

        return $"{stdOut}{stdErr}";
    }
}
