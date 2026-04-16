using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyWorksheet.Website.Server.Migrations
{
    /// <inheritdoc />
    public partial class CreateViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Helper function: worksheet_is_submitted(uuid) -> boolean
            // Checks if a worksheet's current status equals the workflow's NoModificationsStep
            migrationBuilder.Sql(@"
CREATE OR REPLACE FUNCTION worksheet_is_submitted(p_worksheet_id uuid)
RETURNS boolean
LANGUAGE sql STABLE AS $$
    SELECT EXISTS (
        SELECT 1 FROM ""Worksheet"" w
        JOIN ""WorksheetStatusLookup"" wsl
            ON wsl.""WorksheetStatusLookup_Id"" = w.""Id_CurrentStatus""
        WHERE w.""Worksheet_Id"" = p_worksheet_id
          AND wsl.""AllowModifications"" = FALSE
    )
$$;
");

            // Views/ClientStructure/ClientStructureWithRights.sql
            migrationBuilder.Sql(@"
CREATE VIEW ""ClientStructureWithRights"" AS
SELECT cs.*, csr.""Id_Role"", csr.""Inverse""
FROM ""ClientStructure"" cs
    JOIN ""ClientSturctureRight"" csr ON cs.""ClientStructure_Id"" = csr.""Id_ClientStructure"";
");

            // Views/Dashboard/DashboardWorksheets.sql
            migrationBuilder.Sql(@"
CREATE VIEW ""DashboardWorksheets"" AS
SELECT
    p.""Name"" AS ""ProjectName"",
    w.*,
    COALESCE(
        (SELECT FALSE FROM ""WorksheetItem"" wi
         WHERE wi.""Id_Worksheet"" = w.""Worksheet_Id""
           AND wi.""DateOfAction""::date = CURRENT_DATE
         LIMIT 1),
        TRUE
    ) AS ""HasDaysOpen""
FROM ""Worksheet"" w
    JOIN ""Project"" p ON p.""Project_Id"" = w.""Id_Project""
WHERE w.""Hidden"" = FALSE
  AND worksheet_is_submitted(w.""Worksheet_Id"") = FALSE;
");

            // Views/OrdersAggregated.sql
            migrationBuilder.Sql(@"
CREATE VIEW ""OrdersAggregated"" AS
SELECT
    pfapp.""Id_AppUser"",
    pfapp.""Id_PromisedFeatureContent"",
    COUNT(1) AS ""Ammount""
FROM ""PaymentOrder"" AS pfapp
GROUP BY pfapp.""Id_AppUser"", pfapp.""Id_PromisedFeatureContent"";
");

            // Views/Organisation/OrganisationWorksheets.sql
            // Simplified: XML serialisation replaced with plain columns; worksheet list resolved in application code
            migrationBuilder.Sql(@"
CREATE VIEW ""OrganisationWorksheets"" AS
SELECT
    org.""Organisation_Id"",
    oum.""Id_AppUser"",
    org.""IsActive""
FROM ""Organisation"" org
    JOIN ""OrganisationUserMap"" oum ON oum.""Id_Organisation"" = org.""Organisation_Id"";
");

            // Views/Organisation/ProjectsInOrganisation.sql
            migrationBuilder.Sql(@"
CREATE VIEW ""ProjectsInOrganisation"" AS
WITH ""ProjectsWithUsers"" AS (
    SELECT
        proj.""Project_Id"",
        proj.""Name"",
        proj.""Hidden"",
        proj.""BookToOvertimeAccount"",
        proj.""Id_WorksheetWorkflow"",
        proj.""Id_WorksheetWorkflowDataMap"",
        proj.""UserOrderNo"",
        proj.""NumberRangeEntry"",
        proj.""ProjectReference"",
        proj.""Id_PaymentCondition"",
        proj.""Id_BillingFrame"",
        proj.""Id_Organisation"",
        proj.""Id_Creator"",
        proj.""Id_DefaultRate"",
        oum.""Id_AppUser""
    FROM ""Project"" proj
    LEFT JOIN ""OrganisationUserMap"" oum ON oum.""Id_Organisation"" = proj.""Id_Organisation""
)
SELECT
    pwu.*,
    COALESCE((
        SELECT TRUE FROM ""Worksheet"" ws
        JOIN ""WorksheetStatusLookup"" wsl ON wsl.""WorksheetStatusLookup_Id"" = ws.""Id_CurrentStatus""
        WHERE ws.""Id_Project"" = pwu.""Project_Id""
          AND wsl.""AllowModifications"" = FALSE
        LIMIT 1
    ), FALSE) AS ""NoModifications""
FROM ""ProjectsWithUsers"" AS pwu;
");

            // Views/Organisation/UserOrganisationMappings.sql
            migrationBuilder.Sql(@"
CREATE VIEW ""UserOrganisationMappings"" AS
SELECT DISTINCT
    au.*,
    oum.""Id_Organisation"" AS ""Organisation_Id""
FROM ""AppUser"" au
    JOIN ""OrganisationUserMap"" oum ON oum.""Id_AppUser"" = au.""AppUser_ID"";
");

            // Views/Reporting/PerDayReporting.sql
            // FOR XML PATH string aggregation replaced with string_agg()
            migrationBuilder.Sql(@"
CREATE VIEW ""PerDayReporting"" AS
WITH wis AS (
    SELECT ws.*, wisl.""Description""
    FROM ""WorksheetItemStatus"" ws
        JOIN ""WorksheetItemStatusLookup"" wisl
            ON wisl.""WorksheetItemStatusLookup_Id"" = ws.""Id_WorksheetItemStatusLookup""
)
SELECT
    wi.""Id_Worksheet"",
    wi.""DateOfAction"",
    MIN(wi.""FromTime"")                  AS ""FromTime"",
    MAX(wi.""ToTime"")                    AS ""ToTime"",
    p.""Project_Id"",
    w.""Id_CurrentStatus"",
    p.""Id_Creator"",
    MAX(wi.""ToTime"") - MIN(wi.""FromTime"") AS ""Timespan"",
    SUM(wi.""ToTime""  - wi.""FromTime"")    AS ""WorkTimespan"",
    (SELECT string_agg(wis2.""Description"", ', ')
     FROM wis wis2
     WHERE wis2.""DateOfAction"" = wi.""DateOfAction""
       AND wis2.""Id_Worksheet""  = wi.""Id_Worksheet"")   AS ""WorksheetActionsCsv""
FROM ""WorksheetItem"" wi
LEFT JOIN ""Worksheet"" w ON w.""Worksheet_Id"" = wi.""Id_Worksheet""
LEFT JOIN ""Project""   p ON p.""Project_Id""   = w.""Id_Project""
GROUP BY wi.""Id_Worksheet"", wi.""DateOfAction"",
         p.""Id_Creator"", w.""Id_CurrentStatus"", p.""Project_Id"";
");

            // Views/Reporting/ProjectOverviewReporting.sql
            migrationBuilder.Sql(@"
CREATE VIEW ""ProjectOverviewReporting"" AS
WITH ""wsTimes"" (""Id_Creator"", ""hoursInWs"", ""Id_Worksheet"", ""Id_ProjectItemRate"") AS (
    SELECT
        wi.""Id_Creator"",
        SUM(wi.""ToTime"" - wi.""FromTime"") AS ""hoursInWs"",
        wi.""Id_Worksheet"",
        wi.""Id_ProjectItemRate""
    FROM ""WorksheetItem"" wi
    GROUP BY wi.""Id_Creator"", wi.""Id_Worksheet"", wi.""Id_ProjectItemRate""
)
SELECT
    w.""Id_Creator"",
    ws.""Id_Project""         AS ""ProjectId"",
    p.""Name""                AS ""ProjectName"",
    SUM(w.""hoursInWs"") / 60                    AS ""WorkedHours"",
    (SUM(w.""hoursInWs"") / 60) * pir.""Rate""   AS ""Earned"",
    pir.""Rate""              AS ""Honorar"",
    p.""UserOrderNo""
FROM ""Project"" p
    RIGHT JOIN ""Worksheet""        ws  ON ws.""Id_Project""        = p.""Project_Id""
    RIGHT JOIN ""wsTimes""          w   ON w.""Id_Worksheet""        = ws.""Worksheet_Id""
    RIGHT JOIN ""ProjectItemRate""  pir ON pir.""ProjectItemRate_Id"" = w.""Id_ProjectItemRate""
WHERE worksheet_is_submitted(ws.""Worksheet_Id"") = TRUE
  AND p.""Hidden"" = FALSE
GROUP BY ws.""Id_Project"", pir.""Rate"", w.""Id_Creator"", p.""Name"", p.""UserOrderNo"";
");

            // Views/Reporting/ProjectReporting.sql
            migrationBuilder.Sql(@"
CREATE VIEW ""ProjectReporting"" AS
WITH ""projectRateOne"" AS (
    SELECT COUNT(1) AS ""MoreThenOne"", pir.""Id_Project""
    FROM ""ProjectItemRate"" AS pir
    GROUP BY pir.""Id_Project""
),
""projectRate"" AS (
    SELECT
        pir.*,
        (CASE WHEN pro.""MoreThenOne"" > 1 THEN 1 ELSE 0 END) AS ""MoreThenOne""
    FROM ""ProjectItemRate"" AS pir
        JOIN ""projectRateOne"" pro ON pro.""Id_Project"" = pir.""Id_Project""
        JOIN ""Project""        proj ON pro.""Id_Project"" = proj.""Project_Id""
    WHERE pir.""ProjectItemRate_Id"" = proj.""Id_DefaultRate""
)
SELECT
    proj.""Project_Id"",
    proj.""Name"",
    proj.""Hidden"",
    proj.""Id_Creator"",
    pr.""MoreThenOne"" AS ""MoreRatesKnown"",
    pr.""Rate""        AS ""Honorar"",
    pr.""TaxRate""
FROM ""Project"" proj
    JOIN ""projectRate"" pr ON pr.""Id_Project"" = proj.""Project_Id"";
");

            // Views/Reporting/SubmittedWorksheetsReporting.sql
            migrationBuilder.Sql(@"
CREATE VIEW ""SubmittedWorksheetsReporting"" AS
WITH ""wskItem"" AS (
    SELECT
        wi.""Id_Worksheet"",
        SUM(wi.""ToTime"" - wi.""FromTime"")                                AS ""Timespan"",
        (SUM(wi.""ToTime"" - wi.""FromTime"") / 60.0 * pir.""Rate"")        AS ""BeforeTaxes"",
        (SUM(wi.""ToTime"" - wi.""FromTime"") / 60.0 * pir.""Rate"" / 100 * pir.""TaxRate"") AS ""Taxes"",
        pir.""Rate"",
        pir.""TaxRate""
    FROM ""WorksheetItem"" wi
        JOIN ""ProjectItemRate"" pir ON pir.""ProjectItemRate_Id"" = wi.""Id_ProjectItemRate""
    GROUP BY wi.""Id_Worksheet"", pir.""Rate"", pir.""TaxRate""
)
SELECT
    pdr.*,
    wi.""Timespan"",
    wi.""Rate"",
    wi.""TaxRate"",
    p.""Name"",
    wi.""BeforeTaxes"",
    wi.""Taxes""
FROM ""Worksheet"" AS pdr
    JOIN ""wskItem"" wi ON wi.""Id_Worksheet"" = pdr.""Worksheet_Id""
    JOIN ""Project""  p  ON p.""Project_Id""   = pdr.""Id_Project""
WHERE worksheet_is_submitted(pdr.""Worksheet_Id"") = TRUE;
");

            // Views/Reporting/WorksheetItemReporting.sql
            // FOR XML PATH string aggregation replaced with string_agg()
            migrationBuilder.Sql(@"
CREATE VIEW ""WorksheetItemReporting"" AS
WITH wis AS (
    SELECT ws.*, wisl.""Description""
    FROM ""WorksheetItemStatus"" ws
        JOIN ""WorksheetItemStatusLookup"" wisl
            ON wisl.""WorksheetItemStatusLookup_Id"" = ws.""Id_WorksheetItemStatusLookup""
)
SELECT
    wi.""WorksheetItem_Id"",
    wi.""Id_Worksheet"",
    wi.""DateOfAction"",
    wi.""FromTime"",
    wi.""ToTime"",
    wi.""Hidden"",
    wi.""Comment"",
    p.""Project_Id"",
    wsl.""DisplayKey""               AS ""StatusCodeKey"",
    p.""Id_Creator"",
    wi.""ToTime"" - wi.""FromTime""  AS ""Timespan"",
    (SELECT string_agg(wis2.""Description"", ',')
     FROM wis wis2
     WHERE wis2.""DateOfAction"" = wi.""DateOfAction""
       AND wis2.""Id_Worksheet""  = w.""Worksheet_Id"")  AS ""WorksheetActionsCsv""
FROM ""WorksheetItem"" wi
LEFT JOIN ""Worksheet""          w   ON w.""Worksheet_Id""              = wi.""Id_Worksheet""
LEFT JOIN ""Project""            p   ON p.""Project_Id""                = w.""Id_Project""
LEFT JOIN ""WorksheetStatusLookup"" wsl
    ON wsl.""WorksheetStatusLookup_Id"" = w.""Id_CurrentStatus"";
");

            // Views/Reporting/WorksheetItemsStatusReporting.sql
            migrationBuilder.Sql(@"
CREATE VIEW ""WorksheetItemsStatusReporting"" AS
SELECT
    wisl.""Description"",
    COUNT(wis.""Id_WorksheetItemStatusLookup"") AS ""Counts"",
    p.""Project_Id"",
    p.""Id_Creator""
FROM ""WorksheetItemStatus"" wis
    JOIN ""WorksheetItemStatusLookup"" wisl
        ON wisl.""WorksheetItemStatusLookup_Id"" = wis.""Id_WorksheetItemStatusLookup""
    JOIN ""Worksheet"" ws ON ws.""Worksheet_Id"" = wis.""Id_Worksheet""
    JOIN ""Project""   p  ON p.""Project_Id""   = ws.""Id_Project""
GROUP BY wisl.""Description"", p.""Project_Id"", p.""Id_Creator"";
");

            // Views/Reporting/WorksheetReporting.sql
            migrationBuilder.Sql(@"
CREATE VIEW ""WorksheetReporting"" AS
SELECT
    w.""Worksheet_Id"",
    w.""StartTime"",
    w.""EndTime"",
    w.""Id_Project"",
    w.""Hidden"",
    wsl.""DisplayKey"" AS ""StatusCodeKey"",
    w.""Id_Creator""
FROM ""Worksheet"" w
    JOIN ""WorksheetStatusLookup"" wsl
        ON wsl.""WorksheetStatusLookup_Id"" = w.""Id_CurrentStatus"";
");

            // Views/SubmittedProjects.sql
            // TOP replaced with LIMIT; ORDER BY placed outside the GROUP BY subquery
            migrationBuilder.Sql(@"
CREATE VIEW ""SubmittedProjects"" AS
SELECT
    p.""Project_Id"",
    COUNT(w.""Id_Project"")        AS ""WCount"",
    p.""Id_Creator"",
    MAX(wsItem.""DateOfAction"")   AS ""LastAction""
FROM ""Project""       p
    RIGHT JOIN ""Worksheet""     w      ON w.""Id_Project""    = p.""Project_Id""
    RIGHT JOIN ""WorksheetItem"" wsItem ON wsItem.""Id_Worksheet"" = w.""Worksheet_Id""
WHERE worksheet_is_submitted(w.""Worksheet_Id"") = TRUE
GROUP BY p.""Project_Id"", p.""Id_Creator""
ORDER BY MAX(wsItem.""DateOfAction"") DESC
LIMIT 1000;
");

            // Views/WorksheetComments.sql
            migrationBuilder.Sql(@"
CREATE VIEW ""WorksheetComments"" AS
SELECT DISTINCT
    wsItem.""Id_Creator"",
    wsItem.""Comment"",
    wsItem.""Id_Worksheet""
FROM ""WorksheetItem"" wsItem;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS ""WorksheetComments"";");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS ""SubmittedProjects"";");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS ""WorksheetReporting"";");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS ""WorksheetItemsStatusReporting"";");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS ""WorksheetItemReporting"";");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS ""SubmittedWorksheetsReporting"";");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS ""ProjectReporting"";");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS ""ProjectOverviewReporting"";");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS ""PerDayReporting"";");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS ""UserOrganisationMappings"";");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS ""ProjectsInOrganisation"";");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS ""OrganisationWorksheets"";");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS ""OrdersAggregated"";");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS ""DashboardWorksheets"";");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS ""ClientStructureWithRights"";");
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS worksheet_is_submitted(uuid);");
        }
    }
}
