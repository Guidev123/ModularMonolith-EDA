﻿using SalesSystem.SharedKernel.Abstractions;

namespace SalesSystem.Registers.Application.Commands.Authentication.ForgetPassword
{
    public record ForgetPasswordUserCommand : Command<ForgetPasswordUserResponse>
    {
        public ForgetPasswordUserCommand(string email, string clientUrlToResetPassword)
        {
            Email = email;
            ClientUrlToResetPassword = clientUrlToResetPassword;
        }

        public string Email { get; }
        public string ClientUrlToResetPassword { get; private set; }
    }
}