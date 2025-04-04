﻿using SalesSystem.SharedKernel.Messages;

namespace SalesSystem.Register.Application.Commands.Authentication.Delete
{
    public record DeleteUserCommand : Command<DeleteUserResponse>
    {
        public DeleteUserCommand(Guid id, string email)
        {
            AggregateId = id;
            Id = id;
            Email = email;
        }

        public Guid Id { get; }
        public string Email { get; } = string.Empty;
        public override bool IsValid()
        {
            SetValidationResult(new DeleteUserValidation().Validate(this));
            return ValidationResult!.IsValid;
        }
    }
}