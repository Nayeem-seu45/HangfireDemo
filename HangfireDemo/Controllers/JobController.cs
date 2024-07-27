using Hangfire;
using HangfireDemo.Jobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HangfireDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        [HttpPost]
        [Route("CreateBackgroundJob")]
        public ActionResult CreateBackgroundJob()
        {
            //BackgroundJob.Enqueue(() => Console.WriteLine("Background Job Trigger"));
            BackgroundJob.Enqueue<TestJob>(x =>x.WriteLog("Background Job Trigger"));
            return Ok();
        }

        [HttpPost]
        [Route("CreateScheduledJob")]
        public ActionResult CreateScheduledJob()
        {
            var scheduleDateTime = DateTime.UtcNow.AddSeconds(5);
            var dateTimeOffset = new DateTimeOffset(scheduleDateTime);

            //BackgroundJob.Schedule(() => Console.WriteLine("Scheduled Job Trigger"), dateTimeOffset);
            BackgroundJob.Schedule<TestJob>(x => x.WriteLog("Scheduled Job Trigger"), dateTimeOffset);
            return Ok();
        }

        [HttpPost]
        [Route("CreateContinuationJob")]
        public ActionResult CreateContinuationJob()
        {
            var scheduleDateTime = DateTime.UtcNow.AddSeconds(5);
            var dateTimeOffset = new DateTimeOffset(scheduleDateTime);

            //var jobId = BackgroundJob.Schedule(() => Console.WriteLine("Scheduled Job 2 Trigger"), dateTimeOffset);
            var jobId = BackgroundJob.Schedule<TestJob>(x => x.WriteLog("Scheduled Job 2 Trigger"), dateTimeOffset);

            var job2Id = BackgroundJob.ContinueJobWith<TestJob>(jobId, x => x.WriteLog("Continuation Job 1 Trigger"));
            var job3Id = BackgroundJob.ContinueJobWith<TestJob>(job2Id,x => x.WriteLog("Continuation Job 2 Trigger"));
            var job4Id = BackgroundJob.ContinueJobWith<TestJob>(job3Id, x => x.WriteLog("Continuation Job 3 Trigger"));

            return Ok();
        }

        [HttpPost]
        [Route("CreateRecurringJob")]
        public ActionResult CreateRecurringJob()
        {
            RecurringJob.AddOrUpdate<TestJob>("RecurringJob1", x => x.WriteLog("Recurring job Trigger"), "* * * * *");
            return Ok();
        }
    }
}
