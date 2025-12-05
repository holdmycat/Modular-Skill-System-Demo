//------------------------------------------------------------
// File: GlobalHelper.cs
// Created: 2025-11-29
// Purpose: Utility helpers for file paths, time, networking, math, and editor tooling.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System.Collections.Generic;
//using Cysharp.Threading.Tasks;
using UnityEngine;
using UObject = UnityEngine.Object;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

// File: GlobalHelper.cs
// Summary: Utility helpers for paths, time, math, parsing, and editor convenience methods used by the demo.
// Note: Editor-only calls are guarded by UNITY_EDITOR; keep dependencies minimal for portability.

namespace Ebonor.Framework
{
    public static class GlobalHelper
    {
        #region File Path Helpers
        public static string GetParentDirectoryName(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError("Input path is empty.");
                return null;
            }

            // Ensure Unity-style separators
            assetPath = assetPath.Replace("\\", "/");

            // Strip filename
            string directoryPath = System.IO.Path.GetDirectoryName(assetPath);

            if (!string.IsNullOrEmpty(directoryPath))
            {
                // Extract parent folder name
                string[] parts = directoryPath.Split('/');
                return parts.Length > 0 ? parts[parts.Length - 1] : null;
            }

            Debug.LogError("Failed to parse parent directory.");
            return null;
        }
        
        /// <summary>
        /// Get folder name from current selection.
        /// </summary>
        /// <returns>Folder name of selection.</returns>
        public static string GetCurrentFolderName()
        {
            // Get current selection path
            string path = GetSelectedPathOrFallback();

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("No selection or default path unavailable.");
                return string.Empty;
            }

            // Extract folder name
            string folderName = System.IO.Path.GetFileName(path);
            return folderName;
        }
        
        public static string GetCurrentFolderName(string path)
        {
            // Path supplied explicitly
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("No selection or default path unavailable.");
                return string.Empty;
            }
