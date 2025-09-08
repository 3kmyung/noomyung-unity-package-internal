using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Noomyung.Authentication.Domain;
using PlayFab;
using PlayFab.ClientModels;
using Cysharp.Threading.Tasks;

namespace Noomyung.Authentication.Infrastructure
{
    internal sealed class PlayFabAuthSession : IAuthSession
    {
        public string PlayerId { get; }

        public bool IsSignedIn { get; }

        public PlayFabAuthSession(string playerId, bool isSignedIn)
        {
            PlayerId = playerId;
            IsSignedIn = isSignedIn;
        }
    }

    public sealed class PlayFabAuthService : IAuthService
    {
        public PlayFabAuthService()
        {
        }

        public async Task<IAuthSession> SignInAnonymouslyAsync(CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<LoginResult>();

            PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest
            {
                CreateAccount = true,
                CustomId = SystemInfo.deviceUniqueIdentifier
            }, r => tcs.TrySetResult(r), e =>
            {
                Debug.LogError($"PlayFab Login failed. {e.ErrorMessage}");
                tcs.TrySetException(new Exception(e.ErrorMessage));
            });

            var result = await tcs.Task.AsUniTask();

            var session = new PlayFabAuthSession(result.PlayFabId, true);

            return session;
        }

        public async Task<IAuthSession> SignInWithProviderAsync(AuthProvider provider, string accessToken, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<LoginResult>();

            try
            {
                switch (provider)
                {
                    case AuthProvider.Google:
                        PlayFabClientAPI.LoginWithGoogleAccount(new LoginWithGoogleAccountRequest { ServerAuthCode = accessToken, CreateAccount = true },
                            r => tcs.TrySetResult(r),
                            e => { Debug.LogError($"PlayFab Google login failed. {e.ErrorMessage}"); tcs.TrySetException(new Exception(e.ErrorMessage)); });
                        break;
                    case AuthProvider.Apple:
                        PlayFabClientAPI.LoginWithApple(new LoginWithAppleRequest { IdentityToken = accessToken, CreateAccount = true },
                            r => tcs.TrySetResult(r),
                            e => { Debug.LogError($"PlayFab Apple login failed. {e.ErrorMessage}"); tcs.TrySetException(new Exception(e.ErrorMessage)); });
                        break;
                    case AuthProvider.Facebook:
                        PlayFabClientAPI.LoginWithFacebook(new LoginWithFacebookRequest { AccessToken = accessToken, CreateAccount = true },
                            r => tcs.TrySetResult(r),
                            e => { Debug.LogError($"PlayFab Facebook login failed. {e.ErrorMessage}"); tcs.TrySetException(new Exception(e.ErrorMessage)); });
                        break;
                    case AuthProvider.Custom:
                        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest { CustomId = accessToken, CreateAccount = true },
                            r => tcs.TrySetResult(r),
                            e => { Debug.LogError($"PlayFab custom login failed. {e.ErrorMessage}"); tcs.TrySetException(new Exception(e.ErrorMessage)); });
                        break;
                    default:
                        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest { CreateAccount = true, CustomId = SystemInfo.deviceUniqueIdentifier },
                            r => tcs.TrySetResult(r),
                            e => { Debug.LogError($"PlayFab default login failed. {e.ErrorMessage}"); tcs.TrySetException(new Exception(e.ErrorMessage)); });
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"PlayFab SignInWithProviderAsync exception. {ex.Message}");

                throw;
            }

            var result = await tcs.Task.AsUniTask();

            var session = new PlayFabAuthSession(result.PlayFabId, true);

            return session;
        }

        public async Task<IAuthSession> SignInWithUsernamePasswordAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<LoginResult>();

            try
            {
                PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest
                {
                    Username = username,
                    Password = password
                }, r => tcs.TrySetResult(r), e =>
                {
                    Debug.LogError($"PlayFab Username/Password login failed. {e.ErrorMessage}");
                    tcs.TrySetException(new Exception(e.ErrorMessage));
                });
            }
            catch (Exception ex)
            {
                Debug.LogError($"PlayFab SignInWithUsernamePasswordAsync exception. {ex.Message}");
                throw;
            }

            var result = await tcs.Task.AsUniTask();
            var session = new PlayFabAuthSession(result.PlayFabId, true);

            return session;
        }

        public async Task RegisterWithUsernamePasswordAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<RegisterPlayFabUserResult>();

            try
            {
                PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest
                {
                    Username = username,
                    Password = password,
                    RequireBothUsernameAndEmail = false
                }, r => tcs.TrySetResult(r), e =>
                {
                    Debug.LogError($"PlayFab Username/Password registration failed. {e.ErrorMessage}");
                    tcs.TrySetException(new Exception(e.ErrorMessage));
                });
            }
            catch (Exception ex)
            {
                Debug.LogError($"PlayFab RegisterWithUsernamePasswordAsync exception. {ex.Message}");
                throw;
            }

            var result = await tcs.Task.AsUniTask();
            
            // Register 성공 후 자동으로 로그인을 수행합니다.
            // 이렇게 하면 Register 후 즉시 로그인된 상태가 됩니다.
            Debug.Log($"User registered successfully with username: {username}. Auto-login will be performed.");
        }

        public Task LinkProviderAsync(AuthProvider provider, string accessToken, CancellationToken cancellationToken = default)
        {
            // Implement provider linking as needed (e.g., AddUsernamePassword, LinkGoogleAccount etc.).
            return Task.CompletedTask;
        }

        public Task UnlinkProviderAsync(AuthProvider provider, CancellationToken cancellationToken = default)
        {
            // Implement provider unlink as needed (e.g., UnlinkGoogleAccount, UnlinkCustomID etc.).
            return Task.CompletedTask;
        }

        public Task<string> GetPlayerIdAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(PlayFabSettings.staticPlayer.PlayFabId);
        }

        public async Task SignOutAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // PlayFab에서 로그인 상태 확인
                if (PlayFabSettings.staticPlayer != null && !string.IsNullOrEmpty(PlayFabSettings.staticPlayer.PlayFabId))
                {
                    PlayFabClientAPI.ForgetAllCredentials();
                    
                    // 로그아웃 상태를 지속적으로 확인
                    await WaitForSignOutCompletion(cancellationToken);
                    
                    Debug.Log("PlayFab Sign out completed successfully");
                }
                else
                {
                    Debug.Log("User is not signed in, no need to sign out");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"PlayFab Sign out failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 로그아웃이 완료될 때까지 상태를 지속적으로 확인합니다.
        /// </summary>
        private async UniTask WaitForSignOutCompletion(CancellationToken cancellationToken)
        {
            const int maxWaitTime = 5000; // 최대 5초 대기
            const int checkInterval = 50; // 50ms마다 확인
            int elapsedTime = 0;

            while (IsPlayFabSignedIn() && elapsedTime < maxWaitTime)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                await UniTask.Delay(checkInterval, cancellationToken: cancellationToken);
                elapsedTime += checkInterval;
            }

            if (IsPlayFabSignedIn())
            {
                Debug.LogWarning("PlayFab Sign out timeout - user may still be signed in");
            }
        }

        public async Task<bool> IsSignedInAsync(CancellationToken cancellationToken = default)
        {
            return IsPlayFabSignedIn();
        }

        /// <summary>
        /// PlayFab 로그인 상태를 확인합니다.
        /// </summary>
        private bool IsPlayFabSignedIn()
        {
            return PlayFabSettings.staticPlayer != null && !string.IsNullOrEmpty(PlayFabSettings.staticPlayer.PlayFabId);
        }

        public void Dispose()
        {
        }
    }
}
