using Microsoft.Win32;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sunshine_Loader
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                string name = Utils.RandomString(Utils.RndInt());
                Console.Title = name;
                RestClient client = new RestClient(Utils.link) { Proxy = null };
                RestRequest request = new RestRequest("Auth/c.php", Method.POST);
                request.AddHeader("User-Agent", "8d6e34f987851aa599257d3831a1af040886842f");
                IRestResponse response = client.Execute(request);
                if (int.Parse(response.Content) != 1339)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write("   Username~# ");
                    Console.ForegroundColor = ConsoleColor.White;
                    string login = Console.ReadLine();
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write("   Password~# ");
                    Console.ForegroundColor = ConsoleColor.White;
                    string pass = "";
                    ConsoleKeyInfo key;
                    do
                    {
                        key = Console.ReadKey(true);
                        if (key.Key != ConsoleKey.Backspace)
                        {
                            pass += key.KeyChar;
                            Console.Write("*");
                        }
                        else
                        {
                            Console.Write("\b");
                        }
                    }
                    while (key.Key != ConsoleKey.Enter);
                    Console.WriteLine("");
                    string replas = pass.Replace("\r","");
                    Utils.Login(login, replas, Utils.GetMachineGuid());
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("   Version disabled");
                    Console.ReadLine();
                }
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   Critical error !");
                Console.ReadLine();
            }
        }
    }
    static class Cipher
    {
        private static readonly byte[] PasswordHashByte = new byte[]
        {
            74,
            56,
            106,
            103,
            116,
            55,
            56,
            57,
            74,
            72,
            104,
            55,
            84,
            56,
            117,
            106,
            56,
            84
        };
        private static readonly byte[] SaltKeyByte = new byte[]
        {
            97,
            115,
            100,
            114,
            103,
            52,
            53,
            114,
            51,
            52,
            114,
            102,
            51,
            114,
            103,
            114
        };

        public static byte[] AESEncryptBytes(byte[] clearBytes)
        {
            byte[] result = null;
            Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(PasswordHashByte, SaltKeyByte, 32768);
            using (Aes aes = new AesManaged())
            {
                aes.KeySize = 256;
                aes.Key = rfc2898DeriveBytes.GetBytes(aes.KeySize / 8);
                aes.IV = rfc2898DeriveBytes.GetBytes(aes.BlockSize / 8);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(clearBytes, 0, clearBytes.Length);
                        cryptoStream.Close();
                    }
                    result = memoryStream.ToArray();
                }
            }
            return result;
        }
        public static byte[] AESDecryptBytes(byte[] cryptBytes)
        {
            byte[] result = null;
            Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(PasswordHashByte, SaltKeyByte, 32768);
            using (Aes aes = new AesManaged())
            {
                aes.KeySize = 256;
                aes.Key = rfc2898DeriveBytes.GetBytes(aes.KeySize / 8);
                aes.IV = rfc2898DeriveBytes.GetBytes(aes.BlockSize / 8);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(cryptBytes, 0, cryptBytes.Length);
                        cryptoStream.Close();
                    }
                    result = memoryStream.ToArray();
                }
            }
            return result;
        }
    }
    static class Utils
    {
        public static string GetMachineGuid()
        {
            using (RegistryKey localMachineX64View = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (RegistryKey rk = localMachineX64View.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography"))
                {
                    return rk.GetValue("MachineGuid").ToString();
                }
            }
        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static int RndInt()
        {
            return random.Next(16, 32);
        }
        private enum Response { enable = 1337, wrong = 1338, wrong_hwid = 1339, end_days = 1400, disabled = 1401 }
        private enum Token { enable = 1337, wrong = 1338, exp = 1339 }

        public static string link = "https://krawkreverser.com.br/";
        public static void Login(string user, string pass, string hwid)
        {
            try
            {
                RestClient client = new RestClient(link) { Proxy = null };
                RestRequest request = new RestRequest("Auth/l.php", Method.POST);
                request.AddHeader("User-Agent", "8d6e34f987851aa599257d3831a1af040886842f");
                request.AddParameter("usr", user);
                request.AddParameter("pss", pass);
                request.AddParameter("hdi", hwid);
                IRestResponse response = client.Execute(request);
                switch (int.Parse(response.Content.Split('|')[0]))
                {
                    case (int)Response.enable:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(string.Format("   Logged in as {0}!", user));
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("   Testing your token('");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(response.Content.Split('|')[1]);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("').");
                        RestRequest TknCheck = new RestRequest("Auth/t.php", Method.POST);
                        TknCheck.AddHeader("User-Agent", "8d6e34f987851aa599257d3831a1af040886842f");
                        TknCheck.AddParameter("usr", user);
                        System.Threading.Thread.Sleep(500);
                        Console.Write(".");
                        TknCheck.AddParameter("tkn", response.Content.Split('|')[1]);
                        IRestResponse TknResponse = client.Execute(TknCheck);
                        System.Threading.Thread.Sleep(500);
                        Console.Write(".");
                        Console.WriteLine("");
                        switch (int.Parse(TknResponse.Content.Split('|')[0]))
                        {
                            case (int)Token.enable:
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.Write("   Decoding..");
                                byte[] dll = Cipher.AESDecryptBytes(Convert.FromBase64String(TknResponse.Content.Split('|')[1]));
                                System.Threading.Thread.Sleep(500);
                                Console.Write(".");
                                Console.WriteLine("");
                                //injection
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("   Injected With Sucessfully !");
                                break;
                            case (int)Token.wrong:
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("   Invalid token !");
                                break;
                            case (int)Token.exp:
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("   Token expired !");
                                break;
                            default:
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("   Invalid response !");
                                break;
                        }
                        break;
                    case (int)Response.wrong:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("   Invalid login !");
                        break;
                    case (int)Response.wrong_hwid:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("   Actived in another computer !");
                        break;
                    case (int)Response.end_days:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("   This login days is ended !");
                        break;
                    case (int)Response.disabled:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("   Contact admin to active your account !");
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("   Invalid response !");
                        break;
                }
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   Invalid response !");
                Console.ReadLine();
            }
            Console.ReadLine();
        }
    }
}


