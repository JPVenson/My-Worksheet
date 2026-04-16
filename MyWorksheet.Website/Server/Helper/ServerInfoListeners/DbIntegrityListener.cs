// using System;
// using System.Collections.Concurrent;
// using System.Collections.Generic;
// using System.Linq;
// using MyWorksheet.Website.Server.Models;
// using MyWorksheet.Website.Server.Services.Db.Migration;
// using MyWorksheet.Website.Server.Services.ServerInfo;
// using Microsoft.EntityFrameworkCore;

// namespace MyWorksheet.Webpage.Helper.ServerInfoListeners;

// public class DbIntegrityListener : ServerInfoListener
// {
// 	private readonly DbMigrationService _dbMigrationService;
// 	private readonly IDbContextFactory<MyworksheetContext> _IDbContextFactory<MyworksheetContext>;

// 	public DbIntegrityListener(Action<string, object> publishValue,
// 		DbMigrationService dbMigrationService,
// 		IDbContextFactory<MyworksheetContext> IDbContextFactory<MyworksheetContext>) : base(publishValue)
// 	{
// 		_dbMigrationService = dbMigrationService;
// 		_IDbContextFactory<MyworksheetContext> = IDbContextFactory<MyworksheetContext>;
// 	}

// 	public override string Key { get; } = "Server Integrity Listener";
// 	public override void PublishValue()
// 	{
// 		var db = _DbContextFactory.CreateDbContext();
// 		var migrations = new ConcurrentDictionary<string, ServerIntegrityEntityGroup>();
// 		foreach (var migration in _dbMigrationService.Migrations.Select(Activator.CreateInstance).OfType<DbMigrationBase>())
// 		{
// 			foreach (var migrationShouldExist in migration.ShouldExists)
// 			{
// 				var type = migrationShouldExist.GetType();
// 				var dbType = db.Config.GetOrCreateClassInfoCache(type);
// 				if (db.Select(type, dbType.PrimaryKeyProperty.Getter.Invoke(migrationShouldExist)) == null)
// 				{
// 					migrations.GetOrAdd(dbType.TableName, s => new ServerIntegrityEntityGroup() {Name = s})
// 						.ShouldExists.Add(migrationShouldExist);
// 				}
// 			}

// 			foreach (var migrationShouldBeLike in migration.ShouldBeLike)
// 			{
// 				var type = migrationShouldBeLike.GetType();
// 				var dbType = db.Config.GetOrCreateClassInfoCache(type);
// 				if (migration.SelectEntity(db, migrationShouldBeLike) == null)
// 				{
// 					var isEntity = db.Select(type, dbType.PrimaryKeyProperty.Getter.Invoke(migrationShouldBeLike));
// 					migrations.GetOrAdd(dbType.TableName, s => new ServerIntegrityEntityGroup() {Name = s})
// 						.Diff.Add(new ServerIntegrityEntity()
// 						{
// 							ShouldBe = migrationShouldBeLike,
// 							Is = isEntity
// 						});
// 				}
// 			}
// 		}

// 		base.Publish(migrations.Select(e => e.Value).ToArray());
// 	}
// }

// public class ServerIntegrityEntityGroup
// {
// 	public ServerIntegrityEntityGroup()
// 	{
// 		Diff = [];
// 		ShouldExists = [];
// 	}
// 	public string Name { get; set; }
// 	public IList<ServerIntegrityEntity> Diff { get; set; }
// 	public IList<object> ShouldExists { get; set; }
// }

// public class ServerIntegrityEntity
// {
// 	public object ShouldBe { get; set; }
// 	public object Is { get; set; }
// }