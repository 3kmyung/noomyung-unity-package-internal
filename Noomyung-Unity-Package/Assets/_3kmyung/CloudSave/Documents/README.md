# 3kmyung CloudSave Package

A Unity package that provides a clean, architecture-based cloud save system supporting multiple cloud providers.

## Features

- **Clean Architecture**: Follows Clean Architecture principles with clear separation of concerns
- **Multiple Providers**: Supports Unity Gaming Services (UGS) and PlayFab cloud save services
- **Type Safety**: Strongly typed interfaces and generic methods
- **Async/Await**: Full async/await support with cancellation token support
- **JSON Serialization**: Built-in JSON serialization for complex objects
- **Byte Array Support**: Direct byte array storage for binary data

## Architecture

The package is structured using Clean Architecture principles:

```
CloudSave/
├── Domain/           # Core business logic and interfaces
├── Application/      # Use cases and application services
└── Infrastructure/   # External service implementations
    ├── UGS/         # Unity Gaming Services implementation
    └── PlayFab/     # PlayFab implementation
```

## Installation

### Prerequisites

Choose one of the following cloud save providers:

#### Unity Gaming Services (UGS)
1. Install Unity Gaming Services packages:
   - `Unity.Services.Core`
   - `Unity.Services.CloudSave`
2. Add `UGS_PACKAGE` to your Scripting Define Symbols

#### PlayFab
1. Install PlayFab SDK
2. Add `PLAYFAB_PACKAGE` to your Scripting Define Symbols

## Usage

### Basic Setup

```csharp
using _3kmyung.CloudSave.Application;
using _3kmyung.CloudSave.Infrastructure;

// For UGS
var cloudSaveService = new UgsCloudSaveService();
var useCases = new CloudSaveUseCases(cloudSaveService);

// For PlayFab
var cloudSaveService = new PlayFabCloudSaveService();
var useCases = new CloudSaveUseCases(cloudSaveService);
```

### Saving Data

#### Byte Arrays
```csharp
byte[] data = Encoding.UTF8.GetBytes("Hello, Cloud Save!");
await useCases.SaveBytesAsync("my_data", data);
```

#### JSON Objects
```csharp
using 3kmyung.CloudSave.Infrastructure;

var serializer = new NewtonsoftJsonSerializer();
var jsonUseCases = new CloudSaveJsonUseCases(cloudSaveService, serializer);

var playerData = new PlayerData
{
    Name = "Player1",
    Level = 10,
    Score = 1500
};

await jsonUseCases.SaveAsync("player_data", playerData);
```

### Loading Data

#### Byte Arrays
```csharp
byte[] loadedData = await useCases.LoadBytesAsync("my_data");
string text = Encoding.UTF8.GetString(loadedData);
```

#### JSON Objects
```csharp
var playerData = await jsonUseCases.LoadAsync<PlayerData>("player_data");
```

### Additional Operations

```csharp
// Check if key exists
bool exists = await cloudSaveService.HasKeyAsync("my_data");

// List all keys
var allKeys = await cloudSaveService.ListKeysAsync();

// List keys with prefix
var playerKeys = await cloudSaveService.ListKeysAsync("player_");

// Delete data
await cloudSaveService.DeleteAsync("my_data");
```

## API Reference

### ICloudSaveService

The core interface for cloud save operations:

```csharp
public interface ICloudSaveService : IDisposable
{
    Task SaveBytesAsync(string key, byte[] data, CancellationToken cancellationToken = default);
    Task<byte[]> LoadBytesAsync(string key, CancellationToken cancellationToken = default);
    Task<bool> HasKeyAsync(string key, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> ListKeysAsync(string prefix = null, CancellationToken cancellationToken = default);
    Task DeleteAsync(string key, CancellationToken cancellationToken = default);
}
```

### CloudSaveUseCases

Application layer for basic byte array operations:

```csharp
public sealed class CloudSaveUseCases : IDisposable
{
    public Task SaveBytesAsync(string key, byte[] data, CancellationToken cancellationToken = default);
    public Task<byte[]> LoadBytesAsync(string key, CancellationToken cancellationToken = default);
}
```

### CloudSaveJsonUseCases

Application layer for JSON serialization:

```csharp
public sealed class CloudSaveJsonUseCases : IDisposable
{
    public Task SaveAsync<T>(string key, T value, CancellationToken cancellationToken = default);
    public Task<T> LoadAsync<T>(string key, CancellationToken cancellationToken = default);
}
```

## Error Handling

The package includes comprehensive error handling:

- Network errors are logged and exceptions are thrown
- Missing data returns `null` for byte arrays and `default(T)` for JSON objects
- Cancellation tokens are respected throughout the API

## Best Practices

1. **Dispose Resources**: Always dispose of use cases and services when done
2. **Error Handling**: Wrap cloud save operations in try-catch blocks
3. **Cancellation**: Use cancellation tokens for long-running operations
4. **Key Naming**: Use consistent, descriptive key naming conventions
5. **Data Validation**: Validate loaded data before using it

## Example: Complete Player Data Management

```csharp
public class PlayerDataManager : IDisposable
{
    private readonly CloudSaveJsonUseCases _cloudSave;
    
    public PlayerDataManager(ICloudSaveService cloudSaveService, IJsonSerializer serializer)
    {
        _cloudSave = new CloudSaveJsonUseCases(cloudSaveService, serializer);
    }
    
    public async Task SavePlayerDataAsync(PlayerData data)
    {
        try
        {
            await _cloudSave.SaveAsync("player_data", data);
            Debug.Log("Player data saved successfully");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save player data: {ex.Message}");
        }
    }
    
    public async Task<PlayerData> LoadPlayerDataAsync()
    {
        try
        {
            var data = await _cloudSave.LoadAsync<PlayerData>("player_data");
            return data ?? new PlayerData(); // Return default if no data exists
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load player data: {ex.Message}");
            return new PlayerData();
        }
    }
    
    public void Dispose()
    {
        _cloudSave?.Dispose();
    }
}
```

## License

This package is part of the 3kmyung Unity Package collection.
