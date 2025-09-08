# 3kmyung Authentication Package

Unity용 인증 시스템 패키지입니다. Clean Architecture와 SOLID 원칙을 기반으로 설계되어 다양한 인증 서비스(PlayFab, Unity Gaming Services)를 지원합니다.

## 📋 목차

- [주요 기능](#주요-기능)
- [아키텍처](#아키텍처)
- [설치 및 설정](#설치-및-설정)
- [사용법](#사용법)
- [API 참조](#api-참조)
- [예제](#예제)
- [문제 해결](#문제-해결)

## 🚀 주요 기능

- **다중 인증 서비스 지원**: PlayFab, Unity Gaming Services (UGS)
- **Clean Architecture**: Domain, Application, Infrastructure 레이어 분리
- **비동기 처리**: async/await 패턴 지원
- **취소 토큰 지원**: CancellationToken을 통한 작업 취소
- **조건부 컴파일**: 필요한 서비스만 포함하여 빌드 크기 최적화
- **다양한 인증 제공자**: 익명, Google, Apple, Facebook, Steam, Custom, Username/Password

## 🏗️ 아키텍처

```
Authentication/
├── Domain/           # 비즈니스 로직 및 인터페이스
│   └── IAuthService.cs
├── Application/      # 유스케이스 및 애플리케이션 로직
│   └── AuthUseCases.cs
└── Infrastructure/   # 외부 서비스 구현체
    ├── Core/         # 기본 인프라스트럭처
    ├── PlayFab/      # PlayFab 구현체
    └── UGS/          # Unity Gaming Services 구현체
```

### 레이어별 역할

- **Domain**: 인증 관련 비즈니스 규칙과 인터페이스 정의
- **Application**: 인증 유스케이스 구현 및 애플리케이션 로직
- **Infrastructure**: 실제 인증 서비스 구현체 (PlayFab, UGS)

## ⚙️ 설치 및 설정

### 1. 패키지 설치

이 패키지는 다음 Unity 패키지들과 함께 사용됩니다:

#### PlayFab 사용 시
```json
{
  "dependencies": {
    "com.playfab.playfab": "2.100.240101"
  }
}
```

#### Unity Gaming Services 사용 시
```json
{
  "dependencies": {
    "com.unity.services.authentication": "3.0.0",
    "com.unity.services.core": "1.10.2"
  }
}
```

### 2. 스크립팅 정의 심볼 설정

사용할 인증 서비스에 따라 다음 심볼을 추가하세요:

- **PlayFab**: `PLAYFAB_PACKAGE`
- **UGS**: `UGS_PACKAGE`

**Project Settings > Player > Scripting Define Symbols**에서 설정할 수 있습니다.

## 📖 사용법

### 기본 사용법

```csharp
using 3kmyung.Authentication.Application;
using 3kmyung.Authentication.Infrastructure;
using 3kmyung.Authentication.Domain;

public class AuthManager : MonoBehaviour
{
    private AuthUseCases _authUseCases;
    
    private void Start()
    {
        // PlayFab 사용 시
        var playFabService = new PlayFabAuthService();
        _authUseCases = new AuthUseCases(playFabService);
        
        // 또는 UGS 사용 시
        // var ugsService = new UgsAuthService();
        // _authUseCases = new AuthUseCases(ugsService);
    }
    
    public async void SignInAnonymously()
    {
        try
        {
            var playerId = await _authUseCases.SignInAnonymouslyAsync();
            Debug.Log($"로그인 성공: {playerId}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"로그인 실패: {ex.Message}");
        }
    }
}
```

### 소셜 로그인

```csharp
public async void SignInWithGoogle(string googleToken)
{
    try
    {
        var playerId = await _authUseCases.SignInWithProviderAsync(
            AuthProvider.Google, 
            googleToken
        );
        Debug.Log($"Google 로그인 성공: {playerId}");
    }
    catch (Exception ex)
    {
        Debug.LogError($"Google 로그인 실패: {ex.Message}");
    }
}
```

### Username & Password 인증

```csharp
// 사용자 등록
public async void RegisterUser(string username, string password)
{
    try
    {
        await _authUseCases.RegisterWithUsernamePasswordAsync(username, password);
        Debug.Log("사용자 등록 성공");
    }
    catch (Exception ex)
    {
        Debug.LogError($"사용자 등록 실패: {ex.Message}");
    }
}

// Username/Password 로그인
public async void SignInWithUsernamePassword(string username, string password)
{
    try
    {
        var playerId = await _authUseCases.SignInWithUsernamePasswordAsync(username, password);
        Debug.Log($"Username/Password 로그인 성공: {playerId}");
    }
    catch (Exception ex)
    {
        Debug.LogError($"Username/Password 로그인 실패: {ex.Message}");
    }
}
```

### 계정 연결/해제

```csharp
// 계정 연결
public async void LinkGoogleAccount(string googleToken)
{
    try
    {
        await _authUseCases.LinkProviderAsync(AuthProvider.Google, googleToken);
        Debug.Log("Google 계정 연결 완료");
    }
    catch (Exception ex)
    {
        Debug.LogError($"계정 연결 실패: {ex.Message}");
    }
}

// 계정 해제
public async void UnlinkGoogleAccount()
{
    try
    {
        await _authUseCases.UnlinkProviderAsync(AuthProvider.Google);
        Debug.Log("Google 계정 해제 완료");
    }
    catch (Exception ex)
    {
        Debug.LogError($"계정 해제 실패: {ex.Message}");
    }
}
```

### 취소 토큰 사용

```csharp
public async void SignInWithCancellation()
{
    var cancellationTokenSource = new CancellationTokenSource();
    
    // 5초 후 취소
    cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(5));
    
    try
    {
        var playerId = await _authUseCases.SignInAnonymouslyAsync(
            cancellationTokenSource.Token
        );
        Debug.Log($"로그인 성공: {playerId}");
    }
    catch (OperationCanceledException)
    {
        Debug.Log("로그인이 취소되었습니다.");
    }
    catch (Exception ex)
    {
        Debug.LogError($"로그인 실패: {ex.Message}");
    }
}
```

## 📚 API 참조

### AuthUseCases

인증 관련 모든 유스케이스를 제공하는 메인 클래스입니다.

#### 메서드

| 메서드 | 설명 | 반환 타입 |
|--------|------|-----------|
| `SignInAnonymouslyAsync` | 익명 로그인 | `Task<string>` |
| `SignInWithProviderAsync` | 제공자별 로그인 | `Task<string>` |
| `SignInWithUsernamePasswordAsync` | Username/Password 로그인 | `Task<string>` |
| `RegisterWithUsernamePasswordAsync` | Username/Password 등록 | `Task` |
| `LinkProviderAsync` | 계정 연결 | `Task` |
| `UnlinkProviderAsync` | 계정 해제 | `Task` |
| `GetPlayerIdAsync` | 플레이어 ID 조회 | `Task<string>` |
| `SignOutAsync` | 로그아웃 | `Task` |

### AuthProvider 열거형

지원하는 인증 제공자 목록:

- `Anonymous`: 익명 로그인
- `Google`: Google 계정
- `Apple`: Apple ID
- `Facebook`: Facebook 계정
- `Steam`: Steam 계정
- `Custom`: 커스텀 토큰
- `UsernamePassword`: Username/Password 인증

## 💡 예제

### 완전한 인증 매니저 예제

```csharp
using System;
using System.Threading;
using UnityEngine;
using 3kmyung.Authentication.Application;
using 3kmyung.Authentication.Infrastructure;
using 3kmyung.Authentication.Domain;

public class CompleteAuthManager : MonoBehaviour
{
    [Header("인증 설정")]
    [SerializeField] private bool usePlayFab = true;
    
    private AuthUseCases _authUseCases;
    private CancellationTokenSource _cancellationTokenSource;
    
    public event Action<string> OnSignInSuccess;
    public event Action<string> OnSignInFailed;
    public event Action OnSignOutSuccess;
    
    private void Awake()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        InitializeAuthService();
    }
    
    private void InitializeAuthService()
    {
        IAuthService authService = usePlayFab 
            ? new PlayFabAuthService() 
            : new UgsAuthService();
            
        _authUseCases = new AuthUseCases(authService);
    }
    
    public async void SignInAnonymously()
    {
        try
        {
            var playerId = await _authUseCases.SignInAnonymouslyAsync(
                _cancellationTokenSource.Token
            );
            OnSignInSuccess?.Invoke(playerId);
        }
        catch (OperationCanceledException)
        {
            OnSignInFailed?.Invoke("로그인이 취소되었습니다.");
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
            var playerId = await _authUseCases.SignInWithProviderAsync(
                AuthProvider.Google, 
                googleToken, 
                _cancellationTokenSource.Token
            );
            OnSignInSuccess?.Invoke(playerId);
        }
        catch (OperationCanceledException)
        {
            OnSignInFailed?.Invoke("로그인이 취소되었습니다.");
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
            await _authUseCases.RegisterWithUsernamePasswordAsync(
                username, 
                password, 
                _cancellationTokenSource.Token
            );
            Debug.Log("사용자 등록 성공");
        }
        catch (OperationCanceledException)
        {
            Debug.Log("사용자 등록이 취소되었습니다.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"사용자 등록 실패: {ex.Message}");
        }
    }
    
    public async void SignInWithUsernamePassword(string username, string password)
    {
        try
        {
            var playerId = await _authUseCases.SignInWithUsernamePasswordAsync(
                username, 
                password, 
                _cancellationTokenSource.Token
            );
            OnSignInSuccess?.Invoke(playerId);
        }
        catch (OperationCanceledException)
        {
            OnSignInFailed?.Invoke("로그인이 취소되었습니다.");
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
            await _authUseCases.SignOutAsync(_cancellationTokenSource.Token);
            OnSignOutSuccess?.Invoke();
        }
        catch (Exception ex)
        {
            Debug.LogError($"로그아웃 실패: {ex.Message}");
        }
    }
    
    private void OnDestroy()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _authUseCases?.Dispose();
    }
}
```

## 🔧 문제 해결

### 자주 발생하는 문제

#### 1. "PLAYFAB_PACKAGE define is missing" 오류
**원인**: PlayFab SDK가 설치되지 않았거나 스크립팅 정의 심볼이 설정되지 않음
**해결방법**: 
- PlayFab SDK 설치 확인
- `PLAYFAB_PACKAGE` 심볼 추가

#### 2. "UGS_PACKAGE define is missing" 오류
**원인**: Unity Gaming Services 패키지가 설치되지 않았거나 스크립팅 정의 심볼이 설정되지 않음
**해결방법**:
- UGS 패키지 설치 확인
- `UGS_PACKAGE` 심볼 추가

#### 3. 로그인 실패
**원인**: 네트워크 연결 문제, 잘못된 토큰, 서비스 설정 오류
**해결방법**:
- 네트워크 연결 확인
- 토큰 유효성 검증
- 서비스 설정 확인

### 디버깅 팁

1. **로그 확인**: Unity Console에서 상세한 오류 메시지 확인
2. **네트워크 상태**: 인터넷 연결 상태 확인
3. **토큰 검증**: 소셜 로그인 토큰의 유효성 확인
4. **서비스 설정**: PlayFab/UGS 프로젝트 설정 확인

## 📄 라이선스

이 패키지는 3kmyung 내부 사용을 위한 것입니다.

## 🤝 지원

문제가 발생하거나 질문이 있으시면 개발팀에 문의해주세요.

---

**버전**: 1.0.0  
**Unity 버전**: 2022.3 LTS 이상  
**마지막 업데이트**: 2024년 1월
