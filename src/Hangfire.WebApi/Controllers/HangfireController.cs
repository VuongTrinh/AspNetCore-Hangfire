using Microsoft.AspNetCore.Mvc;
using System;

namespace Hangfire.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HangfireController : ControllerBase
    {
        [HttpPost]
        [Route("welcome")]
        public IActionResult Welcome(string userName)
        {
            var jobId = BackgroundJob.Enqueue(() => SendWelcomeMail(userName));
            return Ok($"Job Id {jobId} Completed. Welcome Mail Sent!");
        }

        public void SendWelcomeMail(string userName)
        {
            //Logic to Mail the user
            Console.WriteLine($"Welcome to our application, {userName}");
        }

        [HttpPost]
        [Route("delayedWelcome")]
        public IActionResult DelayedWelcome(string userName)
        {
            var jobId = BackgroundJob.Schedule(() => SendDelayedWelcomeMail(userName), TimeSpan.FromMinutes(2));
            return Ok($"Job Id {jobId} Completed. Delayed Welcome Mail Sent!");
        }

        public void SendDelayedWelcomeMail(string userName)
        {
            //Logic to Mail the user
            Console.WriteLine($"Welcome to our application, {userName}");
        }

        //Our Customer has a subscription to our service. 
        //We would obviously have to send him/her a reminder about payment or the invoice itself. 
        //This calls the need for a Recurring Job, where I can send my customer emails on a monthly basis. 
        //This is supported in Hangfire using CRON schedule.
        [HttpPost]
        [Route("invoice")]
        public IActionResult Invoice(string userName)
        {
            RecurringJob.AddOrUpdate(() => SendInvoiceMail(userName), Cron.Monthly);
            return Ok($"Recurring Job Scheduled. Invoice will be mailed Monthly for {userName}!");
        }

        public void SendInvoiceMail(string userName)
        {
            //Logic to Mail the user
            Console.WriteLine($"Here is your invoice, {userName}");
        }



        // A user decides to unsubscribe from your service.
        //After he confirms his action (Maybe clicking the unsubscribe button), we(the application) have to unsubscribe him from the system and send him a confirmation mail after that as well.
        //So, the First job is to actually unsubscribe the user.
        //The second job is to send a mail confirming the action.
        //The second job should be executed only after the first job is completed properly
        [HttpPost]
        [Route("unsubscribe")]
        public IActionResult Unsubscribe(string userName)
        {
            var jobId = BackgroundJob.Enqueue(() => UnsubscribeUser(userName));
            BackgroundJob.ContinueJobWith(jobId, () => Console.WriteLine($"Sent Confirmation Mail to {userName}"));
            return Ok($"Unsubscribed");
        }

        public void UnsubscribeUser(string userName)
        {
            //Logic to Unsubscribe the user
            Console.WriteLine($"Unsubscribed {userName}");
        }
    }
}
