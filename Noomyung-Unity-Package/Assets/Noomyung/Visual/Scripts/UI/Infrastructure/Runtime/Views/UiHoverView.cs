using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;
using Noomyung.UI.Application.Ports;

namespace Noomyung.UI.Infrastructure.Runtime.Views
{
    /// <summary>
    /// UI 요소의 호버 상호작용을 관리하는 컴포넌트입니다.
    /// 마우스 진입/벗어남 효과를 담당합니다.
    /// </summary>
    public class UiHoverView : MonoBehaviour, IUiHoverView, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Hover Settings")]
        [SerializeField] private float _hoverScale = 1.1f;
        [SerializeField] private float _hoverDuration = 0.2f;
        [SerializeField] private AnimationCurve _hoverCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private bool _ignoreTimeScale = true;

        [Header("Components")]
        [SerializeField] private RectTransform _targetTransform;

        private Vector3 _originalScale;
        private bool _isHovered;
        private bool _isTransitioning;
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// 현재 호버 상태인지 여부를 확인합니다.
        /// </summary>
        public bool IsHovered => _isHovered;

        private void Awake()
        {
            if (_targetTransform == null)
            {
                _targetTransform = GetComponent<RectTransform>();
            }

            _originalScale = _targetTransform.localScale;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        /// <summary>
        /// 호버 진입 효과를 재생합니다.
        /// </summary>
        /// <param name="cancellationToken">취소 토큰</param>
        /// <returns>비동기 작업</returns>
        public async Task HoverEnterAsync(CancellationToken cancellationToken = default)
        {
            if (_isHovered || _isTransitioning) return;

            _isHovered = true;
            _isTransitioning = true;

            try
            {
                await ScaleToAsync(_originalScale * _hoverScale, _hoverDuration, cancellationToken);
            }
            finally
            {
                _isTransitioning = false;
            }
        }

        /// <summary>
        /// 호버 벗어남 효과를 재생합니다.
        /// </summary>
        /// <param name="cancellationToken">취소 토큰</param>
        /// <returns>비동기 작업</returns>
        public async Task HoverExitAsync(CancellationToken cancellationToken = default)
        {
            if (!_isHovered || _isTransitioning) return;

            _isHovered = false;
            _isTransitioning = true;

            try
            {
                await ScaleToAsync(_originalScale, _hoverDuration, cancellationToken);
            }
            finally
            {
                _isTransitioning = false;
            }
        }

        private async UniTask ScaleToAsync(Vector3 targetScale, float duration, CancellationToken cancellationToken)
        {
            Vector3 startScale = _targetTransform.localScale;
            float elapsed = 0f;

            while (elapsed < duration && !cancellationToken.IsCancellationRequested)
            {
                var progress = elapsed / duration;
                var curveValue = _hoverCurve.Evaluate(progress);
                _targetTransform.localScale = Vector3.Lerp(startScale, targetScale, curveValue);

                await UniTask.Yield(cancellationToken: cancellationToken);
                elapsed += _ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                _targetTransform.localScale = targetScale;
            }
        }

        /// <summary>
        /// Unity EventSystem의 포인터 진입 이벤트를 처리합니다.
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (enabled && gameObject.activeInHierarchy)
            {
                _ = HoverEnterAsync(_cancellationTokenSource.Token);
            }
        }

        /// <summary>
        /// Unity EventSystem의 포인터 벗어남 이벤트를 처리합니다.
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            if (enabled && gameObject.activeInHierarchy)
            {
                _ = HoverExitAsync(_cancellationTokenSource.Token);
            }
        }

        /// <summary>
        /// 호버 효과를 취소합니다.
        /// </summary>
        public void CancelHover()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();

            _isHovered = false;
            _isTransitioning = false;
            _targetTransform.localScale = _originalScale;
        }

        private void OnValidate()
        {
            if (_targetTransform == null)
            {
                _targetTransform = GetComponent<RectTransform>();
            }
        }
    }
}
