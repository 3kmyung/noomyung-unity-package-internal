using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Noomyung.CloudSave.Application;
using Noomyung.CloudSave.Domain;
using Noomyung.CloudSave.Infrastructure;
using Newtonsoft.Json;

namespace Noomyung.Develop.CloudSave
{
    /// <summary>
    /// CloudSave 기능을 테스트하는 매니저 클래스
    /// </summary>
    public class CloudSaveTestManager : MonoBehaviour
    {
        [Header("테스트 설정")]
        [SerializeField] private bool runTestsOnStart = false;
        [SerializeField] private bool enableDetailedLogging = true;

        private ICloudSaveService _cloudSaveService;
        private CloudSaveUseCases _useCases;
        private CloudSaveJsonUseCases _jsonUseCases;
        private IJsonSerializer _serializer;

        private void Start()
        {
            if (runTestsOnStart)
            {
                _ = RunAllTestsAsync();
            }
        }

        /// <summary>
        /// CloudSave 서비스를 초기화합니다
        /// </summary>
        public void InitializeCloudSave()
        {
            try
            {
                // JSON 직렬화기 초기화
                _serializer = new NewtonsoftJsonSerializer();

                // 클라우드 저장 서비스 초기화
#if UGS_PACKAGE
                _cloudSaveService = new UgsCloudSaveService();
                LogInfo("UGS CloudSave 서비스로 초기화되었습니다.");
#else
                _cloudSaveService = new PlayFabCloudSaveService();
                LogInfo("PlayFab CloudSave 서비스로 초기화되었습니다.");
#endif

                // 유스케이스 초기화
                _useCases = new CloudSaveUseCases(_cloudSaveService);
                _jsonUseCases = new CloudSaveJsonUseCases(_cloudSaveService, _serializer);

                LogInfo("CloudSave 서비스가 성공적으로 초기화되었습니다.");
            }
            catch (Exception ex)
            {
                LogError($"CloudSave 서비스 초기화 실패: {ex.Message}");
            }
        }

        /// <summary>
        /// 모든 테스트를 실행합니다
        /// </summary>
        public async Task RunAllTestsAsync()
        {
            LogInfo("=== CloudSave 테스트 시작 ===");

            try
            {
                InitializeCloudSave();

                if (_cloudSaveService == null)
                {
                    LogError("CloudSave 서비스가 초기화되지 않았습니다.");
                    return;
                }

                // 기본 바이트 배열 테스트
                await TestBasicByteOperationsAsync();

                // JSON 직렬화 테스트
                await TestJsonSerializationAsync();

                // 키 관리 테스트
                await TestKeyManagementAsync();

                // 오류 처리 테스트
                await TestErrorHandlingAsync();

                LogInfo("=== 모든 테스트가 성공적으로 완료되었습니다 ===");
            }
            catch (Exception ex)
            {
                LogError($"테스트 실행 중 오류 발생: {ex.Message}");
            }
        }

        /// <summary>
        /// 기본 바이트 배열 작업을 테스트합니다
        /// </summary>
        private async Task TestBasicByteOperationsAsync()
        {
            LogInfo("--- 기본 바이트 배열 테스트 시작 ---");

            try
            {
                // 테스트 데이터 준비
                string testString = "안녕하세요, CloudSave 테스트입니다!";
                byte[] testData = Encoding.UTF8.GetBytes(testString);
                string testKey = "test_byte_data";

                // 데이터 저장
                LogInfo($"데이터 저장 중... (키: {testKey})");
                await _useCases.SaveBytesAsync(testKey, testData);
                LogInfo("데이터 저장 완료");

                // 데이터 로드
                LogInfo($"데이터 로드 중... (키: {testKey})");
                byte[] loadedData = await _useCases.LoadBytesAsync(testKey);
                
                if (loadedData != null)
                {
                    string loadedString = Encoding.UTF8.GetString(loadedData);
                    LogInfo($"로드된 데이터: {loadedString}");
                    
                    if (loadedString == testString)
                    {
                        LogInfo("✓ 바이트 배열 저장/로드 테스트 성공");
                    }
                    else
                    {
                        LogError("✗ 바이트 배열 저장/로드 테스트 실패: 데이터 불일치");
                    }
                }
                else
                {
                    LogError("✗ 바이트 배열 로드 테스트 실패: 데이터가 null입니다");
                }

                // 테스트 데이터 정리
                await _cloudSaveService.DeleteAsync(testKey);
                LogInfo("테스트 데이터 정리 완료");
            }
            catch (Exception ex)
            {
                LogError($"바이트 배열 테스트 중 오류: {ex.Message}");
            }

            LogInfo("--- 기본 바이트 배열 테스트 완료 ---");
        }

        /// <summary>
        /// JSON 직렬화를 테스트합니다
        /// </summary>
        private async Task TestJsonSerializationAsync()
        {
            LogInfo("--- JSON 직렬화 테스트 시작 ---");

            try
            {
                // 플레이어 데이터 테스트
                await TestPlayerDataSerializationAsync();

                // 게임 설정 데이터 테스트
                await TestGameSettingsSerializationAsync();

                // 복잡한 데이터 구조 테스트
                await TestComplexDataSerializationAsync();
            }
            catch (Exception ex)
            {
                LogError($"JSON 직렬화 테스트 중 오류: {ex.Message}");
            }

            LogInfo("--- JSON 직렬화 테스트 완료 ---");
        }

