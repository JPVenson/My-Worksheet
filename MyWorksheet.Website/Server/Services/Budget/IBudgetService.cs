using Katana.CommonTasks.Models;
using System;
using MyWorksheet.Website.Server.Models;

namespace MyWorksheet.Website.Server.Services.Budget;

public interface IBudgetService
{
    QuestionableBoolean Add(MyworksheetContext db, Guid projectId, Guid forUser, int time, bool overwrite = false);
    QuestionableBoolean Substract(MyworksheetContext db, Guid projectId, Guid forUser, int time, bool overwrite = false);
    QuestionableBoolean ExceedsBudget(MyworksheetContext db, Guid idProject, Guid forUser, int time);
    void ReevaluateBudget(MyworksheetContext db, ProjectBudget budget);

    ProjectBudget BudgetForUserInProject(MyworksheetContext db, Guid project, Guid userId);
}