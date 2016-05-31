﻿using Microsoft.Azure.KeyVault;
using Microsoft.PS.Common.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Microsoft.PS.Common.Vault.Explorer
{
    public static class Utils
    {
        public const string AppName = "Azure Key Vault Explorer";

        /// <summary>
        /// Space with black down triangle char
        /// </summary>
        public const string DropDownSuffix = " \u25BC"; 

        /// <summary>
        /// Converts DateTime? to LocalTime string
        /// </summary>
        /// <param name="dt">DateTime?</param>
        /// <returns>string</returns>
        public static string NullableDateTimeToString(DateTime? dt)
        {
            if (dt == null) return "Unknown";
            return dt.Value.ToLocalTime().ToString();
        }

        public static string CalculateMd5(string value)
        {
            byte[] buff = Encoding.UTF8.GetBytes(value);
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(buff);
                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
            }
        }

        public static Dictionary<string, string> AddChangedBy(Dictionary<string, string> tags)
        {
            if (tags == null)
            {
                tags = new Dictionary<string, string>();
            }
            tags[Consts.ChangedByKey] = $"{Environment.UserDomainName}\\{Environment.UserName}";
            return tags;
        }

        public static string GetChangedBy(Dictionary<string, string> tags)
        {
            if ((tags == null) || (!tags.ContainsKey(Consts.ChangedByKey)))
            {
                return "";
            }
            return tags[Consts.ChangedByKey];
        }

        public static string GetMd5(Dictionary<string, string> tags)
        {
            if ((tags == null) || (!tags.ContainsKey(Consts.Md5Key)))
            {
                return "";
            }
            return tags[Consts.Md5Key];
        }

        public static string FullPathToJsonFile(string filename)
        {
            filename = Environment.ExpandEnvironmentVariables(filename);
            if (Path.IsPathRooted(filename)) return filename;
            filename = Path.Combine(Settings.Default.JsonConfigurationFilesRoot, filename);
            if (Path.IsPathRooted(filename)) return filename;
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
        }

        public static T LoadFromJsonFile<T>(string filename)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(FullPathToJsonFile(filename)));
        }

        public static Cursor LoadCursorFromResource(byte[] buffer)
        {
            using (var ms = new MemoryStream(buffer))
            {
                return new Cursor(ms);
            }
        }
        public static string GetRtfUnicodeEscapedString(string s)
        {
            var sb = new StringBuilder();
            foreach (var c in s)
            {
                if (c == '\\' || c == '{' || c == '}')
                    sb.Append(@"\" + c);
                else if (c <= 0x7f)
                    sb.Append(c);
                else
                    sb.Append("\\u" + Convert.ToUInt32(c) + "?");
            }
            return sb.ToString();
        }

        public static string GetFileVersionString(string title, string peFilename, string optionalPrefix = "")
        {
            var filepath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), peFilename);
            string version = "Unknown";
            try
            {
                var verInfo = FileVersionInfo.GetVersionInfo(filepath);
                version = string.Format("{0}.{1}.{2}.{3}", verInfo.FileMajorPart, verInfo.FileMinorPart, verInfo.FileBuildPart, verInfo.FilePrivatePart);
            }
            catch { }
            return string.Format(string.Format("{0}: {1}{2}", title, version, optionalPrefix));
        }
    }
}
