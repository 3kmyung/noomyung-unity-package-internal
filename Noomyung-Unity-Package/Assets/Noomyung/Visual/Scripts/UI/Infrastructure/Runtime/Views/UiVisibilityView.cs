using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Noomyung.UI.Application.Ports;

namespace Noomyung.UI.Infrastructure.Runtime.Views
{
    /// <summary>
    /// UI 요소의 가시성을 관리하는 컴포넌트입니다.
    /// GameObject의 active/inactive 상태를 제어하며, UiTransitionView가 연결되어 있으면 전환 효과를 사용합니다.
    /// </summary>
    public class UiVisibilityView : MonoBehaviour, IUiVisibilityView
    {
        [Header("Settings")]
        [SerializeField] private bool _startVisible = true;

        private IUiTransitionView _transitionView;
        private bool _isVisible;

        /// <summary>
        /// UI 요소가 현재 표시되어 있는지 여부를 확인합니다.
        /// </summary>
        public bool IsVisible => _isVisible;

        private void Awake()
        {
            // 같은 GameObject에서 UiTransitionView 컴포넌트를 찾습니다.
            _transitionView = GetComponent<IUiTransitionView>();

            // 초기 가시성 설정
            _isVisible = _startVisible;
            gameObject.SetActive(_isVisible);
        }

        /// <summary>
        /// UI 요소를 표시합니다.
        /// UiTransitionView가 연결되어 있으면 전환 효과를 사용하고, 없으면 즉시 활성화합니다.
        /// </summary>
        /// <param name="cancellationToken">취소 토큰</param>
        /// <returns>비동기 작업</returns>
        public async Task ShowAsync(CancellationToken cancellationToken = default)
        {
            if (_isVisible) return;

            _isVisible = true;

            if (_transitionView != null)
            {
                // 전환 효과가 있으면 전환 효과를 사용
                await _transitionView.PlayShowAsync(cancellationToken);
            }
            else
            {
                // 전환 효과가 없으면 즉시 활성화
                gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// UI 요소를 숨깁니다.
        /// UiTransitionView가 연결되어 있으면 전환 효과를 사용하고, 없으면 즉시 비활성화합니다.
        /// </summary>
        /// <param name="cancellationToken">취소 토큰</param>
        /// <returns>비동기 작업</returns>
        public async Task HideAsync(CancellationToken cancellationToken = default)
        {
            if (!_isVisible) return;

            _isVisible = false;

            if (_transitionView != null)
            {
                // 전환 효과가 있으면 전환 효과를 사용
                await _transitionView.PlayHideAsync(cancellationToken);
            }
            else
            {
                // 전환 효과가 없으면 즉시 비활성화
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 즉시 가시성을 토글합니다 (전환 효과 없이).
        /// </summary>
        public void ToggleImmediate()
        {
            _isVisible = !_isVisible;
            gameObject.SetActive(_isVisible);
        }

        /// <summary>
        /// 즉시 표시합니다 (전환 효과 없이).
        /// </summary>
        public void ShowImmediate()
        {
            _isVisible = true;
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 즉시 숨깁니다 (전환 효과 없이).
        /// </summary>
        public void HideImmediate()
        {
            _isVisible = false;
            gameObject.SetActive(false);
        }
    }
}
