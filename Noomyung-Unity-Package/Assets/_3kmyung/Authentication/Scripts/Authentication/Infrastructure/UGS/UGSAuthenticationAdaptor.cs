using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Cysharp.Threading.Tasks;
using _3kmyung.Authentication.Domain;
using IAuthenticationPort = _3kmyung.Authentication.Domain.IAuthenticationPort;

namespace _3kmyung.Authentication.Infrastructure
{
    public sealed class UGSAuthenticationAdaptor : MonoBehaviour, IAuthenticationPort
    {
        #region Fields

        private bool _isInitialized;
        private readonly Dictionary<AuthenticationProvider, IProviderAuthenticationAdapter> _providerAdapters;

        #endregion

        #region Constructor

        public UGSAuthenticationAdaptor()
        {
            _providerAdapters = new Dictionary<AuthenticationProvider, IProviderAuthenticationAdapter>
            {
                { AuthenticationProvider.Google, new UGSGoogleAuthenticationAdapter() },
                { AuthenticationProvider.Apple, new UGSAppleAuthenticationAdapter() },
                { AuthenticationProvider.Facebook, new UGSFacebookAuthenticationAdapter() }
            };
        }

        #endregion

        public async Task<IAuthenticationSession> SignInAnonymouslyAsync(CancellationToken cancellationToken = default)
        {
            await EnsureInitializedAsync(cancellationToken);

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync().AsUniTask();

                cancellationToken.ThrowIfCancellationRequested();
            }

            var session = new UGSAuthenticationSession(AuthenticationService.Instance.PlayerId, AuthenticationService.Instance.IsSignedIn);

            return session;
        }

        public async Task<IAuthenticationSession> SignInWithProviderAsync(AuthenticationProvider provider, string accessToken, CancellationToken cancellationToken = default)
        {
            await EnsureInitializedAsync(cancellationToken);

            try
            {
                // Custom Provider는 익명 로그인으로 처리
                if (provider == AuthenticationProvider.Custom)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync().AsUniTask();
                }
                else
                {
                    // Dictionary에서 해당 Provider 어댑터를 찾아 로그인 수행
                    if (_providerAdapters.TryGetValue(provider, out var adapter))
                    {
                        return await adapter.SignInAsync(accessToken, cancellationToken);
                    }
                    else
                    {
                        throw new NotSupportedException($"Provider {provider} is not supported.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Authentication failed. {ex.Message}");
                throw;
            }

            if (cancellationToken.IsCancellationRequested) cancellationToken.ThrowIfCancellationRequested();

            var session = new UGSAuthenticationSession(AuthenticationService.Instance.PlayerId, AuthenticationService.Instance.IsSignedIn);

            return session;
        }

        public async Task<IAuthenticationSession> SignInWithUsernameAndPasswordAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            await EnsureInitializedAsync(cancellationToken);

            try
            {
                // UGS에서 지원하는 Username/Password 로그인을 사용합니다.
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password).AsUniTask();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Username/Password authentication failed. {ex.Message}");
                throw;
            }

            if (cancellationToken.IsCancellationRequested) cancellationToken.ThrowIfCancellationRequested();

            var session = new UGSAuthenticationSession(AuthenticationService.Instance.PlayerId, AuthenticationService.Instance.IsSignedIn);

