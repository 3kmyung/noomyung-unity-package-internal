using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using _3kmyung.Authentication.Application;
using _3kmyung.Authentication.Infrastructure;
using _3kmyung.Authentication.Domain;
using Cysharp.Threading.Tasks;

namespace Noomyung.Develop.Authentication
{
    /// <summary>
    /// 인증 테스트를 위한 매니저 클래스입니다.
    /// Input Field를 사용하여 사용자명과 비밀번호로 회원가입 및 로그인을 테스트할 수 있습니다.
    /// </summary>
    public class AuthTestManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_InputField usernameInputField;
        [SerializeField] private TMP_InputField passwordInputField;
        [SerializeField] private Button registerButton;
        [SerializeField] private Button loginButton;
        [SerializeField] private Button anonymousLoginButton;
        [SerializeField] private Button signOutButton;
        [SerializeField] private Button resetAllButton;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private TextMeshProUGUI playerIdText;


        private AuthenticationUseCases _authUseCases;
        private CancellationTokenSource _cancellationTokenSource;

        private void Start()
        {
            InitializeAuthService();
            SetupUI();
        }

        /// <summary>
        /// 인증 서비스를 초기화합니다.
        /// </summary>
        private void InitializeAuthService()
        {
            try
            {
#if UGS_PACKAGE
                var ugsService = new UgsAuthenticationService();
                _authUseCases = new AuthenticationUseCases(ugsService);
                Debug.Log("UGS 인증 서비스가 초기화되었습니다.");
#else
                var playFabService = new PlayFabAuthenticationService();
                _authUseCases = new AuthenticationUseCases(playFabService);
                Debug.Log("PlayFab 인증 서비스가 초기화되었습니다.");
#endif


                UpdateStatusText("인증 서비스가 초기화되었습니다.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"인증 서비스 초기화 실패: {ex.Message}");
                UpdateStatusText($"초기화 실패: {ex.Message}");
            }
        }

        /// <summary>
        /// UI를 설정합니다.
        /// </summary>
        private void SetupUI()
        {
            if (registerButton != null)
                registerButton.onClick.AddListener(OnRegisterButtonClicked);

            if (loginButton != null)
                loginButton.onClick.AddListener(OnLoginButtonClicked);

            if (anonymousLoginButton != null)
                anonymousLoginButton.onClick.AddListener(OnAnonymousLoginButtonClicked);

            if (signOutButton != null)
                signOutButton.onClick.AddListener(OnSignOutButtonClicked);

            if (resetAllButton != null)
                resetAllButton.onClick.AddListener(OnResetAllButtonClicked);

            UpdatePlayerIdText("로그인되지 않음");
        }

        /// <summary>
        /// 회원가입 버튼 클릭 이벤트
        /// </summary>
        public async void OnRegisterButtonClicked()
        {
            if (_authUseCases == null)
            {
                UpdateStatusText("인증 서비스가 초기화되지 않았습니다.");
                return;
            }

            var username = usernameInputField?.text;
            var password = passwordInputField?.text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                UpdateStatusText("사용자명과 비밀번호를 입력해주세요.");
                return;
            }

            try
            {
                UpdateStatusText("회원가입 중...");
                SetButtonsInteractable(false);

                _cancellationTokenSource = new CancellationTokenSource();
                await _authUseCases.RegisterWithUsernamePasswordAsync(username, password, _cancellationTokenSource.Token).AsUniTask();

                // 회원가입 성공 후 Player ID를 가져와서 UI에 표시
                try
                {
                    var playerId = await _authUseCases.GetPlayerIdAsync(_cancellationTokenSource.Token).AsUniTask();
                    UpdateStatusText($"회원가입 성공: {username}");
                    UpdatePlayerIdText($"Player ID: {playerId}");
                    Debug.Log($"회원가입 성공: {username}, Player ID: {playerId}");
                }
                catch (Exception ex)
                {
                    UpdateStatusText($"회원가입 성공: {username} (Player ID 가져오기 실패: {ex.Message})");
                    UpdatePlayerIdText("Player ID: 알 수 없음");
                    Debug.LogWarning($"회원가입 성공했지만 Player ID 가져오기 실패: {ex.Message}");
                }
            }
            catch (OperationCanceledException)
            {
                UpdateStatusText("회원가입이 취소되었습니다.");
                Debug.Log("회원가입이 취소되었습니다.");
            }
            catch (Exception ex)
            {
                UpdateStatusText($"회원가입 실패: {ex.Message}");
                Debug.LogError($"회원가입 실패: {ex.Message}");
            }
            finally
            {
                SetButtonsInteractable(true);
            }
        }

