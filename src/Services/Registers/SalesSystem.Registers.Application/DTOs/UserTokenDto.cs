﻿namespace SalesSystem.Registers.Application.DTOs
{
    public record UserTokenDto(string Id, string Email, IEnumerable<UserClaimDto> Claims);
}