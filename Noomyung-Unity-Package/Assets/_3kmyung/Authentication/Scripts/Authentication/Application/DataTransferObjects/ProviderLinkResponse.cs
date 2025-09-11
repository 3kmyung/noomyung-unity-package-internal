using System;

namespace _3kmyung.Authentication.Application.DataTransferObjects
{
    public sealed record ProviderLinkResponse
    {
        public bool IsSuccess { get; }
        public string? ErrorMessage { get; }

        public ProviderLinkResponse(bool isSuccess, string? errorMessage = null)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }
    }
}
