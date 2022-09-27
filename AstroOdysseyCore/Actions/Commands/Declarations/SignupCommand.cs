﻿using MediatR;

namespace AstroOdysseyCore
{
    public class SignupCommand : IRequest<ServiceResponse>
    {
        public string FullName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string GameId { get; set; } = string.Empty;
    }
}
