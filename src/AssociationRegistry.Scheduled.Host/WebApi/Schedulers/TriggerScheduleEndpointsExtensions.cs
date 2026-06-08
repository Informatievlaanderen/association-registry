namespace AssociationRegistry.Scheduled.Host.WebApi.Schedulers;

using Bewaartermijnen;
using Erkenningen;
using Erkenningen.Activeer;
using Erkenningen.Verloop;
using Microsoft.AspNetCore.Builder;
using PowerBi;
using Quartz;

public static class TriggerScheduleControllersExtensions
{
    public static void AddScheduleEndpoints(this WebApplication app)
    {
        app.MapPost(
            pattern: "v1/trigger/bewaartermijn",
            handler: async (ISchedulerFactory schedulerFactory) =>
            {
                await TriggerJob(ExpiredBewaartermijnJob.JobName, schedulerFactory);
            }
        );

        app.MapPost(
            pattern: "v1/trigger/powerbi-export",
            handler: async (ISchedulerFactory schedulerFactory) =>
            {
                await TriggerJob(PowerBiExportJob.JobName, schedulerFactory);
            }
        );

        app.MapPost(
            pattern: "v1/trigger/activeer-erkenningen",
            handler: async (ISchedulerFactory schedulerFactory) =>
            {
                await TriggerJob(ActiveerErkenningenJob.JobName, schedulerFactory);
            }
        );

        app.MapPost(
            pattern: "v1/trigger/verloop-erkenningen",
            handler: async (ISchedulerFactory schedulerFactory) =>
            {
                await TriggerJob(VerloopErkenningenJob.JobName, schedulerFactory);
            }
        );
    }

    private static async Task TriggerJob(string jobKeyName, ISchedulerFactory schedulerFactory)
    {
        var scheduler = await schedulerFactory.GetScheduler();
        var jobKey = new JobKey(jobKeyName);
        await scheduler.TriggerJob(jobKey);
    }
}
