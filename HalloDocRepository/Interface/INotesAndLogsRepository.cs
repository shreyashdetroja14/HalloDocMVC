using HalloDocEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocRepository.Interface
{
    public interface INotesAndLogsRepository
    {
        Task<RequestNote> GetNoteByRequestId(int requestId);

        IQueryable<RequestStatusLog> GetStatusLogsByRequestId(int requestId);

        Task<RequestNote> AddRequestNote(RequestNote requestNote);

        Task<bool> UpdateRequestNote(RequestNote requestNote);
    }
}