        /// <summary>
        /// 플레이어 데이터 직렬화를 테스트합니다
        /// </summary>
        private async Task TestPlayerDataSerializationAsync()
        {
            LogInfo("플레이어 데이터 직렬화 테스트 시작");

            var originalData = new PlayerTestData("테스트플레이어", 25, 15000, 1250.5f, true);
            string testKey = "test_player_data";

            // 데이터 저장
            await _jsonUseCases.SaveAsync(testKey, originalData);
            LogInfo($"플레이어 데이터 저장 완료: {originalData}");

            // 데이터 로드
            var loadedData = await _jsonUseCases.LoadAsync<PlayerTestData>(testKey);
            
            if (loadedData != null && originalData.Equals(loadedData))
            {
                LogInfo("✓ 플레이어 데이터 직렬화 테스트 성공");
            }
            else
            {
                LogError("✗ 플레이어 데이터 직렬화 테스트 실패");
                LogError($"원본: {originalData}");
                LogError($"로드: {loadedData}");
            }

            // 정리
            await _cloudSaveService.DeleteAsync(testKey);
        }

        /// <summary>
        /// 게임 설정 데이터 직렬화를 테스트합니다
        /// </summary>
        private async Task TestGameSettingsSerializationAsync()
        {
            LogInfo("게임 설정 데이터 직렬화 테스트 시작");

            var originalSettings = new GameSettingsData
            {
                MasterVolume = 0.7f,
                MusicVolume = 0.5f,
                SfxVolume = 0.8f,
                FullScreen = true,
                QualityLevel = 3,
                Language = "English"
            };
            string testKey = "test_game_settings";

            // 데이터 저장
            await _jsonUseCases.SaveAsync(testKey, originalSettings);
            LogInfo($"게임 설정 저장 완료: {originalSettings}");

            // 데이터 로드
            var loadedSettings = await _jsonUseCases.LoadAsync<GameSettingsData>(testKey);
            
            if (loadedSettings != null && originalSettings.Equals(loadedSettings))
            {
                LogInfo("✓ 게임 설정 직렬화 테스트 성공");
            }
            else
            {
                LogError("✗ 게임 설정 직렬화 테스트 실패");
                LogError($"원본: {originalSettings}");
                LogError($"로드: {loadedSettings}");
            }

            // 정리
            await _cloudSaveService.DeleteAsync(testKey);
        }

        /// <summary>
        /// 복잡한 데이터 구조 직렬화를 테스트합니다
        /// </summary>
        private async Task TestComplexDataSerializationAsync()
        {
            LogInfo("복잡한 데이터 구조 직렬화 테스트 시작");

            var complexData = new Dictionary<string, object>
            {
                ["player"] = new PlayerTestData("복잡한플레이어", 50, 50000, 5000f, true),
                ["settings"] = new GameSettingsData(),
                ["inventory"] = new List<string> { "sword", "shield", "potion", "key" },
                ["stats"] = new Dictionary<string, int>
                {
                    ["strength"] = 100,
                    ["agility"] = 80,
                    ["intelligence"] = 90
                }
            };
            string testKey = "test_complex_data";

            // 데이터 저장
            await _jsonUseCases.SaveAsync(testKey, complexData);
            LogInfo("복잡한 데이터 저장 완료");

            // 데이터 로드
            var loadedData = await _jsonUseCases.LoadAsync<Dictionary<string, object>>(testKey);
            
            if (loadedData != null)
            {
                LogInfo("✓ 복잡한 데이터 구조 직렬화 테스트 성공");
                LogInfo($"로드된 데이터 키 수: {loadedData.Count}");
            }
            else
            {
                LogError("✗ 복잡한 데이터 구조 직렬화 테스트 실패");
            }

            // 정리
            await _cloudSaveService.DeleteAsync(testKey);
        }

