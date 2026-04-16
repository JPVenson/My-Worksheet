using System;

namespace MyWorksheet.Website.Server.Services.Reporting;

public delegate void OnReportPositionChanged(Guid reportId, int newPosition, Guid executingUser);