#endif
            // Extract folder name
            string folderName = System.IO.Path.GetFileName(path);
            return folderName;
        }
        
        /// <summary>
        /// Get selected path or return "Assets" by default.
        /// </summary>
        public static string GetSelectedPathOrFallback()
        {
#if UNITY_EDITOR
            // If an asset is selected
            if (Selection.activeObject != null)
            {
                // Path of selected asset
                string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        
                // If it's a file, return its parent folder
                if (!string.IsNullOrEmpty(path) && !AssetDatabase.IsValidFolder(path))
                {
                    path = System.IO.Path.GetDirectoryName(path);
                }

                return path;
            }
#endif
            // Default to Assets folder
            return "Assets";
        }
        
        public static string GetCurrentScriptFolder()
        {
            // Full path of current script
            string scriptPath = new System.Diagnostics.StackTrace(true).GetFrame(0).GetFileName();
            return Path.GetFileName(Path.GetDirectoryName(scriptPath));
        }
        
        public static string GetAssetFolderName(UObject obj)
        {
#if UNITY_EDITOR
            string path = AssetDatabase.GetAssetPath(obj);
            return GetFolderNameFromPath(path);
#else
        Debug.LogWarning("AssetDatabase is only available in the editor.");
        return "";
#endif
        }
        
        /// <summary>
        /// Extract folder name from a full path.
        /// </summary>
        public static string GetFolderNameFromPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return "";

            try
            {
                // Normalize separators
                path = path.Replace('\\', '/');
            
                // If file path, take directory
                if (Path.HasExtension(path))
                {
                    path = Path.GetDirectoryName(path);
                }

                // Trim trailing slash
                path = path.TrimEnd('/');
            
                // Get last segment
                int lastSlashIndex = path.LastIndexOf('/');
                return lastSlashIndex >= 0 ? path.Substring(lastSlashIndex + 1) : path;
            }
            catch
            {
                return "";
            }
        }
        
        public static string GetCurrentAssetPath()
        {
#if UNITY_EDITOR
            // Check selected object
            var activeObject = Selection.activeObject;
            if (activeObject != null)
            {
                // Get selected object path
                string path = AssetDatabase.GetAssetPath(activeObject);
                return path;
            }
#endif
            return null;
        }
        #endregion
        
        
        #region Skill Names

        public static long GetSkillID(string skillName)
        {
            const ulong fnvPrime = 1099511628211;
            const ulong fnvOffsetBasis = 14695981039346656037;

            ulong hash = fnvOffsetBasis;
            byte[] data = Encoding.UTF8.GetBytes(skillName);

            foreach (byte b in data)
            {
                hash ^= b;
                hash *= fnvPrime;
            }

            return (long)(hash & 0x7FFFFFFFFFFFFFFF); // Keep positive
        }
        
       
        #endregion
        
        #region Time Helpers
        // Get current UTC time in milliseconds
        public static long GetCurrentTimeInMilliseconds()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            return (long)now.ToUnixTimeMilliseconds();
        }
        
        /// <summary>
        /// Deadline date (inclusive).
        /// </summary>
        private static readonly DateTime Deadline = new DateTime(2025, 7, 31);

        /// <summary>
        /// Check whether current date is on/before deadline.
        /// </summary>
        public static bool IsBeforeDeadline()
        {
            // Compare date only
            return DateTime.Now.Date <= Deadline;
        }

        /// <summary>
        /// Check whether supplied date is on/before deadline.
        /// </summary>
        /// <param name="date">Date to evaluate</param>
        public static bool IsBeforeDeadline(DateTime date)
        {
            return date.Date <= Deadline;
        }
        
        
        // Compute delta between client time and current server time (ms)
        public static long CalculateTimeDifference(long clientTime)
        {
            long serverTime = GetCurrentTimeInMilliseconds();
            Debug.Log("Server Time:" + serverTime);
            return serverTime - clientTime;
        }
        
        public static DateTimeOffset ConvertMillisecondsToDateTime(long milliseconds)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
        }
        
        // Convert ms timestamp to formatted UTC string
        public static string ConvertMillisecondsToFormattedDateTime(long milliseconds)
        {
            DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }
        
        public static string PrintCurrentUtcTime()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            string formattedTime = now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            return formattedTime;
        }
        
        public static string SubtractMillisecondsFromCurrentTime(long milliseconds)
        {
            DateTimeOffset currentTime = DateTimeOffset.UtcNow;
            TimeSpan timeDifference = TimeSpan.FromMilliseconds(milliseconds);
            DateTimeOffset newTime = currentTime - timeDifference;
            string formattedTime = newTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            return formattedTime;
        }
        
        #endregion
        
        #region UnityEngine Extention
        public static Vector3 RandomVector3(Vector3 minimum, Vector3 maximum)
        {
            return new Vector3(UnityEngine.Random.Range(minimum.x, maximum.x),
                UnityEngine.Random.Range(minimum.y, maximum.y),
                UnityEngine.Random.Range(minimum.z, maximum.z));
        }
        
        public static float Remap(float x, float A, float B, float C, float D)
        {
            float remappedValue = C + (x-A)/(B-A) * (D - C);
            return remappedValue;
        }

        public static bool CanRandomTrigClamp01(float x)
        {
            var ran = UnityEngine.Random.Range(0f, 1f);

            return x >= ran;
        }

        
        #endregion
        
        #region Global Variables

        public static StringBuilder gStrBldInst = new StringBuilder();
        #endregion
        
     
        
        #region ip addr
        private static readonly List<string> IpList = new List<string>();
        public static string GetLocalIPAddress()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            Console.WriteLine("Local IP Address: {0}", ip.Address.ToString());
                            return ip.Address.ToString();
                        }
                    }
                }
            }
            return "";
            // var host = Dns.GetHostEntry(Dns.GetHostName());
            // foreach (var ip in host.AddressList)
            // {
            //     if (ip.AddressFamily == AddressFamily.InterNetwork)
            //     {
            //         return ip.ToString();
            //     }
            // }
            // throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static string GetCloudIPAddress()
        {
            using (WebClient webClient = new WebClient())
            {
                // Download the public IP address from the "https://api.ipify.org" URL
                string publicIpAddress = webClient.DownloadString("https://api.ipify.org");

                // Display the public IP address
                Console.WriteLine("Your public IP address is: " + publicIpAddress);
                return publicIpAddress;
            }
        }
        
        public static string GetPublicIPAddress()
        {
            var request = (HttpWebRequest)WebRequest.Create("http://ifconfig.me");
            request.UserAgent = "curl"; // this will tell the server to return the information as if the request was made by the linux "curl" command
            string publicIPAddress;
            request.Method = "GET";
            using(WebResponse response = request.GetResponse())
            {
                using(var reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    publicIPAddress = reader.ReadToEnd();
                }
            }
            return publicIPAddress.Replace("\n", "");
        }
        
        #endregion

        #region String Helpers
        private  static List<string> _splitBuffer = new List<string>();
        public static List<string> SplitToBuffer(string input, char delimiter)
        {
            _splitBuffer.Clear();
            if (string.IsNullOrEmpty(input))
                return null;

            int start = 0;
            while (true)
            {
                int idx = input.IndexOf(delimiter, start);
                if (idx < 0)
                    break;

                // If you want to drop empty entries, add if(idx>start) …
                _splitBuffer.Add(input.Substring(start, idx - start));
                start = idx + 1;
            }

            // Final segment
            if (start < input.Length)
                _splitBuffer.Add(input.Substring(start));

            return _splitBuffer;
        }
        
        public  static Vector3 StringToVector3(string vectorString)
        {
            if (string.IsNullOrEmpty(vectorString))
            {
                Debug.LogError("Input string is empty.");
                return Vector3.zero;
            }

            string[] values = vectorString.Split(',');

            if (values.Length != 3)
            {
                Debug.LogError("String format invalid for Vector3: " + vectorString);
                return Vector3.zero;
            }

            try
            {
                float x = float.Parse(values[0], CultureInfo.InvariantCulture);
                float y = float.Parse(values[1], CultureInfo.InvariantCulture);
                float z = float.Parse(values[2], CultureInfo.InvariantCulture);

                return new Vector3(x, y, z);
            }
            catch (FormatException)
            {
                Debug.LogError("String contains non-numeric values: " + vectorString);
                return Vector3.zero;
            }
        }
        
        public static List<Vector2Int> OnTransStringToListV2n(string str)
        {
            List<Vector2Int> result = new List<Vector2Int>();

            var list1 = str.Split('&');

            for(var i = 0; i < list1.Length; i++)
            {
                var item = list1[i].Split('|');
                result.Add(new Vector2Int(int.Parse(item[0]), int.Parse(item[1])));
            }
            return result;
        }

        public static List<float> OnTransStringToListF(string str)
        {
            List<float> result = new List<float>();

            var list1 = str.Split('|');

            for (var i = 0; i < list1.Length; i++)
            {
                var item = list1[i];
                result.Add(float.Parse(item));
            }
            return result;
        }
        
        public static string CutStringFromStart(string input, int cutLength)
        {
            // Ensure the cut length is not negative and does not exceed the string length
            if (cutLength < 0 || cutLength > input.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(cutLength), "Cut length must be non-negative and less than or equal to the length of the input string.");
            }

            // If cut length is 0 or the input string is empty, return the original string
            if (cutLength == 0 || input.Length == 0)
            {
                return input;
            }

            // Remove the specified number of characters from the start of the string
            return input.Substring(cutLength);
        }
        
        public static string CutStringFromLast(string input, char delimiter)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            int lastIndex = input.LastIndexOf(delimiter);

            if (lastIndex == -1)
            {
                return input; // Delimiter not found, return original string
            }

            return input.Substring(0, lastIndex);
        }
        
        public static string CutStringFromLast(string input, int cutLength)
        {
            if (string.IsNullOrEmpty(input) || cutLength <= 0)
            {
                return input;
            }

            if (cutLength >= input.Length)
            {
                return string.Empty; // Cut length is greater than or equal to the input length, return empty string
            }

            return input.Substring(0, input.Length - cutLength);
        }
        
        public static string CutStringFromLast(string input, int cutLength, out string remainingString)
        {
            if (string.IsNullOrEmpty(input) || cutLength <= 0)
            {
                remainingString = input;
                return string.Empty;
            }

            if (cutLength >= input.Length)
            {
                remainingString = string.Empty; // Cut length is greater than or equal to the input length, return empty string
                return input;
            }

            remainingString = input.Substring(0, input.Length - cutLength);
            return input.Substring(input.Length - cutLength);
        }
        
        #endregion
        
        #region LayerMask
        
        public static int OnGetRawImageLayerMask_BrotaHero()
        {
            return 1 << LayerMask.NameToLayer("Grass") | 
                   1 << LayerMask.NameToLayer("Hero") | 
                   1 << LayerMask.NameToLayer("Ground") | 
                   1 << LayerMask.NameToLayer("Decorations") | 
                   1 << LayerMask.NameToLayer("Obstacles") | 
                   1 << LayerMask.NameToLayer("ScreenEffects") | 
                   1 << LayerMask.NameToLayer("Building") |
                   1 << LayerMask.NameToLayer("Rocks");
        }
        public static int OnGetRawImageLayerMask_HomeHero()
        {
            return 1 << LayerMask.NameToLayer("Hero") | 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Weapon") | 1 << LayerMask.NameToLayer("Obstacles") | 1 << LayerMask.NameToLayer("ScreenEffects");
        }

        public static int OnGetRawImageLayerMask_Home()
        {
            return 1 << LayerMask.NameToLayer("Hero") | 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Weapon")| 1 << LayerMask.NameToLayer("Default");
        }


        public static int OnGetUnWalkLayer()
        {
            return 1 << LayerMask.NameToLayer("Hero") | 1 << LayerMask.NameToLayer("Buildings") | 
                   1 << LayerMask.NameToLayer("Obstacles") |  1 << LayerMask.NameToLayer("Wall") | 
                   1 << LayerMask.NameToLayer("Npc");
        }
        
        public static int OnGetRawImageLayerMask_LoginHero()
        {
            return 1 << LayerMask.NameToLayer("Hero");
        }

        public static void OnSetCamCullingMask(Camera cam, object[] layers)
        {
            int layerIndex = 0;
            foreach (var layer in layers)
            {
                string layerName = (string) layer;
                layerIndex += 1 << LayerMask.NameToLayer(layerName);
            }

   
            cam.cullingMask = layerIndex;
        }
        
        public static void OnSetCamCullingMask(Camera cam, int layer)
        {
            // int layerIndex = 0;
            // foreach (var layer in layers)
            // {
            //     string layerName = (string) layer;
            //     layerIndex += 1 << LayerMask.NameToLayer(layerName);
            // }

   
            cam.cullingMask = layer;
        }
        
        public static void OnResetCamCullingMask(Camera cam)
        {
            cam.cullingMask = 0;
        }
        #endregion
        
        #region Algorithms
        public static Vector3 GetRandomPositionAround(Vector3 center, float radius, float minDistance)
        {
            Vector3 randomPosition = Vector3.zero;
            bool validPositionFound = false;

            int maxAttempts = 100;
            int attempts = 0;

            while (!validPositionFound && attempts < maxAttempts)
            {
                float angle = UnityEngine.Random.Range(0f, 360f);
                float distance = UnityEngine.Random.Range(minDistance, radius);

                float x = Mathf.Cos(angle * Mathf.Deg2Rad) * distance;
                float z = Mathf.Sin(angle * Mathf.Deg2Rad) * distance;

                randomPosition = new Vector3(center.x + x, 0f, center.z + z);

                if ((randomPosition - center).magnitude >= minDistance)
                {
                    validPositionFound = true;
                }

                attempts++;
            }

            return randomPosition;
        }
        
        public static int Pow2(int exponent) => 1 << exponent;
        
        public static int GetExponent(int powerOfTwo)
        {
            if (powerOfTwo <= 0)
            {
                throw new ArgumentException("Input must be a positive integer", nameof(powerOfTwo));
            }

            if ((powerOfTwo & (powerOfTwo - 1)) != 0)
            {
                throw new ArgumentException("Input must be a power of two", nameof(powerOfTwo));
            }

            int exponent = 0;
            while ((powerOfTwo >>= 1) != 0)
            {
                exponent++;
            }
            return exponent;
        }
        
        public static int GetNumberOfDigits(uint value)
        {
            if (value == 0)
            {
                return 1;
            }

            int digits = 0;
            while (value > 0)
            {
                value /= 10;
                digits++;
            }

            return digits;
        }
        
        public static int ExtractTensAndHundredsDigit(uint number)
        {
            if (number < 10000 || number > 99999)
            {
                throw new ArgumentOutOfRangeException(nameof(number), "Number must be a 5-digit integer.");
            }

            int tens = (int)(number / 10 % 10);
            int hundreds = (int)(number / 100 % 10);

            return hundreds * 10 + tens;
        }
        
        public static int ExtractThousandsDigit(uint number)
        {
            if (number < 10000 || number > 99999)
            {
                throw new ArgumentOutOfRangeException(nameof(number), "Number must be a 5-digit integer.");
            }

            return (int)(number / 1000) % 10;
        }
        #endregion
        
        #region encrypt/decrypt byte array
        
        public static byte[] Encrypt(byte[] bytesToEncrypt, string password)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (AesManaged aes = new AesManaged())
                {
                    byte[] key = Encoding.UTF8.GetBytes(password);
                    aes.Key = key;

                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToEncrypt, 0, bytesToEncrypt.Length);
                        cs.FlushFinalBlock();
                    }

                    return ms.ToArray();
                }
            }
        }

        public static byte[] Decrypt(byte[] bytesToDecrypt, string password)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (AesManaged aes = new AesManaged())
                {
                    byte[] key = Encoding.UTF8.GetBytes(password);
                    aes.Key = key;

                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToDecrypt, 0, bytesToDecrypt.Length);
                        cs.FlushFinalBlock();
                    }

                    return ms.ToArray();
                }
            }
        }
        #endregion
        
        #region thread

        static int g_mainThreadId;
        public static void OnInitMainThread()
        {
            g_mainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

        public static bool OnIsInMainThread()
        {
            return g_mainThreadId == System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

        #endregion

        #region editor gizmos
        
        public static void DrawCircle(Vector3 center, float radius, int segments, Color color)
        {
            
#if UNITY_EDITOR
            Gizmos.color = color;
            float angle = 0f;
            //Quaternion rot = Quaternion.LookRotation(Vector3.up); // Use Y axis as normal

            Vector3 lastPoint = center +  new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle) * radius, 0, Mathf.Sin(Mathf.Deg2Rad * angle) * radius);
            Vector3 thisPoint = Vector3.zero;

            for (int i = 1; i <= segments; i++)
            {
                angle = (i * 360f) / segments;
                thisPoint = center +  new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle) * radius, 0, Mathf.Sin(Mathf.Deg2Rad * angle) * radius);
                Gizmos.DrawLine(lastPoint, thisPoint);
                lastPoint = thisPoint;
            }
