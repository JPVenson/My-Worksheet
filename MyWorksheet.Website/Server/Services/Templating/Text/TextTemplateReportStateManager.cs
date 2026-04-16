using System.Threading.Tasks;
using System;
using MyWorksheet.Website.Server.Services.ObjectChanged;
using MyWorksheet.Website.Server.Services.Reporting;
using MyWorksheet.Website.Shared.ViewModels;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.Templating.Text;

[SingletonService()]
public class TextTemplateReportStateManager : RequireInit
{
    private readonly ITextTemplateManager _textTemplateManager;
    private readonly ObjectChangedService _objectChangedService;

    public TextTemplateReportStateManager(ITextTemplateManager textTemplateManager, ObjectChangedService objectChangedService)
    {
        _textTemplateManager = textTemplateManager;
        _objectChangedService = objectChangedService;
    }

    public override ValueTask InitAsync()
    {
        _textTemplateManager.ReportPositionChanged += _textTemplateManager_ReportPositionChanged;
        return base.InitAsync();
    }

    private async void _textTemplateManager_ReportPositionChanged(Guid reportId, int newPosition, Guid executingUser)
    {
        if (newPosition == -1)
        {
            await _objectChangedService.SendObjectChanged(ChangeEventTypes.Removed, "PreviewReport", reportId, null);
        }
        else
        {
            await _objectChangedService.SendObjectChanged(ChangeEventTypes.Changed, "PreviewReport", reportId, null);
        }
    }
}