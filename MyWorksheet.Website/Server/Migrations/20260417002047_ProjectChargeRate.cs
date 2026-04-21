using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyWorksheet.Website.Server.Migrations
{
    /// <inheritdoc />
    public partial class ProjectChargeRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ProjectChargeRate",
                columns: new[] { "ProjectChargeRate_Id", "Code", "DisplayKey" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0009-000000000001"), "PER_HOUR", "ProjectChargeRate/PerHour" },
                    { new Guid("00000000-0000-0000-0009-000000000002"), "PER_MINUTE", "ProjectChargeRate/PerMinute" },
                    { new Guid("00000000-0000-0000-0009-000000000003"), "PER_QUARTER_MINUTE", "ProjectChargeRate/PerQuarterMinute" },
                    { new Guid("00000000-0000-0000-0009-000000000004"), "PER_STARTED_HOUR", "ProjectChargeRate/PerStartedHour" },
                    { new Guid("00000000-0000-0000-0009-000000000005"), "PER_DAY", "ProjectChargeRate/PerDay" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProjectChargeRate",
                keyColumn: "ProjectChargeRate_Id",
                keyValue: new Guid("00000000-0000-0000-0009-000000000001"));

            migrationBuilder.DeleteData(
                table: "ProjectChargeRate",
                keyColumn: "ProjectChargeRate_Id",
                keyValue: new Guid("00000000-0000-0000-0009-000000000002"));

            migrationBuilder.DeleteData(
                table: "ProjectChargeRate",
                keyColumn: "ProjectChargeRate_Id",
                keyValue: new Guid("00000000-0000-0000-0009-000000000003"));

            migrationBuilder.DeleteData(
                table: "ProjectChargeRate",
                keyColumn: "ProjectChargeRate_Id",
                keyValue: new Guid("00000000-0000-0000-0009-000000000004"));

            migrationBuilder.DeleteData(
                table: "ProjectChargeRate",
                keyColumn: "ProjectChargeRate_Id",
                keyValue: new Guid("00000000-0000-0000-0009-000000000005"));
        }
    }
}
