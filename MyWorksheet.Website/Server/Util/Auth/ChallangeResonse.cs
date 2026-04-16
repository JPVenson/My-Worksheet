//using Katana.CommonTasks.Middelware.Auth;
//using Microsoft.Owin;

//namespace Katana.CommonTasks.Models.Auth
//{
//    public class ChallangeResonse : ChallangeRequestBody
//    {
//        public string Challange { get; set; }
//        public bool RememberMe { get; set; }

//        public static ChallangeResonse ReadFromStringCollection(IReadableStringCollection collection)
//        {
//            var tokenType = (collection["tt"] ?? collection["grant_type"])?.ToUpper();

//            if (tokenType == null)
//                return null;

//            if (tokenType == HandshakeOatuhTokenIdentityProvider.HandshakeType.ToUpper())
//            {
//                var challange = collection["nc"] ?? collection["Challange"];
//                var username = collection["na"] ?? collection["Username"];
//                var recaptcha = collection["rc"] ?? collection["Recaptcha"];
//                var rememberMe = collection["rm"] ?? collection["RememberMe"];

//                if (challange == null || username == null || recaptcha == null)
//                {
//                    return null;
//                }

//                var rememberMeParsed = false;
//                if (rememberMe != null && !bool.TryParse(rememberMe, out rememberMeParsed))
//                {
//                    return null;
//                }

//                return new ChallangeResonse()
//                {
//                    RememberMe = rememberMeParsed,
//                    Challange = challange,
//                    Username = username,
//                    Recaptcha = recaptcha
//                };
//            }
//            else if (tokenType == "REFRESH_TOKEN")
//            {
//                var username = collection["na"] ?? collection["Username"];
//                var recaptcha = collection["rc"] ?? collection["Recaptcha"];
//                if (username == null || recaptcha == null)
//                {
//                    return null;
//                }
//                return new ChallangeResonse()
//                {
//                    Username = username,
//                    Recaptcha = recaptcha
//                };
//            }
//            return null;
//        }
//    }
//}