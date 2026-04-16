using System.Collections.Generic;

namespace MyWorksheet.Webpage.Helper;

public class TestPasswordResult
{
    public TestPasswordResult()
    {
        Hints = [];
    }
    public bool IsSecure { get; set; }
    public PasswordScore Score { get; set; }
    public List<string> Hints { get; set; }
}