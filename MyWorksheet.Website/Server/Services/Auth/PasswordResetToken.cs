using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Services.Auth;

public class PasswordResetToken
{
    private sealed class PasswordResetTokenEqualityComparer : IEqualityComparer<PasswordResetToken>
    {
        public bool Equals(PasswordResetToken x, PasswordResetToken y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (ReferenceEquals(x, null))
            {
                return false;
            }

            if (ReferenceEquals(y, null))
            {
                return false;
            }

            if (x.GetType() != y.GetType())
            {
                return false;
            }

            return string.Equals(x.Token, y.Token) && x.UserId == y.UserId && string.Equals(x.Username, y.Username) && string.Equals(x.Email, y.Email) && x.ValidUntil.Equals(y.ValidUntil);
        }

        public int GetHashCode(PasswordResetToken obj)
        {
            unchecked
            {
                var hashCode = (obj.Token != null ? obj.Token.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ obj.UserId.GetHashCode();
                hashCode = (hashCode * 397) ^ (obj.Username != null ? obj.Username.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.Email != null ? obj.Email.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ obj.ValidUntil.GetHashCode();
                return hashCode;
            }
        }
    }

    public static IEqualityComparer<PasswordResetToken> PasswordResetTokenComparer { get; } = new PasswordResetTokenEqualityComparer();

    public string Token { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public DateTime ValidUntil { get; set; }
}