using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using StormLauncher;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using static StormLauncher.Utils.Library;


internal class Program
{

    public static string exchangeCode = "";
    public static string AccessToken = null;
    public static string CredentialsToken = null;
    public static string AccountId = null;
    public static string DisplayName = null;
    public static string Content = null;
    public static string Caldera = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiIiwiZ2VuZXJhdGVkIjoxNjc1MTY5ODQ2LCJjYWxkZXJhR3VpZCI6IjIwZDg3N2JjLTc1MGUtNGY3Ni1hZjIxLTA3NjUxMmVjOTMyNCIsImFjUHJvdmlkZXIiOiJFYXN5QW50aUNoZWF0Iiwibm90ZXMiOiJkb1JlcXVlc3QgZXJyb3I6IGRvUmVxdWVzdCBmYWlsdXJlIGNvZGU6IDQwMCIsImZhbGxiYWNrIjp0cnVlfQ.npqgHriv6k1PIf34mcc0yFV0MBVAM2k5bZKZ7SA6uQ4";
    public static bool LoginStatus = false;
    public static Thread Thread = null;

    public static Process Shipping = null;
    public static Process Shipping_EAC = null;
    public static Process FNLauncher = null;







    private static void Main(string[] args)
    {


        void LoginAccount()
        {
            var client = new RestClient("https://account-public-service-prod03.ol.epicgames.com");
            var request = new RestRequest("/account/api/oauth/token", Method.Post);
            request.AddHeader("Authorization", "Basic OThmN2U0MmMyZTNhNGY4NmE3NGViNDNmYmI0MWVkMzk6MGEyNDQ5YTItMDAxYS00NTFlLWFmZWMtM2U4MTI5MDFjNGQ3");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "client_credentials");
            var Execute = client.Execute(request);
            var StatusCode = Execute.StatusCode;
            Content = Execute.Content;
            if (StatusCode == HttpStatusCode.OK)
            {
                CredentialsToken = ((object)JObject.Parse(Content)["access_token"]).ToString();
                client = new RestClient("https://account-public-service-prod03.ol.epicgames.com");
                request = new RestRequest("/account/api/oauth/deviceAuthorization", Method.Post);
                request.AddHeader("Authorization", "Bearer " + CredentialsToken);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                Execute = client.Execute(request);
                StatusCode = Execute.StatusCode;
                Content = Execute.Content;
                if (StatusCode == HttpStatusCode.OK)
                {
                    var DeviceCode = ((object)JObject.Parse(Content)["device_code"]).ToString();
                    var VerificationUri = ((object)JObject.Parse(Content)["verification_uri_complete"]).ToString();
                    Process.Start(new ProcessStartInfo
                    {
                        UseShellExecute = true,
                        FileName = VerificationUri,
                    });
                    while (true)
                    {
                        client = new RestClient("https://account-public-service-prod03.ol.epicgames.com");
                        request = new RestRequest("/account/api/oauth/token", Method.Post);
                        request.AddHeader("Authorization", "Basic OThmN2U0MmMyZTNhNGY4NmE3NGViNDNmYmI0MWVkMzk6MGEyNDQ5YTItMDAxYS00NTFlLWFmZWMtM2U4MTI5MDFjNGQ3");
                        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        request.AddParameter("grant_type", "device_code");
                        request.AddParameter("device_code", DeviceCode);
                        Execute = client.Execute(request);
                        StatusCode = Execute.StatusCode;
                        Content = Execute.Content;
                        if (StatusCode == HttpStatusCode.OK)
                        {
                            AccessToken = ((object)JObject.Parse(Content)["access_token"]).ToString();
                            AccountId = ((object)JObject.Parse(Content)["account_id"]).ToString();
                            DisplayName = ((object)JObject.Parse(Content)["displayName"]).ToString();
                            LoginStatus = true;
                            return;
                        }
                        if (StatusCode == HttpStatusCode.NotFound)
                        {
                            break;
                        }
                        Thread.Sleep(1000);
                    }
                }
            }
        }



