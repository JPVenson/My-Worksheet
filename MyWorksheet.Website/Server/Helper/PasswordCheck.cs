using System;
using System.Collections.Generic;
using System.Linq;

namespace MyWorksheet.Webpage.Helper;

public class PasswordTest
{
    public PasswordTest(string name, string hintText, Func<string, PasswordTestResult> testFunc)
    {
        Name = name;
        HintText = hintText;
        TestFunc = testFunc;
    }

    public string Name { get; private set; }
    public string HintText { get; private set; }
    public Func<string, PasswordTestResult> TestFunc { get; private set; }
}

public enum PasswordTestResult
{
    NoMatch,
    AcceptedMatch,
    Match
}

public static class PasswordCheck
{
    static PasswordCheck()
    {
        PasswordTests =
        [
            new PasswordTest("Password Length", "The password must be contain more then 8 charaters", s => s.Length < 8 ? PasswordTestResult.NoMatch : PasswordTestResult.Match),
            new PasswordTest("Lower Case character", "The password must have at least one Lower case charater",
                s =>
                {
                    var lowerCases = s.Count(char.IsLower);
                    if (lowerCases == 0)
                    {
                        return PasswordTestResult.NoMatch;
                    }
                    if (lowerCases > 2)
                    {
                        return PasswordTestResult.Match;
                    }
                    return PasswordTestResult.AcceptedMatch;
                }),
            new PasswordTest("Upper Case character", "The password must have at least one Upper case charater",
                s =>
                {
                    var lowerCases = s.Count(char.IsUpper);
                    if (lowerCases == 0)
                    {
                        return PasswordTestResult.NoMatch;
                    }
                    if (lowerCases > 2)
                    {
                        return PasswordTestResult.Match;
                    }
                    return PasswordTestResult.AcceptedMatch;
                }),
            new PasswordTest("Number", "The password must have at least one number",
                s =>
                {
                    var lowerCases = s.Count(char.IsDigit);
                    if (lowerCases == 0)
                    {
                        return PasswordTestResult.NoMatch;
                    }
                    if (lowerCases > 2)
                    {
                        return PasswordTestResult.Match;
                    }
                    return PasswordTestResult.AcceptedMatch;
                }),
            new PasswordTest("Number", "The password must have at least one Special Character",
                s =>
                {
                    var lowerCases = s.Count(e => !char.IsLetterOrDigit(e));
                    if (lowerCases == 0)
                    {
                        return PasswordTestResult.NoMatch;
                    }
                    if (lowerCases > 2)
                    {
                        return PasswordTestResult.Match;
                    }
                    return PasswordTestResult.AcceptedMatch;
                }),
        ];
    }
    public static ICollection<PasswordTest> PasswordTests { get; private set; }

    public static bool FullPasswordCheck(string password, List<string> hits)
    {
        hits.Clear();
        var matches = true;
        foreach (var passwordTest in PasswordTests)
        {
            var testRes = passwordTest.TestFunc(password);
            if (testRes == PasswordTestResult.NoMatch)
            {
                matches = false;
                hits.Add(passwordTest.HintText);
            }
        }

        return matches;
    }

    /// <summary>
    /// Generic method to retrieve password strength: use this for general purpose scenarios,
    /// i.e. when you don't have a strict policy to follow.
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public static PasswordStrength GetPasswordStrength(string password)
    {
        int score = 0;
        if (String.IsNullOrEmpty(password) || String.IsNullOrEmpty(password.Trim()))
        {
            return PasswordStrength.Blank;
        }

        if (!HasMinimumLength(password, 5))
        {
            return PasswordStrength.VeryWeak;
        }

        if (HasMinimumLength(password, 8))
        {
            score++;
        }

        if (HasUpperCaseLetter(password) && HasLowerCaseLetter(password))
        {
            score++;
        }

        if (HasDigit(password))
        {
            score++;
        }

        if (HasSpecialChar(password))
        {
            score++;
        }

        return (PasswordStrength)score;
    }

    /// <summary>
    /// Generic method to retrieve password strength: use this for general purpose scenarios,
    /// i.e. when you don't have a strict policy to follow.
    /// </summary>
    /// <param name="password"></param>
    /// <param name="passwordHints"></param>
    /// <returns></returns>
    public static PasswordStrength TestPasswordStrength(string password, ICollection<string> passwordHints)
    {
        int score = 0;
        if (String.IsNullOrEmpty(password) || String.IsNullOrEmpty(password.Trim()))
        {
            return PasswordStrength.Blank;
        }

        if (!HasMinimumLength(password, 5))
        {
            return PasswordStrength.VeryWeak;
        }

        if (HasMinimumLength(password, 8))
        {
            score++;
        }

        if (HasUpperCaseLetter(password) && HasLowerCaseLetter(password))
        {
            score++;
        }

        if (HasDigit(password))
        {
            score++;
        }

        if (HasSpecialChar(password))
        {
            score++;
        }

        return (PasswordStrength)score;
    }

    /// <summary>
    /// Sample password policy implementation:
    /// - minimum 8 characters
    /// - at lease one UC letter
    /// - at least one LC letter
    /// - at least one non-letter char (digit OR special char)
    /// </summary>
    /// <returns></returns>
    public static bool IsStrongPassword(string password)
    {
        return HasMinimumLength(password, 8)
               && HasUpperCaseLetter(password)
               && HasLowerCaseLetter(password)
               && (HasDigit(password) || HasSpecialChar(password));
    }


    /// <summary>
    /// Sample password policy implementation following the Microsoft.AspNetCore.Identity.PasswordOptions standard.
    /// </summary>
    public static bool IsValidPassword(
        string password,
        int requiredLength,
        int requiredUniqueChars,
        bool requireNonAlphanumeric,
        bool requireLowercase,
        bool requireUppercase,
        bool requireDigit)
    {
        if (!HasMinimumLength(password, requiredLength))
        {
            return false;
        }

        if (!HasMinimumUniqueChars(password, requiredUniqueChars))
        {
            return false;
        }

        if (requireNonAlphanumeric && !HasSpecialChar(password))
        {
            return false;
        }

        if (requireLowercase && !HasLowerCaseLetter(password))
        {
            return false;
        }

        if (requireUppercase && !HasUpperCaseLetter(password))
        {
            return false;
        }

        if (requireDigit && !HasDigit(password))
        {
            return false;
        }

        return true;
    }

    public static bool HasMinimumLength(string password, int minLength)
    {
        return password.Length >= minLength;
    }

    public static bool HasMinimumUniqueChars(string password, int minUniqueChars)
    {
        return password.Distinct().Count() >= minUniqueChars;
    }

    /// <summary>
    /// Returns TRUE if the password has at least one digit
    /// </summary>
    public static bool HasDigit(string password)
    {
        return password.Any(c => char.IsDigit(c));
    }

    /// <summary>
    /// Returns TRUE if the password has at least one special character
    /// </summary>
    public static bool HasSpecialChar(string password)
    {
        // return password.Any(c => char.IsPunctuation(c)) || password.Any(c => char.IsSeparator(c)) || password.Any(c => char.IsSymbol(c));
        return password.IndexOfAny("!@#$%^&*?_~-£().,".ToCharArray()) != -1;
    }

    /// <summary>
    /// Returns TRUE if the password has at least one uppercase letter
    /// </summary>
    public static bool HasUpperCaseLetter(string password)
    {
        return password.Any(c => char.IsUpper(c));
    }

    /// <summary>
    /// Returns TRUE if the password has at least one lowercase letter
    /// </summary>
    public static bool HasLowerCaseLetter(string password)
    {
        return password.Any(c => char.IsLower(c));
    }
}