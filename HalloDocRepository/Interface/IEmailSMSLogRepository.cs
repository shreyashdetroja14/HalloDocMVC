using HalloDocEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocRepository.Interface
{
    public interface IEmailSMSLogRepository
    {
        #region EMAIL LOGS

        public List<EmailLog> GetEmailLogs();

        IQueryable<EmailLog> GetIqueryableEmailLogs();

        Task<EmailLog> CreateEmailLog(EmailLog emailLog);
             
        #endregion
        
        #region SMS LOGS

        public List<SmsLog> GetSmsLogs();

        IQueryable<SmsLog> GetIqueryableSmsLogs();

        Task<SmsLog> CreateSmsLog(SmsLog smsLog);

        #endregion


    }
}