        Console.ForegroundColor = ConsoleColor.Red;
        Console.Title = "StormFN Launcher";
        Console.WriteLine("[STORM] Welcome To StormFN! https://discord.gg/stormfn");
        Console.WriteLine("[STORM] Finding Fortnite Installation");
        string FortnitePath = "";
        try
        {
            dynamic StormJVal = JsonConvert.DeserializeObject(System.IO.File.ReadAllText(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Epic\\UnrealEngineLauncher\\LauncherInstalled.dat")));
            foreach (var installion in StormJVal.InstallationList)
            {
                if (installion.AppName == "Fortnite")
                {
                    FortnitePath = installion.InstallLocation.ToString();
                    string version = installion.AppVersion.ToString().Split('-')[1];
                    Console.WriteLine($"[STORM] Found Version {version} in {FortnitePath} !");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR] Could not locate Epic Installations List, Please make sure you have Fortnite Installed!");
            System.Threading.Thread.Sleep(5000);
        }
        Console.WriteLine("[STORM] Fetching Required Files");
        new WebClient().DownloadFile("https://cdn.discordapp.com/attachments/1065262762210635808/1194918258416033792/CobaltV2.dll?ex=65b21959&is=659fa459&hm=05d189a1df0ed437167d9e2e64ed789e2848fdea2f87b1cf252feed1611925c3&", FortnitePath + "//FortniteGame//Binaries//Win64//ssl.dll");
        new WebClient().DownloadFile("https://cdn.discordapp.com/attachments/1122843972881092721/1125876395512451122/Injector.exe?ex=65b0189a&is=659da39a&hm=1500859b6242a4555717b451e1ebfca1997dc23afb73bab6cb359a3700f8b958&", FortnitePath + "//FortniteGame//Binaries//Win64//Injector.exe");

        Console.WriteLine("[STORM] Logging In");
        LoginAccount();

        Console.WriteLine("[STORM] Launching");
        FNLauncher = new Process
        {
            StartInfo =
            {
                FileName = FortnitePath + "\\FortniteGame\\Binaries\\Win64\\FortniteLauncher.exe",
                UseShellExecute = false,
            }
        };
        FNLauncher.Start();
        foreach (ProcessThread thread in FNLauncher.Threads)
            Win32.SuspendThread(Win32.OpenThread(2, bInheritHandle: false, thread.Id));
        Shipping_EAC = new Process
        {
            StartInfo =
            {
                FileName = FortnitePath + "\\FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping_EAC.exe",
                Arguments = "-epiclocale = en - nobe - fromfl = eac - fltoken = 3db3ba5dcbd2e16703f3978d - caldera = eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiYmU5ZGE1YzJmYmVhNDQwN2IyZjQwZWJhYWQ4NTlhZDQiLCJnZW5lcmF0ZWQiOjE2Mzg3MTcyNzgsImNhbGRlcmFHdWlkIjoiMzgxMGI4NjMtMmE2NS00NDU3LTliNTgtNGRhYjNiNDgyYTg2IiwiYWNQcm92aWRlciI6IkVhc3lBbnRpQ2hlYXQiLCJub3RlcyI6IiIsImZhbGxiYWNrIjpmYWxzZX0.VAWQB67RTxhiWOxx7DBjnzDnXyyEnX7OljJm - j2d88G_WgwQ9wrE6lwMEHZHjBd1ISJdUO1UVUqkfLdU5nofBQ",
                UseShellExecute = false
            }
        };
        Shipping_EAC.Start();
        foreach (ProcessThread thread2 in Shipping_EAC.Threads)
            Win32.SuspendThread(Win32.OpenThread(2, bInheritHandle: false, thread2.Id));
        Shipping = new Process
        {
            StartInfo =
            {
                FileName = FortnitePath + "\\FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe",
                Arguments = "-AUTH_LOGIN=unused -AUTH_PASSWORD=" + exchangeCode + " -AUTH_TYPE=exchangecode -epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -nobe -fromfl=eac -fltoken=3db3ba5dcbd2e16703f3978d -caldera=eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiYmU5ZGE1YzJmYmVhNDQwN2IyZjQwZWJhYWQ4NTlhZDQiLCJnZW5lcmF0ZWQiOjE2Mzg3MTcyNzgsImNhbGRlcmFHdWlkIjoiMzgxMGI4NjMtMmE2NS00NDU3LTliNTgtNGRhYjNiNDgyYTg2IiwiYWNQcm92aWRlciI6IkVhc3lBbnRpQ2hlYXQiLCJub3RlcyI6IiIsImZhbGxiYWNrIjpmYWxzZX0.VAWQB67RTxhiWOxx7DBjnzDnXyyEnX7OljJm-j2d88G_WgwQ9wrE6lwMEHZHjBd1ISJdUO1UVUqkfLdU5nofBQ - skippatchcheck",
                UseShellExecute = false,

            }
        };
        Shipping.Start();
        Shipping.WaitForInputIdle();
        Process process = new Process();
        process.StartInfo.FileName = FortnitePath + "\\FortniteGame\\Binaries\\Win64\\Injector.exe";
        process.StartInfo.Arguments = string.Format("\"{0}\" \"{1}\"", Shipping.Id, FortnitePath + "\\FortniteGame\\Binaries\\Win64\\ssl.dll");
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
        Shipping.WaitForExit();
        Shipping_EAC.Kill();
        FNLauncher.Kill();
        LoginStatus = false;
    }
}