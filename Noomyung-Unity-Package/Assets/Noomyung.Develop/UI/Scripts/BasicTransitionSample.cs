using System.Threading;
using UnityEngine;
using Noomyung.UI.Infrastructure.Runtime;

#if UNITASK_PRESENT
using Cysharp.Threading.Tasks;
#endif

namespace Noomyung.UI.Samples
{
    /// <summary>
    /// 기본 전환 효과를 시연하는 샘플 스크립트입니다.
    /// </summary>
    public class BasicTransitionSample : MonoBehaviour
    {
        [Header("UI Views")]
        [SerializeField] private UIView fadeView;
        [SerializeField] private UIView scaleView;
        [SerializeField] private UIView moveView;
        [SerializeField] private UIView combinedView;

        [Header("Controls")]
        [SerializeField] private bool showOnStart = true;
        [SerializeField] private float delayBetweenEffects = 0.5f;

        private CancellationTokenSource _cancellationTokenSource;

        private async void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            if (showOnStart)
            {
                await ShowAllViewsSequentially();
            }
        }

        private void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        /// <summary>
        /// 모든 뷰를 순차적으로 표시합니다.
        /// </summary>
        public async System.Threading.Tasks.Task ShowAllViewsSequentially()
        {
            var token = _cancellationTokenSource.Token;

            try
            {
                if (fadeView != null)
                {
                    await fadeView.ShowAsync(token);
                    await DelayAsync(delayBetweenEffects, token);
                }

                if (scaleView != null)
                {
                    await scaleView.ShowAsync(token);
                    await DelayAsync(delayBetweenEffects, token);
                }

                if (moveView != null)
                {
                    await moveView.ShowAsync(token);
                    await DelayAsync(delayBetweenEffects, token);
                }

                if (combinedView != null)
                {
                    await combinedView.ShowAsync(token);
                }
            }
            catch (System.OperationCanceledException)
            {
                Debug.Log("전환 시퀀스가 취소되었습니다.");
            }
        }

        /// <summary>
        /// 모든 뷰를 동시에 표시합니다.
        /// </summary>
        public void ShowAllViewsSimultaneously()
        {
            var token = _cancellationTokenSource.Token;

            _ = fadeView?.ShowAsync(token);
            _ = scaleView?.ShowAsync(token);
            _ = moveView?.ShowAsync(token);
            _ = combinedView?.ShowAsync(token);
        }

        /// <summary>
        /// 모든 뷰를 순차적으로 숨깁니다.
        /// </summary>
        public async System.Threading.Tasks.Task HideAllViewsSequentially()
        {
            var token = _cancellationTokenSource.Token;

            try
            {
                if (combinedView != null)
                {
                    await combinedView.HideAsync(token);
                    await DelayAsync(delayBetweenEffects, token);
                }

                if (moveView != null)
                {
                    await moveView.HideAsync(token);
                    await DelayAsync(delayBetweenEffects, token);
                }

                if (scaleView != null)
                {
                    await scaleView.HideAsync(token);
                    await DelayAsync(delayBetweenEffects, token);
                }

                if (fadeView != null)
                {
                    await fadeView.HideAsync(token);
                }
            }
            catch (System.OperationCanceledException)
            {
                Debug.Log("숨김 시퀀스가 취소되었습니다.");
            }
        }

        /// <summary>
        /// 모든 뷰를 동시에 숨깁니다.
        /// </summary>
        public void HideAllViewsSimultaneously()
        {
            var token = _cancellationTokenSource.Token;

            _ = fadeView?.HideAsync(token);
            _ = scaleView?.HideAsync(token);
            _ = moveView?.HideAsync(token);
            _ = combinedView?.HideAsync(token);
        }

        /// <summary>
        /// 모든 전환을 취소합니다.
        /// </summary>
        public void CancelAllTransitions()
        {
            fadeView?.CancelAllTransitions();
            scaleView?.CancelAllTransitions();
            moveView?.CancelAllTransitions();
            combinedView?.CancelAllTransitions();
        }

        private async System.Threading.Tasks.Task DelayAsync(float seconds, CancellationToken cancellationToken)
        {
#if UNITASK_PRESENT
            await UniTask.Delay(System.TimeSpan.FromSeconds(seconds), cancellationToken: cancellationToken);
#else
            await System.Threading.Tasks.Task.Delay(System.TimeSpan.FromSeconds(seconds), cancellationToken);
#endif
        }

        // UI 버튼용 퍼블릭 메서드들
        public async void OnShowAllSequentiallyButtonClicked() => await ShowAllViewsSequentially();
        public void OnShowAllSimultaneouslyButtonClicked() => ShowAllViewsSimultaneously();
        public async void OnHideAllSequentiallyButtonClicked() => await HideAllViewsSequentially();
        public void OnHideAllSimultaneouslyButtonClicked() => HideAllViewsSimultaneously();
        public void OnCancelAllButtonClicked() => CancelAllTransitions();
    }
}
