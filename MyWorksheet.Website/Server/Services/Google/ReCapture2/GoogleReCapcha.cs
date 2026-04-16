using System.IO;
using System.Net;
using Katana.CommonTasks.Extentions;
using Katana.CommonTasks.Models;
using Katana.CommonTasks.Models.Google.ReCapture2;
using Newtonsoft.Json;

namespace MyWorksheet.Website.Server.Services.Google.ReCapture2;

public static class GoogleReCapcha
{
    public static string SecretKey { get; set; }

    public static void LoadDebugCode()
    {
        SecretKey = "6LeIxAcTAAAAAGG-vFI1TnRWxMZNFuojJ4WifJWe";
    }

    public static QuestionableBoolean Validate(string key, string enduserIp)
    {
        if (string.IsNullOrEmpty(key))
        {
            return false.Because("Invalid or no Recapture Key");
        }

        GoogleCapchaV2Resp data = null;

        //Request to Google Server
        var req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=" + SecretKey + "&response=" + key + "&remoteip=" + enduserIp);
        try
        {
            //Google recaptcha Response
            using (var wResponse = req.GetResponse())
            {
                using (var readStream = new StreamReader(wResponse.GetResponseStream()))
                {
                    var jsonResponse = readStream.ReadToEnd();

                    data = JsonConvert.DeserializeObject<GoogleCapchaV2Resp>(jsonResponse);
                }
            }

            return data.Success;
            //.Because(data.ErrorCode?.(g => g.AggregateIf(f => f.Any(), (e, f) => e + ", " + f)));
        }
        catch (WebException)
        {
            return false.Because("Your ReCapcha could not be validated!");
        }
    }
}