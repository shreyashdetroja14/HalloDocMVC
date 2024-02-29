using HalloDocEntities.Data;
using HalloDocEntities.Models;
using HalloDocRepository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocRepository.Implementation
{
    public class NotesAndLogsRepository : INotesAndLogsRepository
    {
        private readonly HalloDocContext _context;

        public NotesAndLogsRepository(HalloDocContext context)
        {
            _context = context;
        }

        public async Task<RequestNote> AddRequestNote(RequestNote requestNote)
        {
            _context.Add(requestNote);
            await _context.SaveChangesAsync();

            return requestNote;
        }

        public async Task<RequestNote> GetNoteByRequestId(int requestId)
        {
            var notes = await _context.RequestNotes.FirstOrDefaultAsync(x => x.RequestId == requestId);
            return notes;
        }

        public IQueryable<RequestStatusLog> GetStatusLogsByRequestId(int requestId)
        {
            var statusLogs = _context.RequestStatusLogs.AsQueryable().Where(x => x.RequestId == requestId);
            return statusLogs;
        }

        public async Task<bool> UpdateRequestNote(RequestNote requestNote)
        {
            _context.RequestNotes.Update(requestNote);
            await _context.SaveChangesAsync();
            
            return true;
        }
    }
}
