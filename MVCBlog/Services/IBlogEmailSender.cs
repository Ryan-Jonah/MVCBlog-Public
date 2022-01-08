using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCBlog.Services
{
    public interface IBlogEmailSender : IEmailSender
    {
        //Add a new required function for implementation
        Task SendContactEmailAsync(string emailFrom, string name, string subject, string htmlMessage); 
    }
}
