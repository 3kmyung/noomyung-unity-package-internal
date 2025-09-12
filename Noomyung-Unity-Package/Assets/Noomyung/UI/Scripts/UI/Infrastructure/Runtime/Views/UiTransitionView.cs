using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Noomyung.UI.Application.Ports;

namespace Noomyung.UI.Infrastructure.Runtime.Views
{
    /// <summary>
    /// UI 요소의 전환 효과를 관리하는 컴포넌트입니다.
    /// 페이드 인/아웃, 애니메이션 등의 전환 효과를 담당합니다.
    /// </summary>
    public class UiTransitionView : MonoBehaviour, IUiTransitionView
    {
        [Header("Transition Settings")]
        [SerializeField] private float _fadeInDuration = 0.3f;
        [SerializeField] private float _fadeOutDuration = 0.2f;
        [SerializeField] private AnimationCurve _fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _fadeOutCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        [SerializeField] private bool _ignoreTimeScale = true;

        [Header("Components")]
        [SerializeField] private CanvasGroup _canvasGroup;

        private bool _isTransitioning;
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// 전환 효과가 현재 재생 중인지 여부를 확인합니다.
        /// </summary>
        public bool IsTransitioning => _isTransitioning;

        private void Awake()
        {
            // CanvasGroup이 없으면 자동으로 추가
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
                if (_canvasGroup == null)
                {
                    _canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }

            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        /// <summary>
        /// 표시 전환 효과를 재생합니다.
        /// </summary>
        /// <param name="cancellationToken">취소 토큰</param>
        /// <returns>비동기 작업</returns>
        public async Task PlayShowAsync(CancellationToken cancellationToken = default)
        {
            if (_isTransitioning) return;

            _isTransitioning = true;
            gameObject.SetActive(true);

            try
            {
                await FadeInAsync(cancellationToken);
            }
            finally
            {
                _isTransitioning = false;
            }
        }

        /// <summary>
        /// 숨김 전환 효과를 재생합니다.
        /// </summary>
        /// <param name="cancellationToken">취소 토큰</param>
        /// <returns>비동기 작업</returns>
        public async Task PlayHideAsync(CancellationToken cancellationToken = default)
        {
            if (_isTransitioning) return;

            _isTransitioning = true;

            try
            {
                await FadeOutAsync(cancellationToken);
                gameObject.SetActive(false);
            }
            finally
            {
                _isTransitioning = false;
            }
        }

        private async UniTask FadeInAsync(CancellationToken cancellationToken)
        {
            _canvasGroup.alpha = 0f;
            float elapsed = 0f;

            while (elapsed < _fadeInDuration && !cancellationToken.IsCancellationRequested)
            {
                var progress = elapsed / _fadeInDuration;
                var curveValue = _fadeInCurve.Evaluate(progress);
                _canvasGroup.alpha = curveValue;

                await UniTask.Yield(cancellationToken: cancellationToken);
                elapsed += _ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                _canvasGroup.alpha = 1f;
            }
        }

        private async UniTask FadeOutAsync(CancellationToken cancellationToken)
        {
            float elapsed = 0f;

            while (elapsed < _fadeOutDuration && !cancellationToken.IsCancellationRequested)
            {
                var progress = elapsed / _fadeOutDuration;
                var curveValue = _fadeOutCurve.Evaluate(progress);
                _canvasGroup.alpha = curveValue;

                await UniTask.Yield(cancellationToken: cancellationToken);
                elapsed += _ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                _canvasGroup.alpha = 0f;
            }
        }

        /// <summary>
        /// 전환 효과를 취소합니다.
        /// </summary>
        public void CancelTransition()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void OnValidate()
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }
        }
    }
}
