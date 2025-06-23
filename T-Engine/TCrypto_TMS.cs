using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Threading.Tasks;

namespace T_Engine
{
    public class TCrypto_TMS
    {
        public static string GenerateKey(ulong mac, string cpu_id, string due_date, int using_month, int machines, int ui)
        {
            string result;
            try
            {
                bool flag = !THwInfo_TMS.IsCpuIdValid(cpu_id);
                if (flag)
                {
                    result = null;
                }
                else
                {
                    bool flag2 = !TCrypto_TMS.IsDueDateValid(due_date);
                    if (flag2)
                    {
                        result = null;
                    }
                    else
                    {
                        string text = TCrypto_TMS.GenerateCharacterSetKey(mac);
                        List<string> key_tokens = TCrypto_TMS.EncryptEachKeys(text, mac, cpu_id, due_date, using_month, machines, ui);
                        int num = (Convert.ToInt32(due_date) + using_month + machines + ui) % TCrypto_TMS.charset_len;
                        string str = TCrypto_TMS.ShuffleKeys(num, key_tokens);
                        string text2 = text + TCrypto_TMS.Base35Encode(num, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY") + str;
                        string text3 = string.Concat(new string[]
                        {
                            text2.Substring(0, 4),
                            "-",
                            text2.Substring(4, 5),
                            "-",
                            text2.Substring(9, 5),
                            "-",
                            text2.Substring(14, 4)
                        });
                        result = text3;
                    }
                }
            }
            catch
            {
                result = null;
            }
            return result;
        }
        
        public static string GenerateKey(string mac_str, string cpu_id, string due_date, int using_month, int machines, int ui)
        {
            bool flag = !THwInfo_TMS.IsMacAddressValid(mac_str);
            string result;
            if (flag)
            {
                result = null;
            }
            else
            {
                mac_str = mac_str.Replace("-", string.Empty);
                ulong mac = Convert.ToUInt64(mac_str, 16);
                result = TCrypto_TMS.GenerateKey(mac, cpu_id, due_date, using_month, machines, ui);
            }
            return result;
        }
        
        public static string[] DecryptKey(string encryptedText)
        {
            string[] result;
            try
            {
                encryptedText = encryptedText.ToUpper();
                bool flag = !TCrypto_TMS.IsValidEncryptedKeyFormat(encryptedText);
                if (flag)
                {
                    result = null;
                }
                else
                {
                    string text = encryptedText.Replace("-", string.Empty);
                    char keysetKey = text[0];
                    char shuffleKey = text[1];
                    text = text.Substring(2);
                    List<string> encodedTextList = TCrypto_TMS.Unshuffle(shuffleKey, text);
                    List<string> list = TCrypto_TMS.DecodeEachKeys(keysetKey, encodedTextList);
                    string[] array = new string[]
                    {
                        list[0],
                        string.Concat(new string[]
                        {
                            list[1],
                            ", ",
                            list[2],
                            ", ",
                            list[3]
                        })
                    };
                    result = array;
                }
            }
            catch
            {
                result = null;
            }
            return result;
        }
        
        private static List<string> EncryptEachKeys(string characterSetKey, ulong mac, string cpu_id, string due_date, int using_month, int machines, int ui)
        {
            string text = TCrypto_TMS.Base35Encode(mac, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY");
            int num = (int)TCrypto_TMS.Base35Decode(characterSetKey, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY");
            string str = TCrypto_TMS.Base35Encode((ulong)TCrypto_TMS.Base35Decode(text.Substring(text.Length - 3), "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY"), TCrypto_TMS.rcharset.Substring(num * TCrypto_TMS.charset_len, TCrypto_TMS.charset_len)).PadLeft(3, TCrypto_TMS.padChar);
            string text2 = TCrypto_TMS.Base35Encode(cpu_id, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY");
            string str2 = TCrypto_TMS.Base35Encode((ulong)TCrypto_TMS.Base35Decode(text2.Substring(text2.Length - 2), "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY"), TCrypto_TMS.rcharset.Substring(num * TCrypto_TMS.charset_len, TCrypto_TMS.charset_len)).PadLeft(2, TCrypto_TMS.padChar);
            num = (num + 1) % TCrypto_TMS.charset_len;
            string item = TCrypto_TMS.Base35Encode(Convert.ToUInt64(due_date), TCrypto_TMS.rcharset.Substring(num * TCrypto_TMS.charset_len, TCrypto_TMS.charset_len));
            num = (num + 1) % TCrypto_TMS.charset_len;
            string item2 = TCrypto_TMS.Base35Encode(using_month, TCrypto_TMS.rcharset.Substring(num * TCrypto_TMS.charset_len, TCrypto_TMS.charset_len)).PadLeft(2, TCrypto_TMS.padChar);
            num = (num + 1) % TCrypto_TMS.charset_len;
            string item3 = TCrypto_TMS.Base35Encode(machines, TCrypto_TMS.rcharset.Substring(num * TCrypto_TMS.charset_len, TCrypto_TMS.charset_len)).PadLeft(2, TCrypto_TMS.padChar);
            num = (num + 1) % TCrypto_TMS.charset_len;
            string item4 = TCrypto_TMS.Base35Encode(ui, TCrypto_TMS.rcharset.Substring(num * TCrypto_TMS.charset_len, TCrypto_TMS.charset_len)).PadLeft(3, TCrypto_TMS.padChar);
            return new List<string>
            {
                str + str2,
                item,
                item2,
                item3,
                item4
            };
        }
        
        private static string ShuffleKeys(int shuffleKey, List<string> key_tokens)
        {
            string text = TCrypto_TMS.rshuffle.Substring(shuffleKey * 5, 5);
            string text2 = "";
            for (int i = 1; i <= 5; i++)
            {
                text2 += key_tokens[text.IndexOf(i.ToString())];
            }
            return text2;
        }
        
        private static List<string> Unshuffle(char shuffleKey, string shuffledText)
        {
            string text = TCrypto_TMS.LoadShuffleSet((int)TCrypto_TMS.Base35Decode(shuffleKey.ToString(), "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY"));
            List<string> list = new List<string>();
            for (int i = 1; i < 6; i++)
            {
                int num = TCrypto_TMS.keyLengths[text.IndexOf(i.ToString())];
                list.Add(shuffledText.Substring(0, num));
                shuffledText = shuffledText.Substring(num);
            }
            List<string> list2 = new List<string>();
            for (int j = 0; j < text.Length; j++)
            {
                list2.Add(list[TCrypto_TMS.CharToInt32(text[j]) - 1]);
            }
            return list2;
        }
        
        private static string Base35Encode(int decimalNumber, string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY")
        {
            return TCrypto_TMS.Base35Encode((ulong)((long)decimalNumber), alphabet);
        }
        
        private static string Base35Encode(string hexString, string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY")
        {
            bool flag = hexString.Length <= 0;
            string result;
            if (flag)
            {
                result = "";
            }
            else
            {
                ulong decimalNumber = Convert.ToUInt64(hexString, 16);
                result = TCrypto_TMS.Base35Encode(decimalNumber, alphabet);
            }
            return result;
        }
        
        private static string Base35Encode(ulong decimalNumber, string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY")
        {
            string text = "";
            bool flag = 0UL <= decimalNumber && decimalNumber < (ulong)((long)alphabet.Length);
            string result;
            if (flag)
            {
                result = alphabet[(int)decimalNumber].ToString();
            }
            else
            {
                BigInteger dividend = decimalNumber;
                while (!dividend.IsZero)
                {
                    BigInteger value;
                    dividend = BigInteger.DivRem(dividend, alphabet.Length, out value);
                    text = alphabet[(int)value].ToString() + text;
                }
                result = text;
            }
            return result;
        }
        
        private static long Base35Decode(string base35, string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY")
        {
            string text = base35;
            bool flag = text.Length <= 0;
            long result;
            if (flag)
            {
                result = 0L;
            }
            else
            {
                bool flag2 = false;
                bool flag3 = text.Substring(0, 1) == "-";
                if (flag3)
                {
                    flag2 = true;
                    text = text.Substring(1);
                }
                long num = 0L;
                while (1 < text.Length)
                {
                    num += (long)alphabet.IndexOf(text.Substring(0, 1));
                    text = text.Substring(1);
                    num *= 35L;
                }
                num += (long)alphabet.IndexOf(text.Substring(0, 1));
                bool flag4 = flag2;
                if (flag4)
                {
                    num = -num;
                }
                result = num;
            }
            return result;
        }
        
        private static string LoadKeySet(int idx)
        {
            return TCrypto_TMS.rcharset.Substring(idx * TCrypto_TMS.charset_len, TCrypto_TMS.charset_len);
        }
        
        private static string LoadShuffleSet(int idx)
        {
            return TCrypto_TMS.rshuffle.Substring(idx * TCrypto_TMS.keys, TCrypto_TMS.keys);
        }
        
        private static int Base35DecodeWithPadChar(string value, string set)
        {
            while (value.Length > 0)
            {
                bool flag = value[0] == 'Z';
                if (!flag)
                {
                    break;
                }
                value = value.Substring(1);
            }
            return (int)TCrypto_TMS.Base35Decode(value, set);
        }
        
        private static bool IsDueDateValid(string due_date)
        {
            DateTime dateTime;
            return DateTime.TryParseExact(due_date, "yyMMdd", null, DateTimeStyles.None, out dateTime);
        }
        
        public static bool IsValidEncryptedKeyFormat(string encryptedKey)
        {
            return THwInfo_TMS.IsStringValidForFormat(encryptedKey, "ZZZZ-ZZZZZ-ZZZZZ-ZZZZ");
        }
        
        private static List<string> DecodeEachKeys(char keysetKey, List<string> encodedTextList)
        {
            int num = (int)TCrypto_TMS.Base35Decode(keysetKey.ToString(), "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY");
            num = (num + 1) % TCrypto_TMS.charset_len;
            List<string> list = new List<string>();
            for (int i = 1; i < encodedTextList.Count; i++)
            {
                string value = encodedTextList[i];
                list.Add(TCrypto_TMS.Base35DecodeWithPadChar(value, TCrypto_TMS.LoadKeySet(num)).ToString());
                num = (num + 1) % TCrypto_TMS.charset_len;
            }
            return list;
        }
        
        private static int CharToInt32(char ch)
        {
            return Convert.ToInt32(ch.ToString());
        }
        
        private static string GenerateCharacterSetKey(ulong mac)
        {
            string text = TCrypto_TMS.Base35Encode(mac, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY");
            return TCrypto_TMS.Base35Encode((ulong)(TCrypto_TMS.Base35Decode(text.Substring(0, 4), "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY") % (long)TCrypto_TMS.charset_len), "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY");
        }
        
        private static bool IsValidHardwareInfoInKey(string strEncrypted)
        {
            List<string> hwinfoMacAddressList = THwInfo_TMS.GeTHwInfo_TMSMacAddressList();
            bool flag = hwinfoMacAddressList.Count < 1;
            bool result;
            if (flag)
            {
                result = false;
            }
            else
            {
                for (int i = 0; i < hwinfoMacAddressList.Count; i++)
                {
                    bool flag2 = TCrypto_TMS.IsValidHardwareInfoInKeyWithMacAddress(strEncrypted, hwinfoMacAddressList[i]);
                    bool flag3 = flag2;
                    if (flag3)
                    {
                        return true;
                    }
                }
                result = false;
            }
            return result;
        }
        
        private static bool IsValidHardwareInfoInKeyWithMacAddress(string strEncrypted, string strMAC)
        {
            strEncrypted = strEncrypted.ToUpper();
            string text = strEncrypted.Replace("-", string.Empty);
            string value = strMAC.Replace("-", string.Empty);
            ulong num = Convert.ToUInt64(value, 16);
            char c = text[0];
            string text2 = TCrypto_TMS.GenerateCharacterSetKey(num).ToUpper();
            bool flag = c.ToString() != text2;
            bool result;
            if (flag)
            {
                result = false;
            }
            else
            {
                int num2 = (int)TCrypto_TMS.Base35Decode(text2, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY");
                string text3 = TCrypto_TMS.Base35Encode(num, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY");
                string str = TCrypto_TMS.Base35Encode((ulong)TCrypto_TMS.Base35Decode(text3.Substring(text3.Length - 3), "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY"), TCrypto_TMS.rcharset.Substring(num2 * TCrypto_TMS.charset_len, TCrypto_TMS.charset_len)).PadLeft(3, TCrypto_TMS.padChar);
                string hwinfoCpuId = THwInfo_TMS.GeTHwInfo_TMSCpuId();
                string text4 = TCrypto_TMS.Base35Encode(hwinfoCpuId, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY");
                string str2 = TCrypto_TMS.Base35Encode((ulong)TCrypto_TMS.Base35Decode(text4.Substring(text4.Length - 2), "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY"), TCrypto_TMS.rcharset.Substring(num2 * TCrypto_TMS.charset_len, TCrypto_TMS.charset_len)).PadLeft(2, TCrypto_TMS.padChar);
                bool flag2 = text.IndexOf(str + str2) < 1;
                result = !flag2;
            }
            return result;
        }
        
        public static TCrypto_TMS.TMSLicenseInfo CheckTMSLicense(string strEncrypted)
        {
            TCrypto_TMS.TMSLicenseInfo tmslicenseInfo = new TCrypto_TMS.TMSLicenseInfo();
            try
            {
                strEncrypted = strEncrypted.ToUpper();
                bool flag = !TCrypto_TMS.IsValidEncryptedKeyFormat(strEncrypted);
                if (flag)
                {
                    throw new Exception("Invalid key format");
                }
                bool flag2 = !TCrypto_TMS.IsValidHardwareInfoInKey(strEncrypted);
                if (flag2)
                {
                    throw new Exception("Hardware info is NOT valid");
                }
                string[] array = TCrypto_TMS.DecryptKey(strEncrypted);
                bool flag3 = array == null;
                if (flag3)
                {
                    throw new Exception("decrypted string is null");
                }
                string[] array2 = array[1].Split(new char[]
                {
                    ','
                });
                string text = array[0];
                string value = array2[0].Trim();
                string value2 = array2[1].Trim();
                string value3 = array2[2].Trim();
                int num = Convert.ToInt32(value);
                int num2 = Convert.ToInt32(value2);
                int num3 = Convert.ToInt32(value3);
                bool flag4 = !TCrypto_TMS.IsDueDateValid(text);
                if (flag4)
                {
                    throw new Exception("Invalid due-date");
                }
                DateTime d = DateTime.ParseExact(text, "yyMMdd", CultureInfo.InvariantCulture).AddMonths(num);
                DateTime date = DateTime.Now.Date;
                TimeSpan timeSpan = date - d;
                int num4 = (num == 0) ? 99999 : (-1 * timeSpan.Days);
                int num5 = (timeSpan.Days < 0) ? 0 : 1;
                List<string> hwinfoMacAddressList = THwInfo_TMS.GeTHwInfo_TMSMacAddressList();
                string hwinfoCpuId = THwInfo_TMS.GeTHwInfo_TMSCpuId();
                bool flag5 = false;
                foreach (string mac_str in hwinfoMacAddressList)
                {
                    string text2 = TCrypto_TMS.GenerateKey(mac_str, hwinfoCpuId, text, num, num2, num3);
                    flag5 = text2.Equals(strEncrypted);
                    bool flag6 = flag5;
                    if (flag6)
                    {
                        break;
                    }
                }
                tmslicenseInfo.bUseTMS = true;
                tmslicenseInfo.nLeftDays = num4;
                tmslicenseInfo.strDueDate = text;
                tmslicenseInfo.nUsingMonth = num;
                tmslicenseInfo.nMachines = ((num2 == 0) ? 99999 : num2);
                tmslicenseInfo.nUI = num3;
                tmslicenseInfo.strServer = THwInfo_TMS.GeTHwInfo_TMSMacAddress();
                tmslicenseInfo.strHardware = hwinfoCpuId;
                bool flag7 = !flag5;
                if (flag7)
                {
                    throw new Exception("information of Server, Hardware, etc have problems");
                }
                bool flag8 = num4 < 0;
                if (flag8)
                {
                    throw new Exception("using month over");
                }
            }
            catch (Exception ex)
            {
                tmslicenseInfo.bUseTMS = false;
                tmslicenseInfo.strDesc = ex.Message;
            }
            return tmslicenseInfo;
        }
        
        public async Task<object> CheckTMSLicenseForServer(string strEncrypted)
        {
            return TCrypto_TMS.CheckTMSLicense(strEncrypted);
        }

        // Token: 0x04000002 RID: 2
        private static string rcharset = "JH0M7WT4QGC8SK1IBEP6OAY5LN3D29RUXVFHAI3S598GXY7RCVMBNOFKEJ0U1T42QD6PWLE4R17KGC28O6P3NB5MDWJV09TILHAUXSQYFT61WSEM3GDI4P2059ROKNYJVXLU7CHA8BFQ4L8720YNXGJ51OKEAMWTVHD9S6FUBIR3PCQ4OYTKLW6DGU9BX7CR21N0EJVAQ8I3F5HMSPWER5T1HXLY60OI4J7NS2FBQ8CGVPKAMUD93DMU0TN7VRCE4G3FX9JL2OKP8A1BW5HYSQI6A9YKXB0SHT8FGEJC1MVUD5OPRQ26IWN74L38LS3FOTRJUPKHDIQY2V0GBE965ANW17CXM42PO9EFN1RJ7ALUQY6ICHGT8XD53S0WMV4KB8A6PHWBRVTYOEQUNC439JL5XFGK0M7S2ID1IXW8QAP7DCU2H16G4O3K5SE0FTNRLVJYB9MW5QJ2TPBU0YOKG6V43C87NS9EXAILFHD1MRYLTQFO8C9D7VW2UR0AGS54H61XNBMEK3JPI196PUA7YF2OVI8H5WTXN0M4JKLCGESBR3QDKHN3YJDIC24GA7EMXP9RF1L58WTOVB0QS6UM0S1PLN54Y2EK9WBH6AXDTFJR3UCVQIO8G7PTLYC321VIUXDSRW8HEKAJNFM749BG605OQX54KR6UF8QNTE0JWMCV13HAOPD7L2GSIBY93C7RWAOSGHBE9UYK6LXD14PMNVI0Q5JFT82SJRIA2QDVU0O8453F61LXMTKHP7CB9NEGYWKREW42H03V5QGJPLA9DF7BUO8IN6CMS1XTY0BHF8VRI15A67NMT9EY3WLU4OKPGQSXDJ2CML0AB23O49PIXEF5YJH7WUD6KSTGQR81CVNJRMDSI6GAY5PB2ECQO4WL893V1N0KHUXTF7XQS7PO92ERYFULVIA3JTM80WNB5H6D14CKGQ068CKN59AHVXEBJ3WDTRUOISF4YP7G2L1ML65XUFS42RA1IJ8B3P9M7OVKQ0WNHDTECYGSXGULCHPDNKIM5T42VYEO69RW7JB8A13Q0FLQBI6EUCNAO9X284R1F5HJ307PVKGTSDMWY8ROTMKL6PF4WY3A09SEIVH7B1NCQXGJ52DUFH4SONCU6W2TG8EX1YBIKJRL7PVMQ350DA94GM3AWT0L12YNIO5HKDJXF7PVRQB86CUS9EPIBDRFE480Y27LNOK6USVHTX9Q5J1MG3WAC";

        // Token: 0x04000003 RID: 3
        private static string rshuffle = "2341514253125433245123154541321254351234531421432524351542312354121543143524125332451452132345135142243515314224531324153215431425123452431535142513422143523514432512541312435";

        // Token: 0x04000004 RID: 4
        private static char padChar = 'Z';

        // Token: 0x04000005 RID: 5
        private static int charset_len = 35;

        // Token: 0x04000006 RID: 6
        private static int[] keyLengths = new int[]
        {
            5,
            4,
            2,
            2,
            3
        };
        
        private static int keys = 5;
        
        public class TMSLicenseInfo
        {
            public TMSLicenseInfo()
            {
                this.bUseTMS = false;
                this.nLeftDays = -1;
                this.strDesc = "";
                this.strDueDate = "";
                this.nUsingMonth = -1;
                this.nMachines = -1;
                this.nUI = -1;
                this.strServer = "";
                this.strHardware = "";
                this.bUseSmartApp = false;
                this.bUseSmartRack = false;
                this.bUseSmartStorage = false;
                this.bUseSmartLink = false;
                this.bUseSmartGate = false;
                this.bUseSmartBarcode = false;
            }

            public int nUI
            {
                get
                {
                    return this._ui;
                }
                set
                {
                    this._ui = value;
                    bool flag = value == 0;
                    if (!flag)
                    {
                        int num = 0;
                        bool flag2 = (value & 1 << num) >> num == 1;
                        if (flag2)
                        {
                            this.bUseSmartApp = true;
                        }
                        num = 1;
                        bool flag3 = (value & 1 << num) >> num == 1;
                        if (flag3)
                        {
                            this.bUseSmartRack = true;
                        }
                        num = 2;
                        bool flag4 = (value & 1 << num) >> num == 1;
                        if (flag4)
                        {
                            this.bUseSmartStorage = true;
                        }
                        num = 3;
                        bool flag5 = (value & 1 << num) >> num == 1;
                        if (flag5)
                        {
                            this.bUseSmartLink = true;
                        }
                        num = 4;
                        bool flag6 = (value & 1 << num) >> num == 1;
                        if (flag6)
                        {
                            this.bUseSmartGate = true;
                        }
                        num = 5;
                        bool flag7 = (value & 1 << num) >> num == 1;
                        if (flag7)
                        {
                            this.bUseSmartBarcode = true;
                        }
                    }
                }
            }
            
            public bool bUseTMS;
            
            public int nLeftDays;
            
            public string strDesc;
            
            public TCrypto_TMS.TMSLicenseInfo.Code eCode;
            
            public string strDueDate;
            
            public int nUsingMonth;
            
            public int nMachines;
            
            private int _ui;
            
            public string strServer;
            
            public string strHardware;
            
            public bool bUseSmartApp;
            
            public bool bUseSmartRack;
            
            public bool bUseSmartStorage;
            
            public bool bUseSmartLink;
            
            public bool bUseSmartGate;
            
            public bool bUseSmartBarcode;
            
            public enum Code
            {
                OK,
                ERR_FORMAT = 1000,
                ERR_HARDWARE = 2000,
                FAIL_DECRYPT = 3000,
                USING_OVER = 4000,
                ERR_ETC = 9000
            }
        }
    }
}
