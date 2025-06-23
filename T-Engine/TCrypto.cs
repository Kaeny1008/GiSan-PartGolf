using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;

namespace T_Engine
{
    public class TCrypto
    {
        public static string GenerateKey(ulong mac, string cpu_id, string due_date, int using_month, int machines, int ui)
        {
            string result;
            try
            {
                if (!THwInfo.IsCpuIdValid(cpu_id))
                {
                    result = null;
                }
                else if (!TCrypto.IsDueDateValid(due_date))
                {
                    result = null;
                }
                else
                {
                    string text = TCrypto.GenerateCharacterSetKey(mac);
                    List<string> key_tokens = TCrypto.EncryptEachKeys(text, mac, cpu_id, due_date, using_month, machines, ui);
                    int num = (Convert.ToInt32(due_date) + using_month + machines + ui) % TCrypto.charset_len;
                    string str = TCrypto.ShuffleKeys(num, key_tokens);
                    string text2 = text + TCrypto.Base35Encode(num, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY") + str;
                    result = string.Concat(new string[]
                    {
                        text2.Substring(0, 4),
                        "-",
                        text2.Substring(4, 5),
                        "-",
                        text2.Substring(9, 5),
                        "-",
                        text2.Substring(14, 4)
                    });
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
            if (!THwInfo.IsMacAddressValid(mac_str))
            {
                return null;
            }
            mac_str = mac_str.Replace("-", string.Empty);
            return TCrypto.GenerateKey(Convert.ToUInt64(mac_str, 16), cpu_id, due_date, using_month, machines, ui);
        }
        
        public static string[] DecryptKey(string encryptedText)
        {
            string[] result;
            try
            {
                encryptedText = encryptedText.ToUpper();
                if (!TCrypto.IsValidEncryptedKeyFormat(encryptedText))
                {
                    result = null;
                }
                else
                {
                    string text = encryptedText.Replace("-", string.Empty);
                    char keysetKey = text[0];
                    char shuffleKey = text[1];
                    text = text.Substring(2);
                    List<string> encodedTextList = TCrypto.Unshuffle(shuffleKey, text);
                    List<string> list = TCrypto.DecodeEachKeys(keysetKey, encodedTextList);
                    result = new string[]
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
            string text = TCrypto.Base35Encode(mac, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY");
            int num = (int)TCrypto.Base35Decode(characterSetKey, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY");
            string str = TCrypto.Base35Encode((ulong)TCrypto.Base35Decode(text.Substring(text.Length - 3), "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY"), TCrypto.rcharset.Substring(num * TCrypto.charset_len, TCrypto.charset_len)).PadLeft(3, TCrypto.padChar);
            string text2 = TCrypto.Base35Encode(cpu_id, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY");
            string str2 = TCrypto.Base35Encode((ulong)TCrypto.Base35Decode(text2.Substring(text2.Length - 2), "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY"), TCrypto.rcharset.Substring(num * TCrypto.charset_len, TCrypto.charset_len)).PadLeft(2, TCrypto.padChar);
            num = (num + 1) % TCrypto.charset_len;
            string item = TCrypto.Base35Encode(Convert.ToUInt64(due_date), TCrypto.rcharset.Substring(num * TCrypto.charset_len, TCrypto.charset_len));
            num = (num + 1) % TCrypto.charset_len;
            string item2 = TCrypto.Base35Encode(using_month, TCrypto.rcharset.Substring(num * TCrypto.charset_len, TCrypto.charset_len)).PadLeft(2, TCrypto.padChar);
            num = (num + 1) % TCrypto.charset_len;
            string item3 = TCrypto.Base35Encode(machines, TCrypto.rcharset.Substring(num * TCrypto.charset_len, TCrypto.charset_len)).PadLeft(2, TCrypto.padChar);
            num = (num + 1) % TCrypto.charset_len;
            string item4 = TCrypto.Base35Encode(ui, TCrypto.rcharset.Substring(num * TCrypto.charset_len, TCrypto.charset_len)).PadLeft(3, TCrypto.padChar);
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
            string text = TCrypto.rshuffle.Substring(shuffleKey * 5, 5);
            string text2 = "";
            for (int i = 1; i <= 5; i++)
            {
                text2 += key_tokens[text.IndexOf(i.ToString())];
            }
            return text2;
        }
        
        private static List<string> Unshuffle(char shuffleKey, string shuffledText)
        {
            string text = TCrypto.LoadShuffleSet((int)TCrypto.Base35Decode(shuffleKey.ToString(), "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY"));
            List<string> list = new List<string>();
            for (int i = 1; i < 6; i++)
            {
                int num = TCrypto.keyLengths[text.IndexOf(i.ToString())];
                list.Add(shuffledText.Substring(0, num));
                shuffledText = shuffledText.Substring(num);
            }
            List<string> list2 = new List<string>();
            for (int j = 0; j < text.Length; j++)
            {
                list2.Add(list[TCrypto.CharToInt32(text[j]) - 1]);
            }
            return list2;
        }
        
        private static string Base35Encode(int decimalNumber, string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY")
        {
            return TCrypto.Base35Encode((ulong)((long)decimalNumber), alphabet);
        }
        
        private static string Base35Encode(string hexString, string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY")
        {
            if (hexString.Length <= 0)
            {
                return "";
            }
            return TCrypto.Base35Encode(Convert.ToUInt64(hexString, 16), alphabet);
        }
        
        private static string Base35Encode(ulong decimalNumber, string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY")
        {
            string text = "";
            if (0UL <= decimalNumber && decimalNumber < (ulong)((long)alphabet.Length))
            {
                return alphabet[(int)decimalNumber].ToString();
            }
            BigInteger dividend = decimalNumber;
            while (!dividend.IsZero)
            {
                BigInteger value;
                dividend = BigInteger.DivRem(dividend, alphabet.Length, out value);
                text = alphabet[(int)value].ToString() + text;
            }
            return text;
        }
        
        private static long Base35Decode(string base35, string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY")
        {
            string text = base35;
            if (text.Length <= 0)
            {
                return 0L;
            }
            bool flag = false;
            if (text.Substring(0, 1) == "-")
            {
                flag = true;
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
            if (flag)
            {
                num = -num;
            }
            return num;
        }
        
        private static string LoadKeySet(int idx)
        {
            return TCrypto.rcharset.Substring(idx * TCrypto.charset_len, TCrypto.charset_len);
        }
        
        private static string LoadShuffleSet(int idx)
        {
            return TCrypto.rshuffle.Substring(idx * TCrypto.keys, TCrypto.keys);
        }
        
        private static int Base35DecodeWithPadChar(string value, string set)
        {
            while (value.Length > 0 && value[0] == 'Z')
            {
                value = value.Substring(1);
            }
            return (int)TCrypto.Base35Decode(value, set);
        }
        
        private static bool IsDueDateValid(string due_date)
        {
            DateTime dateTime;
            return DateTime.TryParseExact(due_date, "yyMMdd", null, DateTimeStyles.None, out dateTime);
        }
        
        public static bool IsValidEncryptedKeyFormat(string encryptedKey)
        {
            return THwInfo.IsStringValidForFormat(encryptedKey, "ZZZZ-ZZZZZ-ZZZZZ-ZZZZ");
        }
        
        private static List<string> DecodeEachKeys(char keysetKey, List<string> encodedTextList)
        {
            int num = (int)TCrypto.Base35Decode(keysetKey.ToString(), "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY");
            num = (num + 1) % TCrypto.charset_len;
            List<string> list = new List<string>();
            for (int i = 1; i < encodedTextList.Count; i++)
            {
                string value = encodedTextList[i];
                list.Add(TCrypto.Base35DecodeWithPadChar(value, TCrypto.LoadKeySet(num)).ToString());
                num = (num + 1) % TCrypto.charset_len;
            }
            return list;
        }
        
        private static int CharToInt32(char ch)
        {
            return Convert.ToInt32(ch.ToString());
        }
        
        private static string GenerateCharacterSetKey(ulong mac)
        {
            return TCrypto.Base35Encode((ulong)(TCrypto.Base35Decode(TCrypto.Base35Encode(mac, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY").Substring(0, 4), "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY") % (long)TCrypto.charset_len), "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY");
        }
        
        private static bool IsValidHardwareInfoInKey(string strEncrypted)
        {
            List<string> hwinfoMacAddressList = THwInfo.GetHWInfoMacAddressList();
            if (hwinfoMacAddressList.Count < 1)
            {
                return false;
            }
            for (int i = 0; i < hwinfoMacAddressList.Count; i++)
            {
                if (TCrypto.IsValidHardwareInfoInKeyWithMacAddress(strEncrypted, hwinfoMacAddressList[i]))
                {
                    return true;
                }
            }
            return false;
        }
        
        private static bool IsValidHardwareInfoInKeyWithMacAddress(string strEncrypted, string strMAC)
        {
            strEncrypted = strEncrypted.ToUpper();
            string text = strEncrypted.Replace("-", string.Empty);
            ulong num = Convert.ToUInt64(strMAC.Replace("-", string.Empty), 16);
            char c = text[0];
            string text2 = TCrypto.GenerateCharacterSetKey(num).ToUpper();
            if (c.ToString() != text2)
            {
                return false;
            }
            int num2 = (int)TCrypto.Base35Decode(text2, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY");
            string text3 = TCrypto.Base35Encode(num, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY");
            string str = TCrypto.Base35Encode((ulong)TCrypto.Base35Decode(text3.Substring(text3.Length - 3), "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY"), TCrypto.rcharset.Substring(num2 * TCrypto.charset_len, TCrypto.charset_len)).PadLeft(3, TCrypto.padChar);
            string text4 = TCrypto.Base35Encode(THwInfo.GetHWInfoCpuId(), "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY");
            string str2 = TCrypto.Base35Encode((ulong)TCrypto.Base35Decode(text4.Substring(text4.Length - 2), "0123456789ABCDEFGHIJKLMNOPQRSTUVWXY"), TCrypto.rcharset.Substring(num2 * TCrypto.charset_len, TCrypto.charset_len)).PadLeft(2, TCrypto.padChar);
            return text.IndexOf(str + str2) >= 1;
        }
        
        public static TCrypto.TPartLicenseInfo CheckTPartLicense(string strEncrypted)
        {
            TCrypto.TPartLicenseInfo tpartLicenseInfo = new TCrypto.TPartLicenseInfo();
            try
            {
                strEncrypted = strEncrypted.ToUpper();
                if (!TCrypto.IsValidEncryptedKeyFormat(strEncrypted))
                {
                    tpartLicenseInfo.eCode = TCrypto.TPartLicenseInfo.Code.ERR_FORMAT;
                    throw new Exception("Invalid key format");
                }
                if (!TCrypto.IsValidHardwareInfoInKey(strEncrypted))
                {
                    tpartLicenseInfo.eCode = TCrypto.TPartLicenseInfo.Code.ERR_HARDWARE;
                    throw new Exception("Hardware info is NOT valid");
                }
                string[] array = TCrypto.DecryptKey(strEncrypted);
                if (array == null)
                {
                    tpartLicenseInfo.eCode = TCrypto.TPartLicenseInfo.Code.FAIL_DECRYPT;
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
                if (!TCrypto.IsDueDateValid(text))
                {
                    tpartLicenseInfo.eCode = TCrypto.TPartLicenseInfo.Code.USING_OVER;
                    throw new Exception("Invalid due-date");
                }
                DateTime d = DateTime.ParseExact(text, "yyMMdd", CultureInfo.InvariantCulture).AddMonths(num);
                TimeSpan timeSpan = DateTime.Now.Date - d;
                int num4 = (num == 0) ? 99999 : (-1 * timeSpan.Days);
                int days = timeSpan.Days;
                List<string> hwinfoMacAddressList = THwInfo.GetHWInfoMacAddressList();
                string hwinfoCpuId = THwInfo.GetHWInfoCpuId();
                bool flag = false;
                foreach (string mac_str in hwinfoMacAddressList)
                {
                    flag = TCrypto.GenerateKey(mac_str, hwinfoCpuId, text, num, num2, num3).Equals(strEncrypted);
                    if (flag)
                    {
                        break;
                    }
                }
                tpartLicenseInfo.bUseTMS = true;
                tpartLicenseInfo.nLeftDays = num4;
                tpartLicenseInfo.strDueDate = text;
                tpartLicenseInfo.nUsingMonth = num;
                tpartLicenseInfo.nMachines = ((num2 == 0) ? 99999 : num2);
                tpartLicenseInfo.nUI = num3;
                tpartLicenseInfo.strServer = THwInfo.GetHWInfoMacAddress();
                tpartLicenseInfo.strHardware = hwinfoCpuId;
                if (!flag)
                {
                    tpartLicenseInfo.eCode = TCrypto.TPartLicenseInfo.Code.ERR_ETC;
                    throw new Exception("information of Server, Hardware, etc have problems");
                }
                if (num4 < 0)
                {
                    tpartLicenseInfo.eCode = TCrypto.TPartLicenseInfo.Code.USING_OVER;
                    throw new Exception("using month over");
                }
            }
            catch (Exception ex)
            {
                tpartLicenseInfo.eCode = TCrypto.TPartLicenseInfo.Code.ERR_ETC;
                tpartLicenseInfo.bUseTMS = false;
                tpartLicenseInfo.strDesc = ex.Message;
            }
            return tpartLicenseInfo;
        }
        
        private static string rcharset = "VRIG1WTB84L37MN6Y90SDPCOJUAE2KHF5QXLDBTRX1ISE6CONKAQ4YPH5GV923F8J0MWU7SWG3K5OMRU96I27QTX0NYB8CDF1PVAE4HJLGN6D71854AXY3UP9BCKRMS0OVHTJE2WQIFLPI2CTAL49VN35U6SXRMBW08KGJ1Q7DEOFYH0WO92148XSJ7MDUPRHYEBTKQIFGN3LAV56C0U2VLHIE1PBQ8AXY54F9STKMWCG3NJR6DO7X8VJEAUHP27FW13CL640IYOBRDNQGKMST59OSP0MDKCG8NHR75BLF623WEQJ4XYV1T9UAIJLH598OD0WNS7BGU42Y6EAQ1FIKPMRTV3CX6GK4MQBWD8IF1OASR2CHPXT3VU5JL9E0Y7NJ8CYP13GQKAT4VHI5XN2DMB7069WSLEFROUPMXF3TL7IYQESDROCB5N4901G6WJK8UVHA2CQ5I72MKRPG1D4SAXBN309HJLV6OWYU8FET2LUGNHJW9YT16D7XOESRBIQ3V5M8AFC0KP4P19FKT0M8XVIL7Q4NSWOUAEBR6Y3D25CHGJ6GFUY8H2TMCPB4E9RKI73D01QXANSV5LOJWVPJB51XHERA8UFO0Y72NK3TLCISW6DMG49QXP890YCWRSIQFN152BMED3KUGOA46V7THLJFBW3MLXQHG9UEJD6IP701NRTCYOVKSA8452TQVE9BS41M283KDY0LJFN6ICXUGPHO57RWA1PVIYXFHD7WOBMUR5T84ES623JK9G0ANLCQ5F71CREYMVAU3N0GLX4STH9POI6W2DQBK8JQJ1MEN2YIUP3F09CD4GV56OTK8RHSL7XAWB7Y0ORSXVH9QTUJ1B3DWM5FNCAKPGE42L6I8OG1LU3T8YPWB2VFK7N5JQSA94HDEIMC6XR0P5HA63FXW7NMD12EB0QGJS8ORL4IU9CTYKV5NVB9WL4GC81SFP30EHKQ27AIOYUJM6XDRTS1TQNKV7WYUB9HE2RPI0865F4J3AMOGXLCDRW7QXTKLBVG6MJ1PN8O3H5SCU9D20YFA4EIA0EQBNXF48MRT1YLC2GOD5J3IP69HUVWSK71ELUISV32R8ONH5TXAQ96WC7D0GPFJKBMY4B9IFJCEXO176G4ST8KWVYAH30UMPNR5DLQ2YKU03MC1PDL62XEO94GWSBH8VINAJ7TQRF5EBWHAKXN9231IYSOJQR70CM8GFUDVT45P6L";
        
        private static string rshuffle = "1432513254351421543253241123452315431245153422345153241215433254115243532414215312354542311543232541531422541335421412353421545132531244312513245425132514352143512434123513542";
        
        private static char padChar = 'Z';
        
        private static int charset_len = 35;
        
        private static int[] keyLengths = new int[]
        {
            5,
            4,
            2,
            2,
            3
        };
        
        private static int keys = 5;
        
        public class TPartLicenseInfo
        {
            public TPartLicenseInfo()
            {
                this.eCode = TCrypto.TPartLicenseInfo.Code.OK;
                this.bUseTMS = false;
                this.nLeftDays = -1;
                this.strDesc = "";
                this.strDueDate = "";
                this.nUsingMonth = -1;
                this.nMachines = -1;
                this.nUI = -1;
                this.strServer = "";
                this.strHardware = "";
                this.bUseTPart = false;
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
                    if (value == 0)
                    {
                        return;
                    }
                    if (value == 1010)
                    {
                        this.bUseTPart = true;
                    }
                }
            }
            
            public const int TPartFeature = 1010;
            
            public bool bUseTMS;
            
            public int nLeftDays;
            
            public string strDesc;
            
            public TCrypto.TPartLicenseInfo.Code eCode;
            
            public string strDueDate;
            
            public int nUsingMonth;
            
            public int nMachines;
            
            private int _ui;
            
            public string strServer;
            
            public string strHardware;
            
            public bool bUseTPart;
            
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
