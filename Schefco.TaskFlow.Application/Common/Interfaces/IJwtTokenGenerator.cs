using Schefco.TaskFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Schefco.TaskFlow.Application.Common.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(AppUser user);
        string GenerateTempToken(AppUser user);
        ClaimsPrincipal ValidateTempToken(string token);
    }
}
