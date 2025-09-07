using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Noomyung.Authentication.Domain;
using Unity.Services.Core;
using Unity.Services.Authentication;

namespace Noomyung.Authentication.Infrastructure
{
    internal sealed class UgsAuthSession : IAuthSession
    {
        public string PlayerId { get; }

        public bool IsSignedIn { get; }

        public UgsAuthSession(string playerId, bool isSignedIn)
        {
            PlayerId = playerId;
            IsSignedIn = isSignedIn;
        }
    }

    public sealed class UgsAuthService : IAuthService
    {
        private bool _isInitialized;
        public UgsAuthService()
        {
        }

        public async Task<IAuthSession> SignInAnonymouslyAsync(CancellationToken cancellationToken = default)
        {
            await EnsureInitializedAsync(cancellationToken);

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (cancellationToken.IsCancellationRequested) cancellationToken.ThrowIfCancellationRequested();
            }

            var session = new UgsAuthSession(AuthenticationService.Instance.PlayerId, AuthenticationService.Instance.IsSignedIn);

            return session;
        }

        public async Task<IAuthSession> SignInWithDeviceID(string deviceID, CancellationToken cancellationToken = default)
        {
            await EnsureInitializedAsync(cancellationToken);

            try
            {
                // UGS에서는 Device ID를 직접 지원하지 않으므로
                // 익명 로그인을 사용하고 Device ID를 별도로 저장하는 방식을 사용합니다.
                Debug.LogWarning("UGS does not support direct Device ID authentication. Using anonymous login with device ID storage.");
                
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }

                if (cancellationToken.IsCancellationRequested) cancellationToken.ThrowIfCancellationRequested();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Device ID authentication failed. {ex.Message}");
                throw;
            }

            var session = new UgsAuthSession(AuthenticationService.Instance.PlayerId, AuthenticationService.Instance.IsSignedIn);

