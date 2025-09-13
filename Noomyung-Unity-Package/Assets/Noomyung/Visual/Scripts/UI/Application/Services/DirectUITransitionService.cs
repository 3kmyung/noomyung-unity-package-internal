using System;
using System.Threading;
using System.Threading.Tasks;
using Noomyung.UI.Application.Ports;
using Noomyung.UI.Domain.Enums;

namespace Noomyung.UI.Application.Services
{
    /// <summary>
    /// 직접 전환 에셋을 사용하는 UI 전환 서비스입니다.
    /// </summary>
    public class DirectUITransitionService : IUITransitionService
    {
        private readonly IUITransitionRunner _transitionRunner;
        private readonly IUITransitionDefinition _transitionAsset;

        /// <summary>
        /// DirectUITransitionService의 새 인스턴스를 초기화합니다.
        /// </summary>
        /// <param name="transitionRunner">전환 실행기</param>
        /// <param name="transitionAsset">사용할 전환 에셋</param>
        public DirectUITransitionService(
            IUITransitionRunner transitionRunner,
            IUITransitionDefinition transitionAsset)
        {
            _transitionRunner = transitionRunner ?? throw new ArgumentNullException(nameof(transitionRunner));
            _transitionAsset = transitionAsset;
        }

        /// <inheritdoc />
        public async Task ShowAsync(IUIElementHandle target, CancellationToken cancellationToken = default)
        {
            await ExecuteTransitionAsync(target, EffectTrigger.Show, cancellationToken);
        }

        /// <inheritdoc />
        public async Task HideAsync(IUIElementHandle target, CancellationToken cancellationToken = default)
        {
            await ExecuteTransitionAsync(target, EffectTrigger.Hide, cancellationToken);
        }

        /// <inheritdoc />
        public async Task HoverEnterAsync(IUIElementHandle target, CancellationToken cancellationToken = default)
        {
            await ExecuteTransitionAsync(target, EffectTrigger.HoverEnter, cancellationToken);
        }

        /// <inheritdoc />
        public async Task HoverExitAsync(IUIElementHandle target, CancellationToken cancellationToken = default)
        {
            await ExecuteTransitionAsync(target, EffectTrigger.HoverExit, cancellationToken);
        }

        private async Task ExecuteTransitionAsync(IUIElementHandle target, EffectTrigger trigger, CancellationToken cancellationToken)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (_transitionAsset == null)
                return; // 전환 에셋이 없으면 아무것도 실행하지 않음

            var transitionSet = _transitionAsset.ToDomain();
            var transition = transitionSet.GetTransition(trigger);

            if (transition.HasValue && !transition.Value.IsEmpty)
            {
                await _transitionRunner.RunAsync(target, transition.Value, cancellationToken);
            }
        }
    }
}
