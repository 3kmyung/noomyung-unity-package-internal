using System;

namespace _3kmyung.Authentication.Application.DataTransferObjects
{
    public sealed record SignUpRequest
    {
        public string Username { get; }
        public string Password { get; }

        public SignUpRequest(string username, string password)
        {
            Username = !string.IsNullOrWhiteSpace(username) ? username : throw new ArgumentException("Username cannot be null or empty", nameof(username));
            Password = !string.IsNullOrWhiteSpace(password) ? password : throw new ArgumentException("Password cannot be null or empty", nameof(password));
        }
    }
}