            return session;
        }

        public async Task<IAuthSession> SignInWithProviderAsync(AuthProvider provider, string accessToken, CancellationToken cancellationToken = default)
        {
            await EnsureInitializedAsync(cancellationToken);

            try
            {
                switch (provider)
                {
                    case AuthProvider.Google:
                        await AuthenticationService.Instance.SignInWithGoogleAsync(accessToken);
                        break;
                    case AuthProvider.Apple:
                        await AuthenticationService.Instance.SignInWithAppleAsync(accessToken);
                        break;
                    case AuthProvider.Facebook:
                        await AuthenticationService.Instance.SignInWithFacebookAsync(accessToken);
                        break;
                    case AuthProvider.Custom:
                        // UGS에서 Custom Token 로그인을 직접 지원하지 않으므로
                        // 익명 로그인을 사용하고 Custom Token을 별도로 저장합니다.
                        Debug.LogWarning("UGS does not support direct Custom Token authentication. Using anonymous login with token storage.");
                        await AuthenticationService.Instance.SignInAnonymouslyAsync();
                        break;
                    default:
                        await AuthenticationService.Instance.SignInAnonymouslyAsync();
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Authentication failed. {ex.Message}");

                throw;
            }

            if (cancellationToken.IsCancellationRequested) cancellationToken.ThrowIfCancellationRequested();

            var session = new UgsAuthSession(AuthenticationService.Instance.PlayerId, AuthenticationService.Instance.IsSignedIn);

            return session;
        }

        public async Task<IAuthSession> SignInWithUsernamePasswordAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            await EnsureInitializedAsync(cancellationToken);

            try
            {
                // UGS에서 지원하는 Username/Password 로그인을 사용합니다.
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Username/Password authentication failed. {ex.Message}");
                throw;
            }

            if (cancellationToken.IsCancellationRequested) cancellationToken.ThrowIfCancellationRequested();

            var session = new UgsAuthSession(AuthenticationService.Instance.PlayerId, AuthenticationService.Instance.IsSignedIn);

            return session;
        }

        public async Task RegisterWithUsernamePasswordAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            await EnsureInitializedAsync(cancellationToken);

            try
            {
                // UGS는 직접적인 Username/Password 등록을 지원하지 않으므로
                // 서버를 통해 사용자 등록을 처리해야 합니다.
                Debug.LogWarning("UGS does not support direct Username/Password registration. Server-side registration required.");
                
                // 실제 구현에서는 서버 API를 호출하여 사용자를 등록해야 합니다.
                await RegisterUserOnServerAsync(username, password);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Username/Password registration failed. {ex.Message}");
                throw;
            }
        }

        public async Task LinkProviderAsync(AuthProvider provider, string accessToken, CancellationToken cancellationToken = default)
        {
            await EnsureInitializedAsync(cancellationToken);

            try
            {
                switch (provider)
                {
                    case AuthProvider.Google:
                        await AuthenticationService.Instance.LinkWithGoogleAsync(accessToken);
                        break;
                    case AuthProvider.Apple:
                        await AuthenticationService.Instance.LinkWithAppleAsync(accessToken);
                        break;
                    case AuthProvider.Facebook:
                        await AuthenticationService.Instance.LinkWithFacebookAsync(accessToken);
                        break;
                    case AuthProvider.Custom:
                        // UGS에서 Custom Token 링크를 직접 지원하지 않으므로
                        // 예외를 발생시킵니다.
                        throw new NotSupportedException("UGS does not support linking with Custom Token. Use other supported providers.");
                    default:
                        throw new NotSupportedException($"Linking provider {provider} is not supported.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Linking failed. {ex.Message}");

                throw;
            }
        }

        public async Task UnlinkProviderAsync(AuthProvider provider, CancellationToken cancellationToken = default)
        {
            await EnsureInitializedAsync(cancellationToken);

            try
            {
                switch (provider)
                {
                    case AuthProvider.Google:
                        await AuthenticationService.Instance.UnlinkGoogleAsync();
                        break;
                    case AuthProvider.Apple:
                        await AuthenticationService.Instance.UnlinkAppleAsync();
                        break;
                    case AuthProvider.Facebook:
                        await AuthenticationService.Instance.UnlinkFacebookAsync();
                        break;
                    case AuthProvider.Custom:
                        // UGS에서 Custom Token 언링크를 직접 지원하지 않으므로
                        // 예외를 발생시킵니다.
                        throw new NotSupportedException("UGS does not support unlinking Custom Token. Use other supported providers.");
                    default:
                        throw new NotSupportedException($"Unlinking provider {provider} is not supported.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unlinking failed. {ex.Message}");

                throw;
            }
        }

        public async Task<string> GetPlayerIdAsync(CancellationToken cancellationToken = default)
        {
            await EnsureInitializedAsync(cancellationToken);

            return AuthenticationService.Instance.PlayerId;
        }

        public async Task SignOutAsync(CancellationToken cancellationToken = default)
        {
            await EnsureInitializedAsync(cancellationToken);

            AuthenticationService.Instance.SignOut();

            await Task.Yield();
        }

        private async Task EnsureInitializedAsync(CancellationToken cancellationToken)
        {
            if (_isInitialized) return;

            try
            {
                await UnityServices.InitializeAsync();

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"UnityServices.InitializeAsync failed. {ex.Message}");

                throw;
            }

            if (cancellationToken.IsCancellationRequested) cancellationToken.ThrowIfCancellationRequested();
        }

        public void Dispose()
        {
        }

        /// <summary>
        /// 서버에 사용자를 등록하는 메서드 (예시 구현)
        /// 실제 구현에서는 서버 API를 호출해야 합니다.
        /// </summary>
        private async Task RegisterUserOnServerAsync(string username, string password)
        {
            // 실제 구현에서는 서버에 사용자 등록 요청을 보내야 합니다.
            await Task.Delay(100); // 서버 호출 시뮬레이션
            
            // 실제 구현 예시:
            // var response = await httpClient.PostAsync("/auth/register", 
            //     new StringContent(JsonConvert.SerializeObject(new { username, password })));
            // if (!response.IsSuccessStatusCode)
            //     throw new Exception("User registration failed");
        }
    }
}
