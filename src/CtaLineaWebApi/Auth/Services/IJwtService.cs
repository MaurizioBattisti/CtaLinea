using CtaLineaWebApi.Auth.Model;
using System;
using System.Collections.Generic;

namespace CtaLineaWebApi.Auth.Services
{

    public interface IJwtService
    {
        JwtResult GenTokenkey(
            string userName,
            string description,
            string eMail,
            Guid? associateId,
            IEnumerable<string> roles,
            bool mustChangePAssword = false
            );
    }
}
