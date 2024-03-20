using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Interface
{
    public interface IMailService
    {
        Task<bool> SendMail(List<string> receiver, string subject, string body, bool isHtml, List<string>? filesToSend);
    }
}
