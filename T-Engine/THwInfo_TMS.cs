using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace T_Engine
{
    public class THwInfo_TMS
    {
        public static string GenerateAccessTokenFor(string macAddress, string cpuId)
        {
            string result;
            try
            {
                macAddress = macAddress.ToUpper();
                bool flag = !THwInfo_TMS.IsMacAddressValid(macAddress);
                if (flag)
                {
                    result = null;
                }
                else
                {
                    cpuId = cpuId.ToUpper();
                    bool flag2 = !THwInfo_TMS.IsCpuIdValid(cpuId);
                    if (flag2)
                    {
                        result = null;
                    }
                    else
                    {
                        string textToEncrypt = "Info A: " + macAddress + "\nInfo B: " + cpuId;
                        string text = "hyunjin.jeongpil.minseok.yeonmin";
                        string text2 = THwInfo_TMS.Encrypt(textToEncrypt, text.Substring(0, 16));
                        result = text2;
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
            string hwinfoMacAddress = THwInfo_TMS.GeTHwInfo_TMSMacAddress();
            string hwinfoCpuId = THwInfo_TMS.GeTHwInfo_TMSCpuId();
            return THwInfo_TMS.GenerateAccessTokenFor(hwinfoMacAddress, hwinfoCpuId);
        }
        
        public static string GeTHwInfo_TMSMacAddress()
        {
            string result;
            try
            {
                List<string> hwinfoMacAddressList = THwInfo_TMS.GeTHwInfo_TMSMacAddressList();
                bool flag = hwinfoMacAddressList.Count < 1;
                if (flag)
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
        
        public static List<string> GeTHwInfo_TMSMacAddressList()
        {
            List<string> list = new List<string>();
            List<string> result;
            try
            {
                list = THwInfo_TMS.GetMacAddressListByWmiQuery();
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
        
        public static string GeTHwInfo_TMSCpuId()
        {
            string result;
            try
            {
                string cpuIdByWmiQuery = THwInfo_TMS.GetCpuIdByWmiQuery();
                result = cpuIdByWmiQuery;
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
            bool flag = text.IndexOf('\n') > -1;
            if (flag)
            {
                text = text.Substring(text.IndexOf('\n') + 1);
            }
            bool flag2 = text.IndexOf(' ') > -1;
            if (flag2)
            {
                text = text.Substring(0, text.IndexOf(' '));
            }
            return text;
        }
        
        protected static string Encrypt(string textToEncrypt, string key)
        {
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            rijndaelManaged.Mode = CipherMode.CBC;
            rijndaelManaged.Padding = PaddingMode.PKCS7;
            rijndaelManaged.KeySize = 128;
            rijndaelManaged.BlockSize = 128;
            byte[] bytes = Encoding.UTF8.GetBytes(key);
            byte[] array = new byte[16];
            int num = bytes.Length;
            bool flag = num > array.Length;
            if (flag)
            {
                num = array.Length;
            }
            Array.Copy(bytes, array, num);
            rijndaelManaged.Key = array;
            rijndaelManaged.IV = array;
            ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor();
            byte[] bytes2 = Encoding.UTF8.GetBytes(textToEncrypt);
            return Convert.ToBase64String(cryptoTransform.TransformFinalBlock(bytes2, 0, bytes2.Length));
        }
        
        public static bool IsMacAddressValid(string macAddress)
        {
            return THwInfo_TMS.IsStringValidForFormat(macAddress, "XX-XX-XX-XX-XX-XX");
        }
        
        public static bool IsCpuIdValid(string cpuId)
        {
            return THwInfo_TMS.IsStringValidForFormat(cpuId, "XXXXXXXXXXXXXXXX");
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
                bool flag = process.WaitForExit(milliseconds);
                bool flag2 = !flag;
                if (flag2)
                {
                    result = null;
                }
                else
                {
                    string text = process.StandardOutput.ReadToEnd();
                    result = text;
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
                ManagementScope managementScope = new ManagementScope();
                managementScope = new ManagementScope("\\\\localhost\\root\\CIMV2");
                managementScope.Connect();
                SelectQuery query = new SelectQuery("SELECT * FROM Win32_Processor");
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(managementScope, query);
                using (ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get())
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
                ManagementScope managementScope = new ManagementScope();
                managementScope = new ManagementScope("\\\\localhost\\root\\CIMV2");
                managementScope.Connect();
                SelectQuery query = new SelectQuery("SELECT * FROM Win32_NetworkAdapter where (AdapterTypeID=0 and netconnectionstatus=2 and not netconnectionid like '%Virtual%' and netenabled=true)");
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(managementScope, query);
                using (ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get())
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
                ManagementScope managementScope = new ManagementScope();
                managementScope = new ManagementScope("\\\\localhost\\root\\CIMV2");
                managementScope.Connect();
                SelectQuery query = new SelectQuery("SELECT * FROM Win32_NetworkAdapter where (AdapterTypeID=0 and not netconnectionid like '%Virtual%' and not netconnectionid like '%Bluetooth%')");
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(managementScope, query);
                using (ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get())
                {
                    foreach (ManagementBaseObject managementBaseObject in managementObjectCollection)
                    {
                        ManagementObject managementObject = (ManagementObject)managementBaseObject;
                        list.Add(string.Format("{0}", managementObject["MACAddress"]));
                        string text = string.Format("{0}", managementObject["AdapterType"]);
                        string text2 = string.Format("{0}", managementObject["Caption"]);
                        string text3 = string.Format("{0}", managementObject["Manufacturer"]);
                        string text4 = string.Format("{0}", managementObject["Status"]);
                        string text5 = string.Format("{0}", managementObject["AdapterTypeID"]);
                        string text6 = string.Format("{0}", managementObject["NetConnectionID"]);
                        string text7 = string.Format("{0}", managementObject["NetConnectionStatus"]);
                        string text8 = string.Format("{0}", managementObject["Installed"]);
                        string text9 = string.Format("{0}", managementObject["Availability"]);
                        string text10 = string.Format("{0}", managementObject["NetEnabled"]);
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
            bool flag = format == null;
            bool result;
            if (flag)
            {
                result = false;
            }
            else
            {
                bool flag2 = input == null;
                if (flag2)
                {
                    result = false;
                }
                else
                {
                    bool flag3 = input.Length != format.Length;
                    if (flag3)
                    {
                        result = false;
                    }
                    else
                    {
                        for (int i = 0; i < format.Length; i++)
                        {
                            char c = format[i];
                            if (c != '-')
                            {
                                if (c != 'X')
                                {
                                    if (c != 'Z')
                                    {
                                        return false;
                                    }
                                    bool flag4 = !char.IsLetterOrDigit(input[i]);
                                    if (flag4)
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        int num = Convert.ToInt32(input[i].ToString(), 16);
                                    }
                                    catch
                                    {
                                        return false;
                                    }
                                }
                            }
                            else
                            {
                                bool flag5 = input[i] != '-';
                                if (flag5)
                                {
                                    return false;
                                }
                            }
                        }
                        result = true;
                    }
                }
            }
            return result;
        }
        
        public const bool IS_FAST_TEST = false;
    }
}
