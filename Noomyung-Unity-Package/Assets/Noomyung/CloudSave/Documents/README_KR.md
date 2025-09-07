# Noomyung CloudSave 패키지

여러 클라우드 제공업체를 지원하는 깔끔하고 아키텍처 기반의 클라우드 저장 시스템을 제공하는 Unity 패키지입니다.

## 주요 기능

- **클린 아키텍처**: 관심사의 명확한 분리와 클린 아키텍처 원칙을 따름
- **다중 제공업체**: Unity Gaming Services (UGS)와 PlayFab 클라우드 저장 서비스 지원
- **타입 안전성**: 강력한 타입 인터페이스와 제네릭 메서드
- **비동기 지원**: 취소 토큰 지원과 완전한 async/await 지원
- **JSON 직렬화**: 복잡한 객체를 위한 내장 JSON 직렬화
- **바이트 배열 지원**: 바이너리 데이터를 위한 직접 바이트 배열 저장

## 아키텍처

패키지는 클린 아키텍처 원칙을 사용하여 구조화되어 있습니다:

```
CloudSave/
├── Domain/           # 핵심 비즈니스 로직 및 인터페이스
├── Application/      # 유스케이스 및 애플리케이션 서비스
└── Infrastructure/   # 외부 서비스 구현
    ├── UGS/         # Unity Gaming Services 구현
    └── PlayFab/     # PlayFab 구현
```

## 설치

### 사전 요구사항

다음 클라우드 저장 제공업체 중 하나를 선택하세요:

#### Unity Gaming Services (UGS)
1. Unity Gaming Services 패키지 설치:
   - `Unity.Services.Core`
   - `Unity.Services.CloudSave`
2. Scripting Define Symbols에 `UGS_PACKAGE` 추가

#### PlayFab
1. PlayFab SDK 설치
2. Scripting Define Symbols에 `PLAYFAB_PACKAGE` 추가

## 사용법

### 기본 설정

```csharp
using Noomyung.CloudSave.Application;
using Noomyung.CloudSave.Infrastructure;

// UGS용
var cloudSaveService = new UgsCloudSaveService();
var useCases = new CloudSaveUseCases(cloudSaveService);

// PlayFab용
var cloudSaveService = new PlayFabCloudSaveService();
var useCases = new CloudSaveUseCases(cloudSaveService);
```

### 데이터 저장

#### 바이트 배열
```csharp
byte[] data = Encoding.UTF8.GetBytes("안녕하세요, 클라우드 저장!");
await useCases.SaveBytesAsync("my_data", data);
```

#### JSON 객체
```csharp
using Noomyung.CloudSave.Infrastructure;

var serializer = new NewtonsoftJsonSerializer();
var jsonUseCases = new CloudSaveJsonUseCases(cloudSaveService, serializer);

var playerData = new PlayerData
{
    Name = "플레이어1",
    Level = 10,
    Score = 1500
};

await jsonUseCases.SaveAsync("player_data", playerData);
```

### 데이터 로드

#### 바이트 배열
```csharp
byte[] loadedData = await useCases.LoadBytesAsync("my_data");
string text = Encoding.UTF8.GetString(loadedData);
```

#### JSON 객체
```csharp
var playerData = await jsonUseCases.LoadAsync<PlayerData>("player_data");
```

### 추가 작업

```csharp
// 키 존재 여부 확인
bool exists = await cloudSaveService.HasKeyAsync("my_data");

// 모든 키 나열
var allKeys = await cloudSaveService.ListKeysAsync();

// 접두사로 키 나열
var playerKeys = await cloudSaveService.ListKeysAsync("player_");

// 데이터 삭제
await cloudSaveService.DeleteAsync("my_data");
```

## API 참조

### ICloudSaveService

클라우드 저장 작업을 위한 핵심 인터페이스:

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

기본 바이트 배열 작업을 위한 애플리케이션 레이어:

```csharp
public sealed class CloudSaveUseCases : IDisposable
{
    public Task SaveBytesAsync(string key, byte[] data, CancellationToken cancellationToken = default);
    public Task<byte[]> LoadBytesAsync(string key, CancellationToken cancellationToken = default);
}
```

### CloudSaveJsonUseCases

JSON 직렬화를 위한 애플리케이션 레이어:

```csharp
public sealed class CloudSaveJsonUseCases : IDisposable
{
    public Task SaveAsync<T>(string key, T value, CancellationToken cancellationToken = default);
    public Task<T> LoadAsync<T>(string key, CancellationToken cancellationToken = default);
}
```

## 오류 처리

패키지는 포괄적인 오류 처리를 포함합니다:

- 네트워크 오류는 로그에 기록되고 예외가 발생합니다
- 누락된 데이터는 바이트 배열의 경우 `null`, JSON 객체의 경우 `default(T)`를 반환합니다
- 취소 토큰은 API 전체에서 존중됩니다

## 모범 사례

1. **리소스 해제**: 완료되면 유스케이스와 서비스를 항상 해제하세요
2. **오류 처리**: 클라우드 저장 작업을 try-catch 블록으로 감싸세요
3. **취소**: 장기 실행 작업에 취소 토큰을 사용하세요
4. **키 명명**: 일관되고 설명적인 키 명명 규칙을 사용하세요
5. **데이터 검증**: 사용하기 전에 로드된 데이터를 검증하세요

## 예제: 완전한 플레이어 데이터 관리

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
            Debug.Log("플레이어 데이터가 성공적으로 저장되었습니다");
        }
        catch (Exception ex)
        {
            Debug.LogError($"플레이어 데이터 저장 실패: {ex.Message}");
        }
    }
    
    public async Task<PlayerData> LoadPlayerDataAsync()
    {
        try
        {
            var data = await _cloudSave.LoadAsync<PlayerData>("player_data");
            return data ?? new PlayerData(); // 데이터가 없으면 기본값 반환
        }
        catch (Exception ex)
        {
            Debug.LogError($"플레이어 데이터 로드 실패: {ex.Message}");
            return new PlayerData();
        }
    }
    
    public void Dispose()
    {
        _cloudSave?.Dispose();
    }
}
```

## 라이선스

이 패키지는 Noomyung Unity 패키지 컬렉션의 일부입니다.
