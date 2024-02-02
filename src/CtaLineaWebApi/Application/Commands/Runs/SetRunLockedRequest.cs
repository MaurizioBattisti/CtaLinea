using MediatR;
using System;
using System.Collections.Generic;

namespace CtaLineaWebApi.Application.Commands.Runs
{
    public class SetRunLockedRequest
        : IRequest<bool>
    {
        public bool IsLocked => this.Date != null;
        public DateTime? Date { get; set; }
        private string? _Note;
        public string? Note 
        {
            get
            {
                if (this.IsLocked == true) return _Note;
                return null;
            }
            set
            {
                _Note = value;
            }
        }
        public IEnumerable<Guid>? RunIds { get; set; }
    }
}
