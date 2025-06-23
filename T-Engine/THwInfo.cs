using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Text;

namespace T_Engine
{
    public class THwInfo
    {
        public static string GenerateAccessTokenFor(string macAddress, string cpuId)
        {
            string result;
            try
            {
                macAddress = macAddress.ToUpper();
                if (!THwInfo.IsMacAddressValid(macAddress))
                {
                    result = null;
                }
                else
                {
                    cpuId = cpuId.ToUpper();
                    if (!THwInfo.IsCpuIdValid(cpuId))
                    {
                        result = null;
                    }
                    else
                    {
                        string textToEncrypt = "Info A: " + macAddress + "\nInfo B: " + cpuId;
                        string key = "sujeong.daekyun.changhwan.dekhan.heesung";
                        result = THwInfo.Encrypt(textToEncrypt, key);
                    }
                }
            }
            catch
            {
                result = null;
            }
            return result;
        }
        
        public static string GenerateAccessTokenForThisPc()
        {
            string hwinfoMacAddress = THwInfo.GetHWInfoMacAddress();
            string hwinfoCpuId = THwInfo.GetHWInfoCpuId();
            return THwInfo.GenerateAccessTokenFor(hwinfoMacAddress, hwinfoCpuId);
        }
        
        public static string GetHWInfoMacAddress()
        {
            string result;
            try
            {
                List<string> hwinfoMacAddressList = THwInfo.GetHWInfoMacAddressList();
                if (hwinfoMacAddressList.Count < 1)
                {
                    result = null;
                }
                else
                {
                    result = hwinfoMacAddressList[0];
                }
            }
            catch
            {
                result = null;
            }
            return result;
        }
        
        public static List<string> GetHWInfoMacAddressList()
        {
            List<string> list = new List<string>();
            List<string> result;
            try
            {
                list = THwInfo.GetMacAddressListByWmiQuery();
                for (int i = 0; i < list.Count; i++)
                {
                    list[i] = list[i].Replace(':', '-');
                }
                result = list;
            }
            catch
            {
                result = list;
            }
            return result;
        }
        
        public static string GetHWInfoCpuId()
        {
            string result;
            try
            {
                result = THwInfo.GetCpuIdByWmiQuery();
            }
            catch
            {
                result = null;
            }
            return result;
        }
        
        protected static string GetFirstValueString(string multiLineString)
        {
            string text = multiLineString;
            if (text.IndexOf('\n') > -1)
            {
                text = text.Substring(text.IndexOf('\n') + 1);
            }
            if (text.IndexOf(' ') > -1)
            {
                text = text.Substring(0, text.IndexOf(' '));
            }
            return text;
        }
        
        protected static string Encrypt(string textToEncrypt, string key)
        {
            ARIAEngine ariaengine = new ARIAEngine(256);
            ariaengine.SetKey(Encoding.UTF8.GetBytes(key.Substring(0, 32)));
            return ariaengine.SetEncForSWLicense(textToEncrypt);
        }
        
        public static bool IsMacAddressValid(string macAddress)
        {
            return THwInfo.IsStringValidForFormat(macAddress, "XX-XX-XX-XX-XX-XX");
        }
        
        public static bool IsCpuIdValid(string cpuId)
        {
            return THwInfo.IsStringValidForFormat(cpuId, "XXXXXXXXXXXXXXXX");
        }
        
        private static string ExecuteWmic(string wmicCommandString)
        {
            string result;
            try
            {
                Process process = Process.Start(new ProcessStartInfo("wmic")
                {
                    Arguments = wmicCommandString,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                });
                int milliseconds = 20000;
                if (!process.WaitForExit(milliseconds))
                {
                    result = null;
                }
                else
                {
                    result = process.StandardOutput.ReadToEnd();
                }
            }
            catch
            {
                result = null;
            }
            return result;
        }
        
