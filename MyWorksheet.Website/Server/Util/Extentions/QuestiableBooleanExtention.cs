using Katana.CommonTasks.Models;

namespace Katana.CommonTasks.Extentions;

public static class QuestiableBooleanExtention
{
    public static QuestionableBoolean Because(this bool value, string reason)
    {
        return new QuestionableBoolean(value, reason);
    }
}