using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CtaLineaWebApi.Application.Commands
{
    public class TtServiceImportRequest
        : IRequest<bool>
    {
        public IFormFile File { get; private set; }
        public string User { get; set; }

        public TtServiceImportRequest (IFormFile file)
        {
            this.File = file;
        }
    }
}
