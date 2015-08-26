using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace PTL.Tools
{
    public static class SystemInfo
    {
        // Finds the MAC address
        public static string GetMacAddress()
        {
            string macAddresses = "";

            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                if ((bool)mo["IPEnabled"] == true)
                {
                    macAddresses = mo["MacAddress"].ToString();
                    break;
                }
            }

            return macAddresses;

        }

        // Finds the Cpu ID
        public static string GetCpuID()
        {

            //獲取cpu序列號
            string cpuInfo = "";//cpu序列號

            ManagementClass mc = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
            }

            return cpuInfo;

        }
    }
}
