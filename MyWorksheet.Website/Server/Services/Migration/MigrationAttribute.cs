namespace MyWorksheet.Website.Server.Services.Migration;

using System;
using System.Globalization;

/// <summary>
/// Declares an class as an migration with its set metadata.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class MigrationAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MigrationAttribute"/> class.
    /// </summary>
    /// <param name="order">The ordering this migration should be applied to. Must be a valid DateTime ISO8601 formatted string.</param>
    /// <param name="name">The name of this Migration.</param>
    public MigrationAttribute(string order, string name)
    {
        Order = DateTime.Parse(order, CultureInfo.InvariantCulture);
        Name = name;
        Stage = MigrationStageTypes.AppInitialisation;
    }

    /// <summary>
    /// Gets or Sets a value indicating whether the annoated migration should be executed on a fresh install.
    /// </summary>
    public bool RunMigrationOnSetup { get; set; }

    /// <summary>
    /// Gets or Sets the stage the annoated migration should be executed at. Defaults to <see cref="MigrationStageTypes.CoreInitialisation"/>.
    /// </summary>
    public MigrationStageTypes Stage { get; set; } = MigrationStageTypes.CoreInitialisation;

    /// <summary>
    /// Gets the ordering of the migration.
    /// </summary>
    public DateTime Order { get; }

    /// <summary>
    /// Gets the name of the migration.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the Legacy Key of the migration. Not required for new Migrations.
    /// </summary>
    public Guid? Key { get; }
}
