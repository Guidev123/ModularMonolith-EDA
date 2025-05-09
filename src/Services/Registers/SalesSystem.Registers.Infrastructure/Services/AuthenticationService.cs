﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using SalesSystem.Email;
using SalesSystem.Email.Models;
using SalesSystem.Registers.Application.Commands.Authentication.AddUserRole;
using SalesSystem.Registers.Application.Commands.Authentication.CreateRole;
using SalesSystem.Registers.Application.Commands.Authentication.Delete;
using SalesSystem.Registers.Application.Commands.Authentication.ForgetPassword;
using SalesSystem.Registers.Application.Commands.Authentication.ResetPassword;
using SalesSystem.Registers.Application.Commands.Authentication.SignIn;
using SalesSystem.Registers.Application.Commands.Authentication.SignUp;
using SalesSystem.Registers.Application.DTOs;
using SalesSystem.Registers.Application.Services;
using SalesSystem.Registers.Infrastructure.Mappers;
using SalesSystem.Registers.Infrastructure.Models;
using SalesSystem.SharedKernel.Enums;
using SalesSystem.SharedKernel.Notifications;
using SalesSystem.SharedKernel.Responses;

namespace SalesSystem.Registers.Infrastructure.Services
{
    public sealed class AuthenticationService(SignInManager<User> signInManager,
                                              INotificator notificator,
                                              UserManager<User> userManager,
                                              RoleManager<IdentityRole> roleManager,
                                              IEmailService emailService,
                                              IJwtGeneratorService jwtGeneratorService)
                                            : IAuthenticationService
    {
        public async Task<Response<SignUpUserResponse>> RegisterAsync(SignUpUserCommand command)
        {
            var user = command.MapToUser();

            var result = await userManager.CreateAsync(user, command.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    notificator.HandleNotification(new(item.Description));
                }

                return Response<SignUpUserResponse>.Failure(GetNotifications());
            }

            var userIdentity = await FindByUserEmailAsync(command.Email);
            if (!userIdentity.IsSuccess || userIdentity.Data is null)
            {
                notificator.HandleNotification(new("Fail to create user."));
                return Response<SignUpUserResponse>.Failure(GetNotifications());
            }

            return Response<SignUpUserResponse>.Success(new(userIdentity.Data.UserId), code: 201);
        }

        public async Task<Response<SignInUserResponse>> SignInAsync(SignInUserCommand command)
        {
            var result = await signInManager.PasswordSignInAsync(command.Email, command.Password, false, true);
            if (!result.Succeeded)
            {
                notificator.HandleNotification(new("Invalid user credentials."));
                return Response<SignInUserResponse>.Failure(GetNotifications());
            }

            if (result.IsLockedOut)
            {
                notificator.HandleNotification(new("You cannot SignIn now, try later."));
                return Response<SignInUserResponse>.Failure(GetNotifications());
            }

            return Response<SignInUserResponse>.Success(await jwtGeneratorService.JwtGenerator(command.Email));
        }