#endif
        }
        #endregion
        
        #region transform

        public static Vector3 GetGroundPosition(Vector3 pos)
        {
            return new Vector3(pos.x, 0f, pos.z);
        }

        public static Vector3 GetVector3(System.Numerics.Vector3 v3)
        {
            return new Vector3(v3.X, v3.Y, v3.Z);
        }
        #endregion
        
        #region Floating Text

        public static string GetFloatingConfigName(uint skillId, uint buffNodeId)
        {
            gStrBldInst.Clear();
            gStrBldInst.Append(skillId);
            gStrBldInst.Append("_");
            gStrBldInst.Append(buffNodeId);

            return gStrBldInst.ToString();
        }
        #endregion
        
        #region Enums

        public static string GetEnumName<T>(T enumValue)
        {
            return Enum.GetName(typeof(T), enumValue);
        }
        
        public static List<string> GetEnumNamesWithoutZero(Type t)
        {
            return Enum.GetValues(t)
                .Cast<Enum>()
                .Where(e => Convert.ToInt32(e) != 0)
                .Select(e => e.ToString())
                .ToList();
        }

        public static IEnumerable<T> GetFlags<T>(this T input) where T : Enum
        {
            foreach (Enum value in Enum.GetValues(input.GetType()))
            {
                if (input.HasFlag(value))
                {
                    yield return (T)value;
                }
            }
        }
        #endregion
        
        #region text color

        /// <summary>
        /// Converts a hex color string to a Unity Color.
        /// </summary>
        /// <param name="hexColor">The hex color string (e.g., "#FF4500").</param>
        /// <returns>The corresponding Color.</returns>
        public static Color HexToColor(string hexColor)
        {
            if (ColorUtility.TryParseHtmlString(hexColor, out Color color))
            {
                return color;
            }
            else
            {
                Debug.LogError("Invalid hex color string: " + hexColor);
                return Color.white; // Default color
            }
        }
        public static void OnSetTextWithColor<T>(T t, string color, ref StringBuilder strBld)
        {
            strBld.Append("<color=");
            strBld.Append(color);
            strBld.Append(">");
            strBld.Append(t);
            strBld.Append("</color>");
        }
        
        public static string ColorText(string text, Color color) {
            return "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + text + "</color>";
        }
        #endregion
        
     
        #region project
        
        #region Dev Build Check
        public static bool CheckDevClient()
        {
            bool _isDev = true;
            
#if UNITY_EDITOR
            _isDev = true;
#endif
            return _isDev;
        }
        #endregion  
        
        #endregion
        
        
        #region Physics Helpers
        public static float CalculateKnockUpTotalTime(float height, float gravity = 9.8f)
        {
            if (height <= 0f || gravity <= 0f)
                return 0f;
            // Time for a single leg (up or down)
            float singleTime = Mathf.Sqrt(2f * height / gravity);
            // Total time = up + down
            return 2f * singleTime;
        }
         #endregion
        
    }

}