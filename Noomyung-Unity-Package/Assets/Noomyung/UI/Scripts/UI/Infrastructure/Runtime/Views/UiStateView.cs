using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Noomyung.UI.Application.Ports;

namespace Noomyung.UI.Infrastructure.Runtime.Views
{
    /// <summary>
    /// UI 요소의 상태 전환을 관리하는 컴포넌트입니다.
    /// Normal, Hovered, Pressed, Disabled, Selected 상태를 관리합니다.
    /// </summary>
    public class UiStateView : MonoBehaviour, IUiStateView
    {
        [Header("State Settings")]
        [SerializeField] private UiState _initialState = UiState.Normal;
        [SerializeField] private float _stateTransitionDuration = 0.2f;
        [SerializeField] private bool _ignoreTimeScale = true;

        [Header("State Visuals")]
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _hoveredColor = new Color(1.2f, 1.2f, 1.2f, 1f);
        [SerializeField] private Color _pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        [SerializeField] private Color _disabledColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        [SerializeField] private Color _selectedColor = new Color(1f, 1f, 0.5f, 1f);

        [Header("Components")]
        [SerializeField] private UnityEngine.UI.Graphic _targetGraphic;

        private UiState _currentState;
        private bool _isTransitioning;
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// 현재 UI 요소의 상태를 가져옵니다.
        /// </summary>
        public UiState CurrentState => _currentState;

        private void Awake()
        {
            if (_targetGraphic == null)
            {
                _targetGraphic = GetComponent<UnityEngine.UI.Graphic>();
            }

            _currentState = _initialState;
            _cancellationTokenSource = new CancellationTokenSource();

            // 초기 상태 적용
            ApplyStateVisual(_currentState);
        }

        private void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        /// <summary>
        /// UI 요소의 상태를 변경합니다.
        /// </summary>
        /// <param name="state">새로운 상태</param>
        /// <param name="cancellationToken">취소 토큰</param>
        /// <returns>비동기 작업</returns>
        public async Task ChangeStateAsync(UiState state, CancellationToken cancellationToken = default)
        {
            if (_currentState == state || _isTransitioning) return;

            var previousState = _currentState;
            _currentState = state;
            _isTransitioning = true;

            try
            {
                await TransitionToStateAsync(previousState, state, cancellationToken);
            }
            finally
            {
                _isTransitioning = false;
            }
        }

        private async UniTask TransitionToStateAsync(UiState fromState, UiState toState, CancellationToken cancellationToken)
        {
            if (_targetGraphic == null) return;

            Color fromColor = GetStateColor(fromState);
            Color toColor = GetStateColor(toState);

            float elapsed = 0f;

            while (elapsed < _stateTransitionDuration && !cancellationToken.IsCancellationRequested)
            {
                var progress = elapsed / _stateTransitionDuration;
                var easedProgress = Mathf.SmoothStep(0f, 1f, progress);

                _targetGraphic.color = Color.Lerp(fromColor, toColor, easedProgress);

                await UniTask.Yield(cancellationToken: cancellationToken);
                elapsed += _ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                _targetGraphic.color = toColor;
            }
        }

        private void ApplyStateVisual(UiState state)
        {
            if (_targetGraphic == null) return;

            _targetGraphic.color = GetStateColor(state);
        }

        private Color GetStateColor(UiState state)
        {
            return state switch
            {
                UiState.Normal => _normalColor,
                UiState.Hovered => _hoveredColor,
                UiState.Pressed => _pressedColor,
                UiState.Disabled => _disabledColor,
                UiState.Selected => _selectedColor,
                _ => _normalColor
            };
        }

        /// <summary>
        /// 상태 전환을 취소합니다.
        /// </summary>
        public void CancelStateTransition()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();

            _isTransitioning = false;
            ApplyStateVisual(_currentState);
        }

        /// <summary>
        /// 즉시 상태를 변경합니다 (전환 효과 없이).
        /// </summary>
        /// <param name="state">새로운 상태</param>
        public void SetStateImmediate(UiState state)
        {
            _currentState = state;
            ApplyStateVisual(state);
        }

        private void OnValidate()
        {
            if (_targetGraphic == null)
            {
                _targetGraphic = GetComponent<UnityEngine.UI.Graphic>();
            }
        }
    }
}