        private static string GetCpuIdByWmiQuery()
        {
            string text = null;
            string result;
            try
            {
                new ManagementScope();
                ManagementScope managementScope = new ManagementScope("\\\\localhost\\root\\CIMV2");
                managementScope.Connect();
                SelectQuery query = new SelectQuery("SELECT * FROM Win32_Processor");
                using (ManagementObjectCollection managementObjectCollection = new ManagementObjectSearcher(managementScope, query).Get())
                {
                    foreach (ManagementBaseObject managementBaseObject in managementObjectCollection)
                    {
                        ManagementObject managementObject = (ManagementObject)managementBaseObject;
                        text = string.Format("{0}", managementObject["ProcessorId"]);
                    }
                }
                result = text;
            }
            catch
            {
                result = null;
            }
            return result;
        }
        
        private static string GetMacAddressByWmiQuery()
        {
            string text = null;
            string result;
            try
            {
                new ManagementScope();
                ManagementScope managementScope = new ManagementScope("\\\\localhost\\root\\CIMV2");
                managementScope.Connect();
                SelectQuery query = new SelectQuery("SELECT * FROM Win32_NetworkAdapter where (AdapterTypeID=0 and netconnectionstatus=2 and not netconnectionid like '%Virtual%' and netenabled=true)");
                using (ManagementObjectCollection managementObjectCollection = new ManagementObjectSearcher(managementScope, query).Get())
                {
                    foreach (ManagementBaseObject managementBaseObject in managementObjectCollection)
                    {
                        ManagementObject managementObject = (ManagementObject)managementBaseObject;
                        text = string.Format("{0}", managementObject["MACAddress"]);
                    }
                }
                result = text;
            }
            catch
            {
                result = null;
            }
            return result;
        }
        
        private static List<string> GetMacAddressListByWmiQuery()
        {
            List<string> list = new List<string>();
            List<string> result;
            try
            {
                new ManagementScope();
                ManagementScope managementScope = new ManagementScope("\\\\localhost\\root\\CIMV2");
                managementScope.Connect();
                SelectQuery query = new SelectQuery("SELECT * FROM Win32_NetworkAdapter where (AdapterTypeID=0 and not netconnectionid like '%Virtual%' and not netconnectionid like '%Bluetooth%')");
                using (ManagementObjectCollection managementObjectCollection = new ManagementObjectSearcher(managementScope, query).Get())
                {
                    foreach (ManagementBaseObject managementBaseObject in managementObjectCollection)
                    {
                        ManagementObject managementObject = (ManagementObject)managementBaseObject;
                        list.Add(string.Format("{0}", managementObject["MACAddress"]));
                        string.Format("{0}", managementObject["AdapterType"]);
                        string.Format("{0}", managementObject["Caption"]);
                        string.Format("{0}", managementObject["Manufacturer"]);
                        string.Format("{0}", managementObject["Status"]);
                        string.Format("{0}", managementObject["AdapterTypeID"]);
                        string.Format("{0}", managementObject["NetConnectionID"]);
                        string.Format("{0}", managementObject["NetConnectionStatus"]);
                        string.Format("{0}", managementObject["Installed"]);
                        string.Format("{0}", managementObject["Availability"]);
                        string.Format("{0}", managementObject["NetEnabled"]);
                    }
                }
                result = list;
            }
            catch
            {
                result = null;
            }
            return result;
        }
        
        public static bool IsStringValidForFormat(string input, string format)
        {
            if (format == null)
            {
                return false;
            }
            if (input == null)
            {
                return false;
            }
            if (input.Length != format.Length)
            {
                return false;
            }
            for (int i = 0; i < format.Length; i++)
            {
                char c = format[i];
                if (c != '-')
                {
                    if (c == 'X')
                    {
                        try
                        {
                            Convert.ToInt32(input[i].ToString(), 16);
                            goto IL_75;
                        }
                        catch
                        {
                            return false;
                        }
                        return false;
                    }
                    if (c == 'Z')
                    {
                        if (!char.IsLetterOrDigit(input[i]))
                        {
                            return false;
                        }
                        goto IL_75;
                    }
                    return false;
                }
                if (input[i] != '-')
                {
                    return false;
                }
            IL_75:;
            }
            return true;
        }
        
        public const bool IS_FAST_TEST = false;
    }
}
