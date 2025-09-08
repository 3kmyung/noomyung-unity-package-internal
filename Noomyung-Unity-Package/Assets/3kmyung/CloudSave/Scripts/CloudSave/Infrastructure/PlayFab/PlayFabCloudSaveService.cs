using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using _3kmyung.CloudSave.Domain;

#if PLAYFAB_PACKAGE
using PlayFab;
using PlayFab.ClientModels;
#endif

namespace _3kmyung.CloudSave.Infrastructure
{
    public sealed class PlayFabCloudSaveService : ICloudSaveService
    {
        public PlayFabCloudSaveService()
        {
        }

        public async Task SaveBytesAsync(string key, byte[] data, CancellationToken cancellationToken = default)
        {
#if PLAYFAB_PACKAGE
            var tcs = new TaskCompletionSource<UpdateUserDataResult>();

            var value = Convert.ToBase64String(data ?? Array.Empty<byte>());

            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string> { { key, value } }
            }, r => tcs.TrySetResult(r), e =>
            {
                Debug.LogError($"PlayFab UpdateUserData failed. {e.ErrorMessage}");
                tcs.TrySetException(new Exception(e.ErrorMessage));
            });

            await tcs.Task.ConfigureAwait(false);
#else
            Debug.LogError("PLAYFAB_PACKAGE define is missing or PlayFab SDK is not installed.");
            await Task.Yield();
#endif
        }

        public async Task<byte[]> LoadBytesAsync(string key, CancellationToken cancellationToken = default)
        {
#if PLAYFAB_PACKAGE
            var tcs = new TaskCompletionSource<GetUserDataResult>();

            PlayFabClientAPI.GetUserData(new GetUserDataRequest
            {
                Keys = new List<string> { key }
            }, r => tcs.TrySetResult(r), e =>
            {
                Debug.LogError($"PlayFab GetUserData failed. {e.ErrorMessage}");
                tcs.TrySetException(new Exception(e.ErrorMessage));
            });

            var result = await tcs.Task.ConfigureAwait(false);

            if (result.Data != null && result.Data.TryGetValue(key, out var record))
            {
                return Convert.FromBase64String(record.Value);
            }

            return null;
#else
            Debug.LogError("PLAYFAB_PACKAGE define is missing or PlayFab SDK is not installed.");
            return null;
#endif
        }

        public async Task<bool> HasKeyAsync(string key, CancellationToken cancellationToken = default)
        {
#if PLAYFAB_PACKAGE
            var keys = await ListKeysAsync(null, cancellationToken).ConfigureAwait(false);

            return keys.Contains(key);
#else
            Debug.LogError("PLAYFAB_PACKAGE define is missing or PlayFab SDK is not installed.");
            return false;
#endif
        }

        public async Task<IReadOnlyList<string>> ListKeysAsync(string prefix = null, CancellationToken cancellationToken = default)
        {
#if PLAYFAB_PACKAGE
            var tcs = new TaskCompletionSource<GetUserDataResult>();

            PlayFabClientAPI.GetUserData(new GetUserDataRequest(), r => tcs.TrySetResult(r), e =>
            {
                Debug.LogError($"PlayFab GetUserData failed. {e.ErrorMessage}");
                tcs.TrySetException(new Exception(e.ErrorMessage));
            });

            var result = await tcs.Task.ConfigureAwait(false);

            var keys = result.Data?.Keys?.ToList() ?? new List<string>();

            if (!string.IsNullOrEmpty(prefix))
            {
                keys = keys.Where(k => k.StartsWith(prefix, StringComparison.Ordinal)).ToList();
            }

            return keys;
#else
            Debug.LogError("PLAYFAB_PACKAGE define is missing or PlayFab SDK is not installed.");
            return Array.Empty<string>();
#endif
        }

        public async Task DeleteAsync(string key, CancellationToken cancellationToken = default)
        {
#if PLAYFAB_PACKAGE
            var tcs = new TaskCompletionSource<UpdateUserDataResult>();

            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
            {
                KeysToRemove = new List<string> { key }
            }, r => tcs.TrySetResult(r), e =>
            {
                Debug.LogError($"PlayFab DeleteUserData failed. {e.ErrorMessage}");
                tcs.TrySetException(new Exception(e.ErrorMessage));
            });

            await tcs.Task.ConfigureAwait(false);
#else
            Debug.LogError("PLAYFAB_PACKAGE define is missing or PlayFab SDK is not installed.");
            await Task.Yield();
#endif
        }

        public void Dispose()
        {
        }
    }

    public sealed class NewtonsoftJsonSerializer : IJsonSerializer
    {
        public string Serialize<T>(T value)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value);
        }

        public T Deserialize<T>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }
    }
}
