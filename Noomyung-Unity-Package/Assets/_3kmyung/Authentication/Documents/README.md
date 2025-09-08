# 3kmyung Authentication Package

A Unity authentication system package designed with Clean Architecture and SOLID principles, supporting multiple authentication services including PlayFab and Unity Gaming Services.

## 📋 Table of Contents

- [Features](#features)
- [Architecture](#architecture)
- [Installation & Setup](#installation--setup)
- [Usage](#usage)
- [API Reference](#api-reference)
- [Examples](#examples)
- [Troubleshooting](#troubleshooting)

## 🚀 Features

- **Multi-Authentication Service Support**: PlayFab, Unity Gaming Services (UGS)
- **Clean Architecture**: Separated Domain, Application, and Infrastructure layers
- **Asynchronous Processing**: Full async/await pattern support
- **Cancellation Token Support**: Task cancellation via CancellationToken
- **Conditional Compilation**: Optimized build size by including only required services
- **Multiple Authentication Providers**: Anonymous, Google, Apple, Facebook, Steam, Custom, Username/Password

## 🏗️ Architecture

```
Authentication/
├── Domain/           # Business logic and interfaces
│   └── IAuthenticationService.cs
├── Application/      # Use cases and application logic
│   └── AuthenticationUseCases.cs
└── Infrastructure/   # External service implementations
    ├── Core/         # Base infrastructure
    ├── PlayFab/      # PlayFab implementation
    └── UGS/          # Unity Gaming Services implementation
```

### Layer Responsibilities

- **Domain**: Defines authentication business rules and interfaces
- **Application**: Implements authentication use cases and application logic
- **Infrastructure**: Actual authentication service implementations (PlayFab, UGS)

## ⚙️ Installation & Setup

### 1. Package Installation

This package works with the following Unity packages:

#### For PlayFab
```json
{
  "dependencies": {
    "com.playfab.playfab": "2.100.240101"
  }
}
```

#### For Unity Gaming Services
```json
{
  "dependencies": {
    "com.unity.services.authentication": "3.0.0",
    "com.unity.services.core": "1.10.2"
  }
}
```

### 2. Scripting Define Symbols

Add the following symbols based on the authentication service you want to use:

- **PlayFab**: `PLAYFAB_PACKAGE`
- **UGS**: `UGS_PACKAGE`

You can set these in **Project Settings > Player > Scripting Define Symbols**.

## 📖 Usage

### Basic Usage

```csharp
using 3kmyung.Authentication.Application;
using 3kmyung.Authentication.Infrastructure;
using 3kmyung.Authentication.Domain;

public class AuthenticationManager : MonoBehaviour
{
    private AuthenticationUseCases _authenticationUseCases;
    
    private void Start()
    {
        // For PlayFab
        var playFabService = new PlayFabAuthenticationService();
        _authenticationUseCases = new AuthenticationUseCases(playFabService);
        
        // Or for UGS
        // var ugsService = new UgsAuthenticationService();
        // _authenticationUseCases = new AuthenticationUseCases(ugsService);
    }
    
    public async void SignInAnonymously()
    {
        try
        {
            var playerId = await _authenticationUseCases.SignInAnonymouslyAsync();
            Debug.Log($"Sign in successful: {playerId}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Sign in failed: {ex.Message}");
        }
    }
}
```

### Social Login

```csharp
public async void SignInWithGoogle(string googleToken)
{
    try
    {
        var playerId = await _authenticationUseCases.SignInWithProviderAsync(
            AuthenticationProvider.Google, 
            googleToken
        );
        Debug.Log($"Google sign in successful: {playerId}");
    }
    catch (Exception ex)
    {
        Debug.LogError($"Google sign in failed: {ex.Message}");
    }
}
```

### Username & Password Authentication

```csharp
// User registration
public async void RegisterUser(string username, string password)
{
    try
    {
        await _authenticationUseCases.RegisterWithUsernamePasswordAsync(username, password);
        Debug.Log("User registration successful");
    }
    catch (Exception ex)
    {
        Debug.LogError($"User registration failed: {ex.Message}");
    }
}

// Username/Password sign in
public async void SignInWithUsernamePassword(string username, string password)
{
    try
    {
        var playerId = await _authenticationUseCases.SignInWithUsernamePasswordAsync(username, password);
        Debug.Log($"Username/Password sign in successful: {playerId}");
    }
    catch (Exception ex)
    {
        Debug.LogError($"Username/Password sign in failed: {ex.Message}");
    }
}
```

### Account Linking/Unlinking

```csharp
// Link account
public async void LinkGoogleAccount(string googleToken)
{
    try
    {
        await _authenticationUseCases.LinkProviderAsync(AuthenticationProvider.Google, googleToken);
        Debug.Log("Google account linked successfully");
    }
    catch (Exception ex)
    {
        Debug.LogError($"Account linking failed: {ex.Message}");
    }
}

// Unlink account
public async void UnlinkGoogleAccount()
{
    try
    {
        await _authenticationUseCases.UnlinkProviderAsync(AuthenticationProvider.Google);
        Debug.Log("Google account unlinked successfully");
    }
    catch (Exception ex)
    {
        Debug.LogError($"Account unlinking failed: {ex.Message}");
    }
}
```

### Using Cancellation Tokens

```csharp
public async void SignInWithCancellation()
{
    var cancellationTokenSource = new CancellationTokenSource();
    
    // Cancel after 5 seconds
    cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(5));
    
    try
    {
        var playerId = await _authenticationUseCases.SignInAnonymouslyAsync(
            cancellationTokenSource.Token
        );
        Debug.Log($"Sign in successful: {playerId}");
    }
    catch (OperationCanceledException)
    {
        Debug.Log("Sign in was cancelled.");
    }
    catch (Exception ex)
    {
        Debug.LogError($"Sign in failed: {ex.Message}");
    }
}
```

## 📚 API Reference

### AuthenticationUseCases

Main class providing all authentication use cases.

#### Methods

| Method | Description | Return Type |
|--------|-------------|-------------|
| `SignInAnonymouslyAsync` | Anonymous sign in | `Task<string>` |
| `SignInWithProviderAsync` | Provider-specific sign in | `Task<string>` |
| `SignInWithUsernamePasswordAsync` | Username/Password sign in | `Task<string>` |
| `RegisterWithUsernamePasswordAsync` | Username/Password registration | `Task` |
| `LinkProviderAsync` | Link account | `Task` |
| `UnlinkProviderAsync` | Unlink account | `Task` |
| `GetPlayerIdAsync` | Get player ID | `Task<string>` |
| `SignOutAsync` | Sign out | `Task` |

### AuthenticationProvider Enumeration

Supported authentication providers:

- `Anonymous`: Anonymous sign in
- `Google`: Google account
- `Apple`: Apple ID
- `Facebook`: Facebook account
- `Steam`: Steam account
- `Custom`: Custom token
- `UsernamePassword`: Username/Password authentication

## 💡 Examples

### Complete Authentication Manager Example

```csharp
using System;
using System.Threading;
using UnityEngine;
using 3kmyung.Authentication.Application;
using 3kmyung.Authentication.Infrastructure;
using 3kmyung.Authentication.Domain;

public class CompleteAuthenticationManager : MonoBehaviour
{
    [Header("Authentication Settings")]
    [SerializeField] private bool usePlayFab = true;
    
    private AuthenticationUseCases _authenticationUseCases;
    private CancellationTokenSource _cancellationTokenSource;
    
    public event Action<string> OnSignInSuccess;
    public event Action<string> OnSignInFailed;
    public event Action OnSignOutSuccess;
    
    private void Awake()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        InitializeAuthenticationService();
    }
    
    private void InitializeAuthenticationService()
    {
        IAuthenticationService authenticationService = usePlayFab 
            ? new PlayFabAuthenticationService() 
            : new UgsAuthenticationService();
            
        _authenticationUseCases = new AuthenticationUseCases(authenticationService);
    }
    
    public async void SignInAnonymously()
    {
        try
        {
            var playerId = await _authenticationUseCases.SignInAnonymouslyAsync(
                _cancellationTokenSource.Token
            );
            OnSignInSuccess?.Invoke(playerId);
        }
        catch (OperationCanceledException)
        {
            OnSignInFailed?.Invoke("Sign in was cancelled.");
        }
        catch (Exception ex)
        {
            OnSignInFailed?.Invoke(ex.Message);
        }
    }
    
    public async void SignInWithGoogle(string googleToken)
    {
        try
        {
            var playerId = await _authenticationUseCases.SignInWithProviderAsync(
                AuthenticationProvider.Google, 
                googleToken, 
                _cancellationTokenSource.Token
            );
            OnSignInSuccess?.Invoke(playerId);
        }
        catch (OperationCanceledException)
        {
            OnSignInFailed?.Invoke("Sign in was cancelled.");
        }
        catch (Exception ex)
        {
            OnSignInFailed?.Invoke(ex.Message);
        }
    }
    
    public async void RegisterUser(string username, string password)
    {
        try
        {
            await _authenticationUseCases.RegisterWithUsernamePasswordAsync(
                username, 
                password, 
                _cancellationTokenSource.Token
            );
            Debug.Log("User registration successful");
        }
        catch (OperationCanceledException)
        {
            Debug.Log("User registration was cancelled.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"User registration failed: {ex.Message}");
        }
    }
    
    public async void SignInWithUsernamePassword(string username, string password)
    {
        try
        {
            var playerId = await _authenticationUseCases.SignInWithUsernamePasswordAsync(
                username, 
                password, 
                _cancellationTokenSource.Token
            );
            OnSignInSuccess?.Invoke(playerId);
        }
        catch (OperationCanceledException)
        {
            OnSignInFailed?.Invoke("Sign in was cancelled.");
        }
        catch (Exception ex)
        {
            OnSignInFailed?.Invoke(ex.Message);
        }
    }
    
    public async void SignOut()
    {
        try
        {
            await _authenticationUseCases.SignOutAsync(_cancellationTokenSource.Token);
            OnSignOutSuccess?.Invoke();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Sign out failed: {ex.Message}");
        }
    }
    
    private void OnDestroy()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _authenticationUseCases?.Dispose();
    }
}
```

## 🔧 Troubleshooting

### Common Issues

#### 1. "PLAYFAB_PACKAGE define is missing" Error
**Cause**: PlayFab SDK not installed or scripting define symbol not set
**Solution**: 
- Verify PlayFab SDK installation
- Add `PLAYFAB_PACKAGE` symbol

#### 2. "UGS_PACKAGE define is missing" Error
**Cause**: Unity Gaming Services packages not installed or scripting define symbol not set
**Solution**:
- Verify UGS package installation
- Add `UGS_PACKAGE` symbol

#### 3. Sign In Failure
**Cause**: Network connectivity issues, invalid tokens, service configuration errors
**Solution**:
- Check network connection
- Validate token authenticity
- Verify service configuration

### Debugging Tips

1. **Check Logs**: Review detailed error messages in Unity Console
2. **Network Status**: Verify internet connectivity
3. **Token Validation**: Confirm social login token validity
4. **Service Configuration**: Check PlayFab/UGS project settings

## 📄 License

This package is for 3kmyung internal use only.

## 🤝 Support

For issues or questions, please contact the development team.

---

**Version**: 1.0.0  
**Unity Version**: 2022.3 LTS or higher  
**Last Updated**: January 2024
