using System.Runtime.Serialization;

namespace Katana.CommonTasks.Models.Google.ReCapture2;

public class GoogleCapchaV2Resp
{
    [DataMember(Name = "success")]
    public bool Success { get; set; }

    [DataMember(Name = "challenge_ts")]
    public string ChallangeTimeStamp { get; set; }

    [DataMember(Name = "hostname")]
    public string Hostname { get; set; }

    [DataMember(Name = "error-codes")]
    public string[] ErrorCode { get; set; }
}