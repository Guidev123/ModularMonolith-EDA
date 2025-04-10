﻿using SalesSystem.SharedKernel.Messages;

namespace SalesSystem.Register.Application.Events
{
    public record UserCreatedEvent(
        Guid Id, string Name,
        string Email, string Document,
        DateTime BirthDate
        ) : Event;
}