        /// <summary>
        /// 키 관리 기능을 테스트합니다
        /// </summary>
        private async Task TestKeyManagementAsync()
        {
            LogInfo("--- 키 관리 테스트 시작 ---");

            try
            {
                // 여러 테스트 키 생성
                var testKeys = new[] { "test_key_1", "test_key_2", "player_data_1", "player_data_2", "settings_1" };
                
                foreach (var key in testKeys)
                {
                    byte[] data = Encoding.UTF8.GetBytes($"테스트 데이터: {key}");
                    await _useCases.SaveBytesAsync(key, data);
                }
                LogInfo($"{testKeys.Length}개의 테스트 키 생성 완료");

                // 모든 키 나열
                var allKeys = await _cloudSaveService.ListKeysAsync();
                LogInfo($"전체 키 수: {allKeys.Count}");
                foreach (var key in allKeys)
                {
                    LogInfo($"  - {key}");
                }

                // 접두사로 키 필터링
                var playerKeys = await _cloudSaveService.ListKeysAsync("player_");
                LogInfo($"'player_' 접두사 키 수: {playerKeys.Count}");
                foreach (var key in playerKeys)
                {
                    LogInfo($"  - {key}");
                }

                // 키 존재 여부 확인
                foreach (var key in testKeys)
                {
                    bool exists = await _cloudSaveService.HasKeyAsync(key);
                    if (exists)
                    {
                        LogInfo($"✓ 키 '{key}' 존재 확인");
                    }
                    else
                    {
                        LogError($"✗ 키 '{key}' 존재하지 않음");
                    }
                }

                // 존재하지 않는 키 확인
                bool nonExistentKey = await _cloudSaveService.HasKeyAsync("non_existent_key");
                if (!nonExistentKey)
                {
                    LogInfo("✓ 존재하지 않는 키 확인 테스트 성공");
                }
                else
                {
                    LogError("✗ 존재하지 않는 키 확인 테스트 실패");
                }

                // 테스트 데이터 정리
                foreach (var key in testKeys)
                {
                    await _cloudSaveService.DeleteAsync(key);
                }
                LogInfo("테스트 데이터 정리 완료");
            }
            catch (Exception ex)
            {
                LogError($"키 관리 테스트 중 오류: {ex.Message}");
            }

            LogInfo("--- 키 관리 테스트 완료 ---");
        }

        /// <summary>
        /// 오류 처리를 테스트합니다
        /// </summary>
        private async Task TestErrorHandlingAsync()
        {
            LogInfo("--- 오류 처리 테스트 시작 ---");

            try
            {
                // 존재하지 않는 키 로드
                LogInfo("존재하지 않는 키 로드 테스트");
                byte[] nonExistentData = await _useCases.LoadBytesAsync("non_existent_key");
                if (nonExistentData == null)
                {
                    LogInfo("✓ 존재하지 않는 키 로드 시 null 반환 확인");
                }
                else
                {
                    LogError("✗ 존재하지 않는 키 로드 시 null이 아닌 값 반환");
                }

                // 존재하지 않는 JSON 데이터 로드
                LogInfo("존재하지 않는 JSON 데이터 로드 테스트");
                var nonExistentJson = await _jsonUseCases.LoadAsync<PlayerTestData>("non_existent_json_key");
                if (nonExistentJson == null)
                {
                    LogInfo("✓ 존재하지 않는 JSON 데이터 로드 시 null 반환 확인");
                }
                else
                {
                    LogError("✗ 존재하지 않는 JSON 데이터 로드 시 null이 아닌 값 반환");
                }

                // 빈 키로 저장 시도
                LogInfo("빈 키로 저장 시도 테스트");
                try
                {
                    await _useCases.SaveBytesAsync("", Encoding.UTF8.GetBytes("test"));
                    LogError("✗ 빈 키로 저장이 허용됨");
                }
                catch (Exception ex)
                {
                    LogInfo($"✓ 빈 키로 저장 시 예외 발생: {ex.Message}");
                }

                // null 데이터 저장 시도
                LogInfo("null 데이터 저장 시도 테스트");
                try
                {
                    await _useCases.SaveBytesAsync("null_test", null);
                    LogInfo("✓ null 데이터 저장 허용됨");
                }
                catch (Exception ex)
                {
                    LogInfo($"null 데이터 저장 시 예외 발생: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                LogError($"오류 처리 테스트 중 예상치 못한 오류: {ex.Message}");
            }

            LogInfo("--- 오류 처리 테스트 완료 ---");
        }

        /// <summary>
        /// 개별 테스트 메서드들
        /// </summary>
        [ContextMenu("바이트 배열 테스트 실행")]
        public async Task RunByteArrayTest()
        {
            InitializeCloudSave();
            await TestBasicByteOperationsAsync();
        }

        [ContextMenu("JSON 직렬화 테스트 실행")]
        public async Task RunJsonSerializationTest()
        {
            InitializeCloudSave();
            await TestJsonSerializationAsync();
        }

        [ContextMenu("키 관리 테스트 실행")]
        public async Task RunKeyManagementTest()
        {
            InitializeCloudSave();
            await TestKeyManagementAsync();
        }

        [ContextMenu("오류 처리 테스트 실행")]
        public async Task RunErrorHandlingTest()
        {
            InitializeCloudSave();
            await TestErrorHandlingAsync();
        }

        [ContextMenu("모든 테스트 실행")]
        public async Task RunAllTests()
        {
            await RunAllTestsAsync();
        }

        /// <summary>
        /// 로깅 메서드들
        /// </summary>
        private void LogInfo(string message)
        {
            if (enableDetailedLogging)
            {
                Debug.Log($"[CloudSaveTest] {message}");
            }
        }

        private void LogError(string message)
        {
            Debug.LogError($"[CloudSaveTest] {message}");
        }

        /// <summary>
        /// 리소스 정리
        /// </summary>
        private void OnDestroy()
        {
            _useCases?.Dispose();
            _jsonUseCases?.Dispose();
            _cloudSaveService?.Dispose();
        }
    }
}
