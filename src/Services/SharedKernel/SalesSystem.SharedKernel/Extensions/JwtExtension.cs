﻿namespace SalesSystem.SharedKernel.Extensions
{
    public class JwtExtension
    {
        public string? Secret { get; set; }
        public int ExpiresAt { get; set; }
        public string? Issuer { get; set; }
        public string? ValidAt { get; set; }
    }
}