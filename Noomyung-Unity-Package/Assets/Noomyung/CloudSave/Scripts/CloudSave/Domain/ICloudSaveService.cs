using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Noomyung.CloudSave.Domain
{
    public interface ICloudSaveService : IDisposable
    {
        Task SaveBytesAsync(string key, byte[] data, CancellationToken cancellationToken = default);

        Task<byte[]> LoadBytesAsync(string key, CancellationToken cancellationToken = default);

        Task<bool> HasKeyAsync(string key, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<string>> ListKeysAsync(string prefix = null, CancellationToken cancellationToken = default);

        Task DeleteAsync(string key, CancellationToken cancellationToken = default);
    }

    public interface IJsonSerializer
    {
        string Serialize<T>(T value);

        T Deserialize<T>(string json);
    }
}
