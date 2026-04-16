using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MyWorksheet.Website.Shared.Util;

namespace MyWorksheet.Website.Server.Util.Auth;

public class LoginChallange : IEquatable<LoginChallange>, IComparable<LoginChallange>
{
    public LoginChallange(DateTime dateTime, long seed, string username, byte[] passwordHash, int privateSeed)
    {
        _passwordHash = passwordHash;
        DateCreated = dateTime;
        Seed = seed;
        Username = username;
        PrivateSeed = privateSeed;
    }

    private readonly byte[] _passwordHash;

    private string _expectedHash;
    private string _hash;
    private bool _isSealed;

    public static IEqualityComparer<LoginChallange> LoginChallangeComparer { get; } = new LoginChallangeEqualityComparer();

    public string GeneratedHash
    {
        get { return Hash(); }
    }

    public DateTime DateCreated { get; private set; }
    public long Seed { get; private set; }
    public int PrivateSeed { get; set; }
    public string Username { get; private set; }

    public int CompareTo(LoginChallange other)
    {
        if (ReferenceEquals(this, other))
        {
            return 0;
        }
        if (ReferenceEquals(null, other))
        {
            return 1;
        }
        var expectedHashComparison = string.Compare(_expectedHash, other._expectedHash, StringComparison.Ordinal);
        if (expectedHashComparison != 0)
        {
            return expectedHashComparison;
        }
        var hashComparison = string.Compare(_hash, other._hash, StringComparison.Ordinal);
        if (hashComparison != 0)
        {
            return hashComparison;
        }
        var dateCreatedComparison = DateCreated.CompareTo(other.DateCreated);
        if (dateCreatedComparison != 0)
        {
            return dateCreatedComparison;
        }
        var seedComparison = Seed.CompareTo(other.Seed);
        if (seedComparison != 0)
        {
            return seedComparison;
        }
        var privateSeedComparison = PrivateSeed.CompareTo(other.PrivateSeed);
        if (privateSeedComparison != 0)
        {
            return privateSeedComparison;
        }
        return string.Compare(Username, other.Username, StringComparison.Ordinal);
    }

    public bool Equals(LoginChallange other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return string.Equals(_hash, other._hash) && DateCreated.Equals(other.DateCreated) &&
               string.Equals(Username, other.Username);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        if (obj.GetType() != GetType())
        {
            return false;
        }
        return Equals((LoginChallange)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = _passwordHash != null ? _passwordHash.GetHashCode() : 0;
            hashCode = (hashCode * 397) ^ (_hash != null ? _hash.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ DateCreated.GetHashCode();
            hashCode = (hashCode * 397) ^ Seed.GetHashCode();
            hashCode = (hashCode * 397) ^ (Username != null ? Username.GetHashCode() : 0);
            return hashCode;
        }
    }

    public static bool operator ==(LoginChallange left, LoginChallange right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(LoginChallange left, LoginChallange right)
    {
        return !Equals(left, right);
    }

    private static byte[] ToReadBytes(byte[] read)
    {
        return read.Select(f => f.ToString("X2")).SelectMany(f => Encoding.ASCII.GetBytes(f)).ToArray();
    }

    public string Hash()
    {
        if (_hash != null)
        {
            return _hash;
        }

        if (_isSealed)
        {
            throw new InvalidOperationException("The object is sealed and cannot be changed anymore");
        }

        var sb = new StringBuilder();
        sb.Append("DC:");
        sb.Append(DateCreated.Ticks);

        sb.Append("SE:");
        sb.Append(Seed);

        sb.Append("US:");
        sb.Append(Username);

        sb.Append("PW:");
        var hexPw = _passwordHash.ToDecHex();
        sb.Append(hexPw);

        using (var sha256 = new SHA256Managed())
        {
            var clear = Encoding.ASCII.GetBytes(sb.ToString());
            var computeHash = sha256.ComputeHash(clear);
            _hash = computeHash.ToDecHex();
        }

        _expectedHash = ChallangeUtil.ShiftString(_hash, _passwordHash.ToHexDecByHexDec());

        return _hash;
    }

    public bool Check(string awnser)
    {
        return _expectedHash == awnser;
    }

    private sealed class LoginChallangeEqualityComparer : IEqualityComparer<LoginChallange>
    {
        public bool Equals(LoginChallange x, LoginChallange y)
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
            return Equals(x._passwordHash, y._passwordHash) && string.Equals(x._hash, y._hash) &&
                   x.DateCreated.Equals(y.DateCreated) && x.Seed == y.Seed && string.Equals(x.Username, y.Username);
        }

        public int GetHashCode(LoginChallange obj)
        {
            unchecked
            {
                var hashCode = obj._passwordHash != null ? obj._passwordHash.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (obj._hash != null ? obj._hash.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ obj.DateCreated.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Seed.GetHashCode();
                hashCode = (hashCode * 397) ^ (obj.Username != null ? obj.Username.GetHashCode() : 0);
                return hashCode;
            }
        }
    }

    public void Seal()
    {
        _isSealed = true;
    }
}