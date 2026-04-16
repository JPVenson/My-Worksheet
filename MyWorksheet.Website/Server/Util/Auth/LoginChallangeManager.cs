using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace MyWorksheet.Website.Server.Util.Auth;

public static class LoginChallangeManager
{
    public static HashSet<LoginChallange> Challanges { get; }
    private static readonly TimeSpan _closedAfter = new TimeSpan(0, 0, 20);

    static LoginChallangeManager()
    {
        Challanges = new HashSet<LoginChallange>(LoginChallange.LoginChallangeComparer);
    }

    private static void ClearOld()
    {
        lock (Challanges)
        {
            Challanges.RemoveWhere(f => DateTime.UtcNow - f.DateCreated > _closedAfter);
        }
    }

    public static string GetNew(string username, byte[] passwordHash)
    {
        lock (Challanges)
        {
            ClearOld();

            if (Challanges.Count(f => f.Username == username) >= 3)
            {
                return null;
            }

            var now = DateTime.UtcNow;
            var challange = new LoginChallange(now, GetRandNumberInternal(now), username, passwordHash, GetRandNumberInternal(now));
            var hash = challange.Hash();
            challange.Seal();
            Challanges.Add(challange);
            return hash;
        }
    }

    internal static int GetRandNumberInternal(DateTime dt)
    {
        lock (Challanges)
        {
            return GetRandNumber(dt, Challanges.Count);
        }
    }

    public static int GetRandNumber(DateTime dt, int seed)
    {
        byte[] buffer;
        do
        {
            buffer = RandomNumberGenerator.GetBytes(1);
        } while (buffer[0] == 0);
        var innerBuffer = new byte[buffer[0]];
        for (var i = 0; i < buffer[0]; i++)
        {
            RandomNumberGenerator.Fill(innerBuffer);
        }

        var sin = (int)dt.Ticks - seed;
        for (var i = 0; i < innerBuffer.Length; i++)
        {
            sin += innerBuffer[i];
        }
        return sin / innerBuffer.Length;
    }

    public static int GetRandNumber(int min, int max, int seed)
    {
        int iterations;
        return GetRandNumber(min, max, seed, out iterations);
    }

    public static int GetRandNumber(int minValue, int maxValue, int seed, out int iterations)
    {
        iterations = 0;
        if (minValue > maxValue)
        {
            throw new ArgumentOutOfRangeException("minValue");
        }

        if (minValue == maxValue)
        {
            return minValue;
        }

        // Make maxValue inclusive.
        maxValue++;

        var uint32Buffer = new byte[4];
        long diff = (long)maxValue - minValue + 1;
        while (true)
        {
            iterations++;
            RandomNumberGenerator.Fill(uint32Buffer);
            uint rand = BitConverter.ToUInt32(uint32Buffer, 0);
            const long max = (1 + (long)int.MaxValue);
            long remainder = max % diff;
            if (rand < max - remainder)
            {
                return (int)(minValue + (rand % diff));
            }
        }
    }

    public static ChallangeResult CheckChallange(string username, string awnser)
    {
        var hasChallange = Challanges.Where(f => f.Username == username).ToArray();
        if (!hasChallange.Any())
        {
            return ChallangeResult.NoChallangeFound();
        }
        foreach (var loginChallange in hasChallange)
        {
            if (loginChallange.Check(awnser))
            {
                Challanges.Remove(loginChallange);
                return ChallangeResult.Data();
            }
        }
        return ChallangeResult.InvalidPassword();
    }
}