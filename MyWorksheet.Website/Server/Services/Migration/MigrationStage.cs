namespace MyWorksheet.Website.Server.Services.Migration;

using System.Collections.ObjectModel;

/// <summary>
/// Defines a Stage that can be Invoked and Handled at different times from the code.
/// </summary>
internal class MigrationStage : Collection<CodeMigration>
{
    public MigrationStage(MigrationStageTypes stage)
    {
        Stage = stage;
    }

    public MigrationStageTypes Stage { get; }
}
