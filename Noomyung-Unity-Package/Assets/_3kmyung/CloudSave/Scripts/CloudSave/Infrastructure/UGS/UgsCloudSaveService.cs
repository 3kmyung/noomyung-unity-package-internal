using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using _3kmyung.CloudSave.Domain;

#if UGS_PACKAGE
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.Core;
#endif

namespace _3kmyung.CloudSave.Infrastructure
{
    public sealed class UgsCloudSaveService : Domain.ICloudSaveService
    {
#if UGS_PACKAGE
        private bool _isInitialized;
#endif
        public UgsCloudSaveService()
        {
        }

        public async Task SaveBytesAsync(string key, byte[] data, CancellationToken cancellationToken = default)
        {
#if UGS_PACKAGE
            await EnsureInitializedAsync(cancellationToken);

            var dict = new Dictionary<string, object>
            {
                { key, Convert.ToBase64String(data ?? Array.Empty<byte>()) }
            };

            await CloudSaveService.Instance.Data.ForceSaveAsync(dict);
#else
            Debug.LogError("UGS_PACKAGE define is missing or UGS packages are not installed.");
            await Task.Yield();
#endif
        }

        public async Task<byte[]> LoadBytesAsync(string key, CancellationToken cancellationToken = default)
        {
#if UGS_PACKAGE
            await EnsureInitializedAsync(cancellationToken);

            try
            {
                var results = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { key });

                if (results.TryGetValue(key, out var item))
                {
                    var base64 = item;

                    return Convert.FromBase64String(base64);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"UgsCloudSaveService.LoadBytesAsync failed. {ex.Message}");
            }

            return null;
#else
            Debug.LogError("UGS_PACKAGE define is missing or UGS packages are not installed.");
            return null;
#endif
        }

        public async Task<bool> HasKeyAsync(string key, CancellationToken cancellationToken = default)
        {
#if UGS_PACKAGE
            await EnsureInitializedAsync(cancellationToken);

            var keys = await ListKeysAsync(null, cancellationToken).ConfigureAwait(false);

            return keys.Contains(key);
#else
            Debug.LogError("UGS_PACKAGE define is missing or UGS packages are not installed.");
            return false;
#endif
        }

        public async Task<IReadOnlyList<string>> ListKeysAsync(string prefix = null, CancellationToken cancellationToken = default)
        {
#if UGS_PACKAGE
            await EnsureInitializedAsync(cancellationToken);

            try
            {
                // UGS CloudSave에서는 모든 키를 직접 가져오는 API가 없으므로
                // LoadAsync를 사용하여 모든 데이터를 로드한 후 키를 추출합니다.
                var results = await CloudSaveService.Instance.Data.LoadAsync();

                var keys = new List<string>();
                foreach (var kvp in results)
                {
                    if (string.IsNullOrEmpty(prefix) || kvp.Key.StartsWith(prefix, StringComparison.Ordinal))
                    {
                        keys.Add(kvp.Key);
                    }
                }

                return keys;
            }
            catch (Exception ex)
            {
                Debug.LogError($"UgsCloudSaveService.ListKeysAsync failed. {ex.Message}");
                return Array.Empty<string>();
            }
#else
            Debug.LogError("UGS_PACKAGE define is missing or UGS packages are not installed.");
            return Array.Empty<string>();
#endif
        }

        public async Task DeleteAsync(string key, CancellationToken cancellationToken = default)
        {
#if UGS_PACKAGE
            await EnsureInitializedAsync(cancellationToken);

            await CloudSaveService.Instance.Data.ForceDeleteAsync(key);
#else
            Debug.LogError("UGS_PACKAGE define is missing or UGS packages are not installed.");
            await Task.Yield();
#endif
        }

#if UGS_PACKAGE
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
#endif

        public void Dispose()
        {
        }
    }
}
