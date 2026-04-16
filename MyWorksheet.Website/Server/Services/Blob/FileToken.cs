using System;

namespace MyWorksheet.Website.Server.Services.Blob;

public struct FileToken : IEquatable<FileToken>
{
    public override string ToString()
    {
        return $"{nameof(UserId)}: {UserId}, {nameof(StorageId)}: {StorageId}, {nameof(CallerIp)}: {CallerIp}, {nameof(Issued)}: {Issued}";
    }

    public FileToken(Guid userId, Guid storageId, string callerIp, DateTime issued)
    {
        UserId = userId;
        StorageId = storageId;
        CallerIp = callerIp;
        Issued = issued;
    }

    public Guid UserId { get; private set; }
    public Guid StorageId { get; private set; }
    public string CallerIp { get; private set; }
    public DateTime Issued { get; private set; }

    public bool Equals(FileToken other)
    {
        return UserId == other.UserId
               && StorageId == other.StorageId
               && string.Equals(CallerIp, other.CallerIp);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        return obj is FileToken && Equals((FileToken)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = UserId.GetHashCode();
            hashCode = (hashCode * 397) ^ StorageId.GetHashCode();
            hashCode = (hashCode * 397) ^ (CallerIp != null ? CallerIp.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ Issued.GetHashCode();
            return hashCode;
        }
    }

    public static bool operator ==(FileToken left, FileToken right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(FileToken left, FileToken right)
    {
        return !left.Equals(right);
    }

    public bool IsValid(string callerIp, string storageId, int ttl)
    {
        return callerIp == CallerIp
               && storageId == this.StorageId.ToString()
               && (this.Issued + TimeSpan.FromSeconds(ttl)) > DateTime.UtcNow;
    }
}
