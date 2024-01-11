using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;

namespace StormLauncher.Utils
{
    internal class Library
    {

        internal class Win32
        {
            [DllImport("kernel32.dll")]
            public static extern int SuspendThread(IntPtr hThread);

            [DllImport("kernel32.dll")]
            public static extern int ResumeThread(IntPtr hThread);

            [DllImport("kernel32.dll")]
            public static extern IntPtr OpenThread(int dwDesiredAccess, bool bInheritHandle, int dwThreadId);
        }

        internal class Auth
        {
            public static string GetExchangeCode(string accessToken)
            {
                var client = new RestClient("https://account-public-service-prod03.ol.epicgames.com");
                var request = new RestRequest("/account/api/oauth/exchange");
                request.AddHeader("Authorization", "Bearer " + accessToken);
                var Execute = client.Execute(request);
                var StatusCode = Execute.StatusCode;
                var Content = Execute.Content;
                if (StatusCode == HttpStatusCode.OK)
                {
                    return ((object)JObject.Parse(Content)["code"]).ToString();
                }
                return null;
            }
        }

        internal class LauncherInstalled
        {

            public List<LauncherInstalled> InstallationList { get; set; }

            public string InstallLocation { get; set; }

            public string NamespaceId { get; set; }

            public string ItemId { get; set; }

            public string ArtifactId { get; set; }

            public string AppVersion { get; set; }

            public string AppName { get; set; }

            public static LauncherInstalled GetInstalled()
            {
                string text = File.ReadAllText("C:\\ProgramData\\Epic\\UnrealEngineLauncher\\LauncherInstalled.dat");
                List<LauncherInstalled> installationList = JsonConvert.DeserializeObject<LauncherInstalled>(text).InstallationList;
                return installationList.FirstOrDefault((LauncherInstalled i) => i.ItemId == "4fe75bbc5a674f4f9b356b5c90567da5");
            }
        }

    }
}

