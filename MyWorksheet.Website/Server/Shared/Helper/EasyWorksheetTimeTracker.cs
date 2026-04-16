using System;

namespace MyWorksheet.Website.Server.Shared.Helper;

public class EasyWorksheetTimeItem : IEquatable<EasyWorksheetTimeItem>, IComparable<EasyWorksheetTimeItem>
{
    public int CompareTo(EasyWorksheetTimeItem other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        var projectIdComparison = WorksheetId.CompareTo(other.WorksheetId);
        if (projectIdComparison != 0) return projectIdComparison;
        return StartTime.CompareTo(other.StartTime);
    }

    public EasyWorksheetTimeItem()
    {

    }

    public EasyWorksheetTimeItem(Guid worksheetId, DateTime startTime, Guid trackerId)
    {
        WorksheetId = worksheetId;
        StartTime = startTime;
        TrackerId = trackerId;
    }

    public bool Equals(EasyWorksheetTimeItem other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return WorksheetId == other.WorksheetId && StartTime.Equals(other.StartTime);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((EasyWorksheetTimeItem)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (WorksheetId.GetHashCode() * 397) ^ StartTime.GetHashCode();
        }
    }

    public static bool operator ==(EasyWorksheetTimeItem left, EasyWorksheetTimeItem right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(EasyWorksheetTimeItem left, EasyWorksheetTimeItem right)
    {
        return !Equals(left, right);
    }

    public Guid TrackerId { get; set; }
    public Guid WorksheetId { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; set; }
}