        public async Task<Response<UserDto>> FindByUserEmailAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email).ConfigureAwait(false);
            if (user is null)
            {
                notificator.HandleNotification(new("User not found."));
                return Response<UserDto>.Failure(GetNotifications());
            }

            return Response<UserDto>.Success(new(Guid.Parse(user.Id), user.Email ?? string.Empty));
        }

        public async Task<Response<DeleteUserResponse>> DeleteAsync(DeleteUserCommand command)
        {
            var user = await userManager.FindByIdAsync(command.Id.ToString());
            if (user is null)
            {
                notificator.HandleNotification(new("User not found."));
                return Response<DeleteUserResponse>.Failure(GetNotifications());
            }

            var delete = await userManager.DeleteAsync(user);
            if (!delete.Succeeded)
            {
                foreach (var item in delete.Errors)
                {
                    notificator.HandleNotification(new(item.Description));
                }

                return Response<DeleteUserResponse>.Failure(GetNotifications());
            }

            return Response<DeleteUserResponse>.Success(new(Guid.Parse(user.Id)), code: 204);
        }

        public async Task<Response<ResetPasswordUserResponse>> ResetPasswordAsync(ResetPasswordUserCommand command)
        {
            var user = await userManager.FindByEmailAsync(command.Email);
            if (user is null)
            {
                notificator.HandleNotification(new("User not found."));
                return Response<ResetPasswordUserResponse>.Failure(GetNotifications(), code: 404);
            }

            var result = await userManager.ResetPasswordAsync(user, command.Token, command.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    notificator.HandleNotification(new(item.Description));
                }

                return Response<ResetPasswordUserResponse>.Failure(GetNotifications());
            }

            return Response<ResetPasswordUserResponse>.Success(default, code: 204);
        }

        public async Task<Response<ForgetPasswordUserResponse>> GeneratePasswordResetTokenAsync(ForgetPasswordUserCommand command)
        {
            var user = await userManager.FindByEmailAsync(command.Email);
            if (user is null)
            {
                notificator.HandleNotification(new("User not found."));
                return Response<ForgetPasswordUserResponse>.Failure(GetNotifications(), code: 404);
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var param = new Dictionary<string, string?>
            {
                {"token", token },
                {"email", command.Email}
            };

            var callback = QueryHelpers.AddQueryString(command.ClientUrlToResetPassword, param);

            var message = new EmailMessage(command.Email, "Link to reset password.", callback);
            await emailService.SendAsync(message);

            return Response<ForgetPasswordUserResponse>.Success(default, code: 204);
        }

        public async Task<Response<AddUserRoleResponse>> AddRoleToUserAsync(AddUserRoleCommand command)
        {
            var roleIsValid = await RoleIsValidAsync(command.RoleName);
            if (!roleIsValid)
            {
                notificator.HandleNotification(new("Invalid role name."));
                return Response<AddUserRoleResponse>.Failure(GetNotifications());
            }

            var user = await userManager.FindByEmailAsync(command.Email);
            if (user is null)
            {
                notificator.HandleNotification(new("User not found."));
                return Response<AddUserRoleResponse>.Failure(GetNotifications(), code: 404);
            }

            var result = await userManager.AddToRoleAsync(user, command.RoleName);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    notificator.HandleNotification(new(item.Description));
                }
                return Response<AddUserRoleResponse>.Failure(GetNotifications());
            }

            return Response<AddUserRoleResponse>.Success(default, code: 204);
        }

        public async Task<Response<CreateRoleResponse>> CreateRoleAsync(CreateRoleCommand command)
        {
            if (!RoleIsInEnum(command.RoleName))
            {
                notificator.HandleNotification(new("Invalid role name. Must be a valid role from enum UserRoles."));
                return Response<CreateRoleResponse>.Failure(GetNotifications());
            }

            if (await roleManager.RoleExistsAsync(command.RoleName))
            {
                notificator.HandleNotification(new($"Role '{command.RoleName}' already exists."));
                return Response<CreateRoleResponse>.Failure(GetNotifications());
            }

            var role = new IdentityRole(command.RoleName);
            var result = await roleManager.CreateAsync(role);

            if (!result.Succeeded || role.Name is null)
            {
                foreach (var error in result.Errors)
                {
                    notificator.HandleNotification(new(error.Description));
                }
                return Response<CreateRoleResponse>.Failure(GetNotifications());
            }

            var roleResult = new CreateRoleResponse(Guid.Parse(role.Id));
            return Response<CreateRoleResponse>.Success(roleResult, code: 201);
        }

        public async Task<Response<IReadOnlyCollection<string>>> FindRolesByUserIdAsync(Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null)
            {
                notificator.HandleNotification(new("User not found."));
                return Response<IReadOnlyCollection<string>>.Failure(GetNotifications(), code: 404);
            }

            return Response<IReadOnlyCollection<string>>.Success([.. await userManager.GetRolesAsync(user)]);
        }

        private async Task<bool> RoleIsValidAsync(string roleName)
        {
            var roleIsValid = RoleIsInEnum(roleName);

            var roleExists = await roleManager.RoleExistsAsync(roleName);

            return roleIsValid && roleExists;
        }

        private static bool RoleIsInEnum(string roleName)
            => Enum.GetNames<EUserRoles>().Any(name => name.Equals(roleName, StringComparison.OrdinalIgnoreCase));

        private List<string> GetNotifications()
            => [.. notificator.GetNotifications().Select(x => x.Message)];
    }
}