            return session;
        }

        public async Task RegisterWithUsernamePasswordAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            await EnsureInitializedAsync(cancellationToken);

            try
            {
                // UGS에서 Username/Password 등록을 수행합니다.
                // 이는 실제로는 서버에 사용자를 등록하는 것이 아니라
                // UGS의 Username/Password 인증 시스템에 사용자를 등록하는 것입니다.
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password).AsUniTask();

                Debug.Log($"User registered successfully with username: {username}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Username/Password registration failed. {ex.Message}");
                throw;
            }
        }

        public async Task LinkProviderAsync(AuthenticationProvider provider, string accessToken, CancellationToken cancellationToken = default)
        {
            await EnsureInitializedAsync(cancellationToken);

            try
            {
                // Custom Provider는 지원하지 않음
                if (provider == AuthenticationProvider.Custom)
                {
                    throw new NotSupportedException("UGS does not support linking with Custom Token. Use other supported providers.");
                }

                // Dictionary에서 해당 Provider 어댑터를 찾아 링크 수행
                if (_providerAdapters.TryGetValue(provider, out var adapter))
                {
                    await adapter.LinkAsync(accessToken, cancellationToken);
                }
                else
                {
                    throw new NotSupportedException($"Linking provider {provider} is not supported.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Linking failed. {ex.Message}");
                throw;
            }
        }

        public async Task UnlinkProviderAsync(AuthenticationProvider provider, CancellationToken cancellationToken = default)
        {
            await EnsureInitializedAsync(cancellationToken);

            try
            {
                // Custom Provider는 지원하지 않음
                if (provider == AuthenticationProvider.Custom)
                {
                    throw new NotSupportedException("UGS does not support unlinking Custom Token. Use other supported providers.");
                }

                // Dictionary에서 해당 Provider 어댑터를 찾아 언링크 수행
                if (_providerAdapters.TryGetValue(provider, out var adapter))
                {
                    await adapter.UnlinkProviderAsync(provider, cancellationToken);
                }
                else
                {
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

        public async Task<bool> IsSignedInAsync(CancellationToken cancellationToken = default)
        {
            await EnsureInitializedAsync(cancellationToken);

            return AuthenticationService.Instance.IsSignedIn;
        }

        public async Task SignOutAsync(CancellationToken cancellationToken = default)
        {
            await EnsureInitializedAsync(cancellationToken);

            try
            {
                // IsSignedIn으로 실제 로그인 상태를 확인
                bool isActuallySignedIn = AuthenticationService.Instance.IsSignedIn;
                var currentPlayerId = AuthenticationService.Instance.PlayerId;

                Debug.Log($"UGS SignOut check - IsSignedIn: {isActuallySignedIn}, PlayerId: '{currentPlayerId}'");

                if (isActuallySignedIn)
                {
                    Debug.Log("Starting UGS sign out process...");
                    AuthenticationService.Instance.SignOut();

                    // 로그아웃 상태를 지속적으로 확인
                    await WaitForSignOutCompletion(cancellationToken);

                    Debug.Log("UGS Sign out completed successfully");
                }
                else
                {
                    Debug.Log("User is not signed in (IsSignedIn is false), no need to sign out");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"UGS Sign out failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 로그아웃이 완료될 때까지 상태를 지속적으로 확인합니다.
        /// </summary>
        private async UniTask WaitForSignOutCompletion(CancellationToken cancellationToken)
        {
            const int maxWaitTime = 5000; // 최대 5초 대기
            const int checkInterval = 100; // 100ms마다 확인
            int elapsedTime = 0;

            while (elapsedTime < maxWaitTime)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                // IsSignedIn이 false가 되면 로그아웃 완료로 간주
                bool isSignedIn = AuthenticationService.Instance.IsSignedIn;
                var playerId = AuthenticationService.Instance.PlayerId;

                Debug.Log($"UGS Sign out check - IsSignedIn: {isSignedIn}, PlayerId: '{playerId}', Elapsed: {elapsedTime}ms");

                if (!isSignedIn)
                {
                    Debug.Log("UGS Sign out completed - IsSignedIn is false");
                    return;
                }

                await UniTask.Delay(checkInterval, cancellationToken: cancellationToken);
                elapsedTime += checkInterval;

                // 1초마다 진행 상황 로그
                if (elapsedTime % 1000 == 0)
                {
                    Debug.Log($"UGS Sign out in progress... ({elapsedTime / 1000}s elapsed)");
                }
            }

            // 타임아웃 후 최종 확인
            bool finalIsSignedIn = AuthenticationService.Instance.IsSignedIn;
            var finalPlayerId = AuthenticationService.Instance.PlayerId;

            if (finalIsSignedIn)
            {
                Debug.LogWarning($"UGS Sign out timeout - IsSignedIn: {finalIsSignedIn}, PlayerId: '{finalPlayerId}'");
            }
            else
            {
                Debug.Log("UGS Sign out completed after timeout check");
            }
        }

        private async Task EnsureInitializedAsync(CancellationToken cancellationToken)
        {
            if (_isInitialized) return;

            try
            {
                await UnityServices.InitializeAsync().AsUniTask();

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

        public Task UnregisterWithUsernamePasswordAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IAuthenticationSession> GetAuthenticationSessionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
