using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Noomyung.CloudSave.Domain;

namespace Noomyung.CloudSave.Application
{
    public sealed class CloudSaveUseCases : IDisposable
    {
        private readonly ICloudSaveService _cloudSave;

        public CloudSaveUseCases(ICloudSaveService cloudSave)
        {
            _cloudSave = cloudSave ?? throw new ArgumentNullException(nameof(cloudSave));
        }

        public Task SaveBytesAsync(string key, byte[] data, CancellationToken cancellationToken = default)
        {
            return _cloudSave.SaveBytesAsync(key, data, cancellationToken);
        }

        public Task<byte[]> LoadBytesAsync(string key, CancellationToken cancellationToken = default)
        {
            return _cloudSave.LoadBytesAsync(key, cancellationToken);
        }

        public void Dispose()
        {
            _cloudSave.Dispose();
        }
    }

    public sealed class CloudSaveJsonUseCases : IDisposable
    {
        private readonly ICloudSaveService _cloudSave;
        private readonly IJsonSerializer _serializer;

        public CloudSaveJsonUseCases(ICloudSaveService cloudSave, IJsonSerializer serializer)
        {
            _cloudSave = cloudSave ?? throw new ArgumentNullException(nameof(cloudSave));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public Task SaveAsync<T>(string key, T value, CancellationToken cancellationToken = default)
        {
            var json = _serializer.Serialize(value);
            var bytes = Encoding.UTF8.GetBytes(json);

            return _cloudSave.SaveBytesAsync(key, bytes, cancellationToken);
        }

        public async Task<T> LoadAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var bytes = await _cloudSave.LoadBytesAsync(key, cancellationToken).ConfigureAwait(false);
            var json = bytes == null ? null : Encoding.UTF8.GetString(bytes);

            if (string.IsNullOrEmpty(json)) return default;

            var value = _serializer.Deserialize<T>(json);

            return value;
        }

        public void Dispose()
        {
            _cloudSave.Dispose();
        }
    }
}