        /// <summary>
        /// 로그인 버튼 클릭 이벤트
        /// </summary>
        public async void OnLoginButtonClicked()
        {
            if (_authUseCases == null)
            {
                UpdateStatusText("인증 서비스가 초기화되지 않았습니다.");
                return;
            }

            var username = usernameInputField?.text;
            var password = passwordInputField?.text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                UpdateStatusText("사용자명과 비밀번호를 입력해주세요.");
                return;
            }

            // 이미 로그인되어 있는지 확인
            if (await IsAlreadySignedInAsync())
            {
                UpdateStatusText("이미 로그인되어 있습니다. 로그아웃 후 다시 시도해주세요.");
                return;
            }

            try
            {
                UpdateStatusText("로그인 중...");
                SetButtonsInteractable(false);

                _cancellationTokenSource = new CancellationTokenSource();
                var playerId = await _authUseCases.SignInWithUsernamePasswordAsync(username, password, _cancellationTokenSource.Token).AsUniTask();

                UpdateStatusText($"로그인 성공: {username}");
                UpdatePlayerIdText($"Player ID: {playerId}");
                Debug.Log($"로그인 성공: {username}, Player ID: {playerId}");
            }
            catch (OperationCanceledException)
            {
                UpdateStatusText("로그인이 취소되었습니다.");
                Debug.Log("로그인이 취소되었습니다.");
            }
            catch (Exception ex)
            {
                UpdateStatusText($"로그인 실패: {ex.Message}");
                Debug.LogError($"로그인 실패: {ex.Message}");
            }
            finally
            {
                SetButtonsInteractable(true);
            }
        }

        /// <summary>
        /// 익명 로그인 버튼 클릭 이벤트
        /// </summary>
        public async void OnAnonymousLoginButtonClicked()
        {
            if (_authUseCases == null)
            {
                UpdateStatusText("인증 서비스가 초기화되지 않았습니다.");
                return;
            }

            // 이미 로그인되어 있는지 확인
            if (await IsAlreadySignedInAsync())
            {
                UpdateStatusText("이미 로그인되어 있습니다. 로그아웃 후 다시 시도해주세요.");
                return;
            }

            try
            {
                UpdateStatusText("익명 로그인 중...");
                SetButtonsInteractable(false);

                _cancellationTokenSource = new CancellationTokenSource();
                var playerId = await _authUseCases.SignInAnonymouslyAsync(_cancellationTokenSource.Token).AsUniTask();

                UpdateStatusText("익명 로그인 성공");
                UpdatePlayerIdText($"Player ID: {playerId}");
                Debug.Log($"익명 로그인 성공, Player ID: {playerId}");
            }
            catch (OperationCanceledException)
            {
                UpdateStatusText("익명 로그인이 취소되었습니다.");
                Debug.Log("익명 로그인이 취소되었습니다.");
            }
            catch (Exception ex)
            {
                UpdateStatusText($"익명 로그인 실패: {ex.Message}");
                Debug.LogError($"익명 로그인 실패: {ex.Message}");
            }
            finally
            {
                SetButtonsInteractable(true);
            }
        }

        /// <summary>
        /// 로그아웃 버튼 클릭 이벤트
        /// </summary>
        public async void OnSignOutButtonClicked()
        {
            if (_authUseCases == null)
            {
                UpdateStatusText("인증 서비스가 초기화되지 않았습니다.");
                return;
            }

            try
            {
                UpdateStatusText("로그아웃 중...");
                SetButtonsInteractable(false);

                _cancellationTokenSource = new CancellationTokenSource();
                await _authUseCases.SignOutAsync(_cancellationTokenSource.Token).AsUniTask();

                UpdateStatusText("로그아웃 성공");
                UpdatePlayerIdText("로그인되지 않음");
                Debug.Log("로그아웃 성공");
            }
            catch (OperationCanceledException)
            {
                UpdateStatusText("로그아웃이 취소되었습니다.");
                Debug.Log("로그아웃이 취소되었습니다.");
            }
            catch (Exception ex)
            {
                UpdateStatusText($"로그아웃 실패: {ex.Message}");
                Debug.LogError($"로그아웃 실패: {ex.Message}");
            }
            finally
            {
                SetButtonsInteractable(true);
            }
        }

        /// <summary>
        /// 상태 텍스트를 업데이트합니다.
        /// </summary>
        /// <param name="message">표시할 메시지</param>
        private void UpdateStatusText(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
        }

        /// <summary>
        /// Player ID 텍스트를 업데이트합니다.
        /// </summary>
        /// <param name="message">표시할 메시지</param>
        private void UpdatePlayerIdText(string message)
        {
            if (playerIdText != null)
            {
                playerIdText.text = message;
            }
        }

        /// <summary>
        /// 버튼들의 상호작용 가능 여부를 설정합니다.
        /// </summary>
        /// <param name="interactable">상호작용 가능 여부</param>
        private void SetButtonsInteractable(bool interactable)
        {
            if (registerButton != null)
                registerButton.interactable = interactable;

            if (loginButton != null)
                loginButton.interactable = interactable;

            if (anonymousLoginButton != null)
                anonymousLoginButton.interactable = interactable;

            if (signOutButton != null)
                signOutButton.interactable = interactable;

            if (resetAllButton != null)
                resetAllButton.interactable = interactable;
        }

        /// <summary>
        /// 현재 Player ID를 가져옵니다.
        /// </summary>
        public async void GetCurrentPlayerId()
        {
            if (_authUseCases == null)
            {
                UpdateStatusText("인증 서비스가 초기화되지 않았습니다.");
                return;
            }

            try
            {
                var playerId = await _authUseCases.GetPlayerIdAsync().AsUniTask();
                UpdatePlayerIdText($"Player ID: {playerId}");
                Debug.Log($"현재 Player ID: {playerId}");
            }
            catch (Exception ex)
            {
                UpdateStatusText($"Player ID 가져오기 실패: {ex.Message}");
                Debug.LogError($"Player ID 가져오기 실패: {ex.Message}");
            }
        }

        /// <summary>
        /// 개발용: 모든 데이터 초기화 (Context Menu)
        /// </summary>
        [ContextMenu("Reset All Authentication Data")]
        public void ResetAllAuthenticationData()
        {
            OnResetAllButtonClicked();
        }

        /// <summary>
        /// 모든 데이터 초기화 버튼 클릭 이벤트
        /// </summary>
        public async void OnResetAllButtonClicked()
        {
            if (_authUseCases == null)
            {
                UpdateStatusText("인증 서비스가 초기화되지 않았습니다.");
                return;
            }

            try
            {
                UpdateStatusText("모든 데이터 초기화 중...");
                SetButtonsInteractable(false);

                _cancellationTokenSource = new CancellationTokenSource();

                // 1. 현재 로그인된 상태라면 로그아웃
                try
                {
                    await _authUseCases.SignOutAsync(_cancellationTokenSource.Token).AsUniTask();
                    Debug.Log("로그아웃 완료");
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"로그아웃 중 오류 (무시됨): {ex.Message}");
                }

                // 2. PlayerPrefs 초기화 (로컬 저장된 인증 정보 삭제)
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
                Debug.Log("PlayerPrefs 초기화 완료");

                // 3. 인증 서비스 재초기화
                _authUseCases?.Dispose();
                InitializeAuthService();

                // 4. UI 초기화
                if (usernameInputField != null)
                    usernameInputField.text = "";
                if (passwordInputField != null)
                    passwordInputField.text = "";

                UpdateStatusText("모든 데이터 초기화 완료");
                UpdatePlayerIdText("로그인되지 않음");
                Debug.Log("모든 데이터 초기화 완료");
            }
            catch (OperationCanceledException)
            {
                UpdateStatusText("초기화가 취소되었습니다.");
                Debug.Log("초기화가 취소되었습니다.");
            }
            catch (Exception ex)
            {
                UpdateStatusText($"초기화 실패: {ex.Message}");
                Debug.LogError($"초기화 실패: {ex.Message}");
            }
            finally
            {
                SetButtonsInteractable(true);
            }
        }

        /// <summary>
        /// 이미 로그인되어 있는지 확인합니다.
        /// </summary>
        /// <returns>로그인되어 있으면 true, 그렇지 않으면 false</returns>
        private async UniTask<bool> IsAlreadySignedInAsync()
        {
            try
            {
                bool isSignedIn = await _authUseCases.IsSignedInAsync().AsUniTask();
                var playerId = await _authUseCases.GetPlayerIdAsync().AsUniTask();

                Debug.Log($"Login state check - IsSignedIn: {isSignedIn}, PlayerId: '{playerId}'");
                return isSignedIn;
            }
            catch (Exception ex)
            {
                // 로그인 상태를 확인할 수 없으면 로그인되지 않은 것으로 간주
                Debug.Log($"Login state check failed (assuming not signed in): {ex.Message}");
                return false;
            }
        }

        private void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _authUseCases?.Dispose();
        }
    }
}
