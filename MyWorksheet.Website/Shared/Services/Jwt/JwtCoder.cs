using System.IdentityModel.Tokens.Jwt;

namespace MyWorksheet.Website.Shared.Services.Jwt
{
    public static class JwtCoder
    {
        public static string EncodeToken(JwtSecurityToken jwtToken)
        {
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        public static JwtSecurityToken DecodeToken(string token)
        {
            return new JwtSecurityTokenHandler().ReadJwtToken(token);
        }
    }
}
