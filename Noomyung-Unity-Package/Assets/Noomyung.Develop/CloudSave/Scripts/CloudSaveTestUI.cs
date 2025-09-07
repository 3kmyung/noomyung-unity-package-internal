using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Noomyung.Develop.CloudSave
{
    /// <summary>
    /// CloudSave 테스트를 위한 UI 컨트롤러
    /// </summary>
    public class CloudSaveTestUI : MonoBehaviour
    {
        [Header("UI 컴포넌트")]
        [SerializeField] private Button initializeButton;
        [SerializeField] private Button runAllTestsButton;
        [SerializeField] private Button byteArrayTestButton;
        [SerializeField] private Button jsonTestButton;
        [SerializeField] private Button keyManagementTestButton;
        [SerializeField] private Button errorHandlingTestButton;
        [SerializeField] private Button clearLogButton;

        [Header("설정")]
        [SerializeField] private Toggle detailedLoggingToggle;

        [Header("로그 출력")]
        [SerializeField] private ScrollRect logScrollRect;
        [SerializeField] private Text logText;
        [SerializeField] private int maxLogLines = 100;

        private CloudSaveTestManager _testManager;
        private string _logBuffer = "";

        private void Awake()
        {
            // 테스트 매니저 찾기 또는 생성
            _testManager = FindFirstObjectByType<CloudSaveTestManager>();
            if (_testManager == null)
            {
                GameObject managerObj = new GameObject("CloudSaveTestManager");
                _testManager = managerObj.AddComponent<CloudSaveTestManager>();
            }

            // UI 이벤트 연결
            SetupUIEvents();

            // 초기 로그 메시지
            AddLog("CloudSave 테스트 UI가 초기화되었습니다.");
        }

        /// <summary>
        /// UI 이벤트를 설정합니다
        /// </summary>
        private void SetupUIEvents()
        {
            if (initializeButton != null)
                initializeButton.onClick.AddListener(OnInitializeClicked);

            if (runAllTestsButton != null)
                runAllTestsButton.onClick.AddListener(OnRunAllTestsClicked);

            if (byteArrayTestButton != null)
                byteArrayTestButton.onClick.AddListener(OnByteArrayTestClicked);

            if (jsonTestButton != null)
                jsonTestButton.onClick.AddListener(OnJsonTestClicked);

            if (keyManagementTestButton != null)
                keyManagementTestButton.onClick.AddListener(OnKeyManagementTestClicked);

            if (errorHandlingTestButton != null)
                errorHandlingTestButton.onClick.AddListener(OnErrorHandlingTestClicked);

            if (clearLogButton != null)
                clearLogButton.onClick.AddListener(OnClearLogClicked);

            if (detailedLoggingToggle != null)
                detailedLoggingToggle.onValueChanged.AddListener(OnDetailedLoggingToggleChanged);
        }

        /// <summary>
        /// 초기화 버튼 클릭 이벤트
        /// </summary>
        private void OnInitializeClicked()
        {
            try
            {
                _testManager.InitializeCloudSave();
                AddLog("CloudSave 서비스 초기화 완료");
            }
            catch (Exception ex)
            {
                AddLog($"초기화 실패: {ex.Message}");
            }
        }

        /// <summary>
        /// 모든 테스트 실행 버튼 클릭 이벤트
        /// </summary>
        private async void OnRunAllTestsClicked()
        {
            AddLog("=== 모든 테스트 시작 ===");
            try
            {
                await _testManager.RunAllTests();
                AddLog("=== 모든 테스트 완료 ===");
            }
            catch (Exception ex)
            {
                AddLog($"테스트 실행 중 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 바이트 배열 테스트 버튼 클릭 이벤트
        /// </summary>
        private async void OnByteArrayTestClicked()
        {
            AddLog("바이트 배열 테스트 시작");
            try
            {
                await _testManager.RunByteArrayTest();
                AddLog("바이트 배열 테스트 완료");
            }
            catch (Exception ex)
            {
                AddLog($"바이트 배열 테스트 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// JSON 테스트 버튼 클릭 이벤트
        /// </summary>
        private async void OnJsonTestClicked()
        {
            AddLog("JSON 직렬화 테스트 시작");
            try
            {
                await _testManager.RunJsonSerializationTest();
                AddLog("JSON 직렬화 테스트 완료");
            }
            catch (Exception ex)
            {
                AddLog($"JSON 테스트 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 키 관리 테스트 버튼 클릭 이벤트
        /// </summary>
        private async void OnKeyManagementTestClicked()
        {
            AddLog("키 관리 테스트 시작");
            try
            {
                await _testManager.RunKeyManagementTest();
                AddLog("키 관리 테스트 완료");
            }
            catch (Exception ex)
            {
                AddLog($"키 관리 테스트 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 오류 처리 테스트 버튼 클릭 이벤트
        /// </summary>
        private async void OnErrorHandlingTestClicked()
        {
            AddLog("오류 처리 테스트 시작");
            try
            {
                await _testManager.RunErrorHandlingTest();
                AddLog("오류 처리 테스트 완료");
            }
            catch (Exception ex)
            {
                AddLog($"오류 처리 테스트 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 로그 지우기 버튼 클릭 이벤트
        /// </summary>
        private void OnClearLogClicked()
        {
            _logBuffer = "";
            if (logText != null)
            {
                logText.text = "";
            }
            AddLog("로그가 지워졌습니다.");
        }


        /// <summary>
        /// 상세 로깅 토글 변경 이벤트
        /// </summary>
        private void OnDetailedLoggingToggleChanged(bool isOn)
        {
            // 리플렉션을 사용하여 private 필드에 접근
            var field = typeof(CloudSaveTestManager).GetField("enableDetailedLogging", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(_testManager, isOn);
            
            AddLog($"상세 로깅 {(isOn ? "활성화" : "비활성화")}");
        }

        /// <summary>
        /// 로그에 메시지를 추가합니다
        /// </summary>
        public void AddLog(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            string logEntry = $"[{timestamp}] {message}\n";
            
            _logBuffer += logEntry;

            // 최대 로그 라인 수 제한
            string[] lines = _logBuffer.Split('\n');
            if (lines.Length > maxLogLines)
            {
                _logBuffer = string.Join("\n", lines, lines.Length - maxLogLines, maxLogLines);
            }

            if (logText != null)
            {
                logText.text = _logBuffer;
                
                // 스크롤을 맨 아래로 이동
                if (logScrollRect != null)
                {
                    Canvas.ForceUpdateCanvases();
                    logScrollRect.verticalNormalizedPosition = 0f;
                }
            }

            // Unity 콘솔에도 출력
            Debug.Log($"[CloudSaveTestUI] {message}");
        }

        /// <summary>
        /// 에러 로그를 추가합니다
        /// </summary>
        public void AddErrorLog(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            string logEntry = $"[{timestamp}] <color=red>ERROR: {message}</color>\n";
            
            _logBuffer += logEntry;

            if (logText != null)
            {
                logText.text = _logBuffer;
                
                if (logScrollRect != null)
                {
                    Canvas.ForceUpdateCanvases();
                    logScrollRect.verticalNormalizedPosition = 0f;
                }
            }

            Debug.LogError($"[CloudSaveTestUI] {message}");
        }

        /// <summary>
        /// 성공 로그를 추가합니다
        /// </summary>
        public void AddSuccessLog(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            string logEntry = $"[{timestamp}] <color=green>SUCCESS: {message}</color>\n";
            
            _logBuffer += logEntry;

            if (logText != null)
            {
                logText.text = _logBuffer;
                
                if (logScrollRect != null)
                {
                    Canvas.ForceUpdateCanvases();
                    logScrollRect.verticalNormalizedPosition = 0f;
                }
            }

            Debug.Log($"[CloudSaveTestUI] SUCCESS: {message}");
        }

        private void OnDestroy()
        {
            // UI 이벤트 해제
            if (initializeButton != null)
                initializeButton.onClick.RemoveListener(OnInitializeClicked);

            if (runAllTestsButton != null)
                runAllTestsButton.onClick.RemoveListener(OnRunAllTestsClicked);

            if (byteArrayTestButton != null)
                byteArrayTestButton.onClick.RemoveListener(OnByteArrayTestClicked);

            if (jsonTestButton != null)
                jsonTestButton.onClick.RemoveListener(OnJsonTestClicked);

            if (keyManagementTestButton != null)
                keyManagementTestButton.onClick.RemoveListener(OnKeyManagementTestClicked);

            if (errorHandlingTestButton != null)
                errorHandlingTestButton.onClick.RemoveListener(OnErrorHandlingTestClicked);

            if (clearLogButton != null)
                clearLogButton.onClick.RemoveListener(OnClearLogClicked);

            if (detailedLoggingToggle != null)
                detailedLoggingToggle.onValueChanged.RemoveListener(OnDetailedLoggingToggleChanged);
        }
    }
}
