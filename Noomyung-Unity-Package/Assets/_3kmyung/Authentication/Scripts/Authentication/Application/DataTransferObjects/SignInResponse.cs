using System;
using _3kmyung.Authentication.Domain;

namespace _3kmyung.Authentication.Application.DataTransferObjects
{
    public sealed record SignInResponse
    {
        public PlayerSession? Session { get; }
        public bool IsSuccess { get; }
        public string? ErrorMessage { get; }

        public SignInResponse(PlayerSession? session, bool isSuccess, string? errorMessage = null)
        {
            Session = session;
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }
    }
}
