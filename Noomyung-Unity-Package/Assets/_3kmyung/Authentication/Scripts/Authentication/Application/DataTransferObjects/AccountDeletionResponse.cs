using System;

namespace _3kmyung.Authentication.Application.DataTransferObjects
{
    public sealed record AccountDeletionResponse
    {
        public bool IsSuccess { get; }
        public string? ErrorMessage { get; }

        public AccountDeletionResponse(bool isSuccess, string? errorMessage = null)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }
    }
}
