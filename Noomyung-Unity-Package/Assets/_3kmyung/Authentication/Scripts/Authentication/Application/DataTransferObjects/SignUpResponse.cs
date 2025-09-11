using System;

namespace _3kmyung.Authentication.Application.DataTransferObjects
{
    public sealed record SignUpResponse
    {
        public bool IsSuccess { get; }
        public string? ErrorMessage { get; }
        public string? PlayerId { get; }

        public SignUpResponse(bool isSuccess, string? errorMessage = null, string? playerId = null)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            PlayerId = playerId;
        }
    }
}
