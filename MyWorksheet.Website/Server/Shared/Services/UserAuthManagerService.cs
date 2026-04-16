using System.Collections.Concurrent;

namespace Katana.CommonTasks.Services;

public class UserAuthManagerService
{
    public UserAuthManagerService()
    {
        UserPwHashCache = new ConcurrentDictionary<long, string>();
    }

    public ConcurrentDictionary<long, string> UserPwHashCache { get; set; }

    public string GetLastPwToken(long getUserId)
    {
        string hash = null;
        if (UserPwHashCache.TryGetValue(getUserId, out hash))
        {
            return hash;
        }
        return null;
    }

    public void SetLastPwToken(long userId, string hash)
    {
        UserPwHashCache.AddOrUpdate(userId, hash, (l, s) => hash);
    }
}