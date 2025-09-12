using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Noomyung.UI.Application.Interfaces;
using Noomyung.UI.Application.Services;
using Noomyung.UI.Domain.Interfaces;
using Noomyung.UI.Infrastructure.ScriptableObjects;
using Noomyung.UI.Infrastructure.Async;

using Cysharp.Threading.Tasks;

namespace Noomyung.UI.Infrastructure.Runtime
{
    /// <summary>
    /// Manages UI transition effects for Unity UI elements.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class UIView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        #region Serialized Field

        [Header("Transition")]
        [SerializeField] private UITransitionAsset transitionAsset;

        [Header("Dependincies")]
        [SerializeField] private RectTransform targetRectTransform;
        [SerializeField] private CanvasGroup targetCanvasGroup;
        [SerializeField] private Graphic[] targetGraphics;

        #endregion

        #region Field

        private IUITransitionDefinition _transitionDefinition;
        private IUITransitionService _transitionService;
        private IUIElementHandle _elementHandle;
        private CancellationTokenSource _cancellationTokenSource;

        #endregion

        #region Public API

        /// <summary>ScriptableObject containing transition effect definitions.</summary>
        public IUITransitionDefinition TransitionAsset => transitionAsset;

        /// <summary>
        /// Changes the transition asset at runtime.
        /// </summary>
        /// <param name="definition">New transition asset to use</param>
        public void SetTransitionDefinition(IUITransitionDefinition definition)
        {
            _transitionDefinition = definition;
            InitializeServices();
        }

        /// <summary>
        /// Executes the show transition for this UI element.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the operation</param>
        /// <returns>Task representing the transition completion</returns>
        public async Task ShowAsync(CancellationToken cancellationToken = default)
        {
            EnsureInitialized();
            await _transitionService.ShowAsync(_elementHandle, cancellationToken);
        }

        /// <summary>
        /// Executes the hide transition for this UI element.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the operation</param>
        /// <returns>Task representing the transition completion</returns>
        public async Task HideAsync(CancellationToken cancellationToken = default)
        {
            EnsureInitialized();
            await _transitionService.HideAsync(_elementHandle, cancellationToken);
        }

        /// <summary>
        /// Executes the hover enter transition for this UI element.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the operation</param>
        /// <returns>Task representing the transition completion</returns>
        public async Task HoverEnterAsync(CancellationToken cancellationToken = default)
        {
            EnsureInitialized();
            await _transitionService.HoverEnterAsync(_elementHandle, cancellationToken);
        }

        /// <summary>
        /// Executes the hover exit transition for this UI element.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the operation</param>
        /// <returns>Task representing the transition completion</returns>
        public async Task HoverExitAsync(CancellationToken cancellationToken = default)
        {
            EnsureInitialized();
            await _transitionService.HoverExitAsync(_elementHandle, cancellationToken);
        }

        /// <summary>
        /// Cancels all currently running transitions for this UI element.
        /// </summary>
        public void CancelAllTransitions()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        #endregion

        #region Event Method

        private void Awake()
        {
            InitializeComponents();

            // transitionAsset이 설정되어 있으면 _transitionDefinition으로 초기화
            if (transitionAsset != null)
            {
                _transitionDefinition = transitionAsset;
            }

            InitializeServices();
        }

        private void OnEnable()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void OnDisable()
        {
            CancelAllTransitions();
        }

        private void OnDestroy()
        {
            _cancellationTokenSource?.Dispose();
        }

        private void OnValidate()
        {
            if (targetRectTransform == null)
                targetRectTransform = GetComponent<RectTransform>();

            if (targetCanvasGroup == null)
                targetCanvasGroup = GetComponent<CanvasGroup>();

            if (targetGraphics == null || targetGraphics.Length == 0)
                targetGraphics = GetComponentsInChildren<Graphic>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (enabled && gameObject.activeInHierarchy)
            {
                _ = ExecuteTransitionSafely(() => HoverEnterAsync(GetCancellationToken()));
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (enabled && gameObject.activeInHierarchy)
            {
                _ = ExecuteTransitionSafely(() => HoverExitAsync(GetCancellationToken()));
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
        }

        #endregion

        #region Private and Protected Method

        private void InitializeComponents()
        {
            if (targetRectTransform == null)
                targetRectTransform = GetComponent<RectTransform>();

            if (targetGraphics == null || targetGraphics.Length == 0)
                targetGraphics = GetComponentsInChildren<Graphic>();
        }

        private void InitializeServices()
        {
            var asyncBridge = AsyncBridgeFactory.Create();
            var ignoreTimeScale = _transitionDefinition?.IgnoreTimeScale ?? true;
            var stepExecutor = new UnityEffectStepExecutor(asyncBridge, ignoreTimeScale);
            var transitionRunner = new UnityTransitionRunner(stepExecutor);
            _elementHandle = new UIElementHandle(targetRectTransform, targetCanvasGroup, targetGraphics);

            // 직접 전환 정의를 사용하는 간단한 서비스 생성
            _transitionService = new DirectUITransitionService(transitionRunner, _transitionDefinition);
        }

        private void EnsureInitialized()
        {
            if (_transitionService == null || _elementHandle == null)
            {
                InitializeServices();
            }
        }

        private CancellationToken GetCancellationToken()
        {
            if (_cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();
            }

            return _cancellationTokenSource.Token;
        }

        private async Task ExecuteTransitionSafely(Func<Task> transitionFunc)
        {
            try
            {
                await transitionFunc();
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Debug.LogError($"Transition execution failed on {name}: {ex.Message}", this);
            }
        }

        #endregion

        /// <summary>
        /// Gets a cancellation token that is cancelled when the component is destroyed.
        /// </summary>
        /// <returns>Cancellation token that cancels on component destruction</returns>
        public CancellationToken GetCancellationTokenOnDestroy()
        {
            return GetCancellationToken();
        }
    }
}
