using HalloDocEntities.Data;
using HalloDocEntities.Models;
using HalloDocRepository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocRepository.Implementation
{
    public class EmailSMSLogRepository : IEmailSMSLogRepository
    {
        private readonly HalloDocContext _context;

        public EmailSMSLogRepository(HalloDocContext context)
        {
            _context = context;
        }

        public async Task<EmailLog> CreateEmailLog(EmailLog emailLog)
        {
            _context.EmailLogs.Add(emailLog);
            await _context.SaveChangesAsync();

            return emailLog;
        }

        public async Task<SmsLog> CreateSmsLog(SmsLog smsLog)
        {
            _context.SmsLogs.Add(smsLog);
            await _context.SaveChangesAsync();

            return smsLog;
        }

        public List<EmailLog> GetEmailLogs()
        {
            return _context.EmailLogs.ToList();
        }

        public IQueryable<EmailLog> GetIqueryableEmailLogs()
        {
            return _context.EmailLogs.AsQueryable();
        }

        public IQueryable<SmsLog> GetIqueryableSmsLogs()
        {
            return _context.SmsLogs.AsQueryable();
        }

        public List<SmsLog> GetSmsLogs()
        {
            return _context.SmsLogs.ToList();
        }
    }
}
