using System.Collections.Generic;
using MyWorksheet.Website.Server.Services.Activity.Types;
using MyWorksheet.Website.Shared.Services;
using MyWorksheet.Website.Shared.Services.Activation;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.Activity;

[SingletonService()]
public class ActivityTypesInitiator : RequireInit
{
    public ActivityTypesInitiator(ActivatorService activatorService)
    {
        ActivityTypes.Init(activatorService);
    }
}

public static class ActivityTypes
{
    public static void Init(ActivatorService activatorService)
    {
        WorksheetNotSubmitted = activatorService.ActivateType<ActivityTypeWorksheetExpired>();
        ActivityWebhookDisabled = activatorService.ActivateType<ActivityWebhookDisabled>();
        UserReminder = activatorService.ActivateType<ActivityType>("user_custom_reminder");
        AppError = activatorService.ActivateType<ActivityType>("critical_app_error");
        TestAccountCreated = activatorService.ActivateType<TestAccountCreated>();
        PaymentRecivedReminder = activatorService.ActivateType<PaymentRecivedReminder>();
        AccountCreated = activatorService.ActivateType<AccountCreated>();
        AccountCreatedGiftGranted = activatorService.ActivateType<AccountCreatedGiftGranted>();
        TrackerStillRunning = activatorService.ActivateType<TrackerStillRunning>();
        ScheduledTaskFailedActivity = activatorService.ActivateType<ScheduledTaskFailedActivity>();
        MailWorkflowFailed = activatorService.ActivateType<MailWorkflowFailedActivity>();
        ReportGenerationFailed = activatorService.ActivateType<ReportGenerationFailedActivity>();
    }


    public static ActivityTypeWorksheetExpired WorksheetNotSubmitted { get; private set; }
    //public static ActivityProjectQuotaNearlyExceeded QuotaProjectQuotaNearlyExceeded { get; private set; } = new ActivityProjectQuotaNearlyExceeded();
    public static ActivityWebhookDisabled ActivityWebhookDisabled { get; private set; }
    public static ActivityType UserReminder { get; private set; }
    public static ActivityType AppError { get; private set; }

    public static TestAccountCreated TestAccountCreated { get; private set; }
    public static PaymentRecivedReminder PaymentRecivedReminder { get; private set; }
    public static AccountCreated AccountCreated { get; private set; }
    public static AccountCreatedGiftGranted AccountCreatedGiftGranted { get; private set; }
    public static TrackerStillRunning TrackerStillRunning { get; private set; }
    public static ScheduledTaskFailedActivity ScheduledTaskFailedActivity { get; private set; }
    public static MailWorkflowFailedActivity MailWorkflowFailed { get; set; }
    public static ReportGenerationFailedActivity ReportGenerationFailed { get; set; }

    public static IEnumerable<ActivityType> Yield()
    {
        yield return WorksheetNotSubmitted;
        yield return ActivityWebhookDisabled;
        yield return UserReminder;
        yield return AppError;
        yield return PaymentRecivedReminder;
        yield return TestAccountCreated;
        yield return AccountCreated;
        yield return AccountCreatedGiftGranted;
        yield return TrackerStillRunning;
        yield return ScheduledTaskFailedActivity;
        yield return MailWorkflowFailed;
        yield return ReportGenerationFailed;
    }
}