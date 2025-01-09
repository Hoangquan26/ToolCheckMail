using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolMail.Models;

namespace ToolMail.Controllers
{
    public class ProcessMail
    {
        public static async Task ProcessEmailsAsync(ObservableCollection<InputMail> emailsToProcess, int numberLimitThread)
        {
            var semaphore = new SemaphoreSlim(numberLimitThread);
            var tasks = emailsToProcess.Select(async email =>
            {
                await semaphore.WaitAsync();
                try
                {
                    email.Status = "Xin chào";
                    await ProcessEmailAsync(email);
                }
                finally
                {
                    semaphore.Release();
                }
            }).ToArray();

            await Task.WhenAll(tasks);
        }

        private static async Task ProcessEmailAsync(InputMail email)
        {
        
            await Task.Delay(1000);
            email.Status = email.Checked ? "Active" : "Inactive";
        }

    }
}
