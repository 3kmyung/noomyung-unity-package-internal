using System;
using System.Threading;
using System.Threading.Tasks;
using Noomyung.UI.Application.Interfaces;
using Noomyung.UI.Domain.Enums;
using Noomyung.UI.Domain.Interfaces;

namespace Noomyung.UI.Application.Services
{
    /// <summary>
    /// UI 전환 서비스의 구현체입니다.
    /// </summary>
    public class UITransitionService : IUITransitionService
    {
        private readonly IUITransitionRunner _transitionRunner;
        private readonly IUITransitionRepository _transitionRepository;
        private readonly string _transitionId;

        /// <summary>
        /// UITransitionService의 새 인스턴스를 초기화합니다.
        /// </summary>
        /// <param name="transitionRunner">전환 실행기</param>
        /// <param name="transitionRepository">전환 리포지토리</param>
        /// <param name="transitionId">사용할 전환 설정 ID</param>
        public UITransitionService(
            IUITransitionRunner transitionRunner, 
            IUITransitionRepository transitionRepository,
            string transitionId)
        {
            _transitionRunner = transitionRunner ?? throw new ArgumentNullException(nameof(transitionRunner));
            _transitionRepository = transitionRepository ?? throw new ArgumentNullException(nameof(transitionRepository));
            _transitionId = transitionId ?? throw new ArgumentNullException(nameof(transitionId));
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

            var transitionSet = _transitionRepository.GetFor(_transitionId);
            var transition = transitionSet.GetTransition(trigger);

            if (transition.HasValue && !transition.Value.IsEmpty)
            {
                await _transitionRunner.RunAsync(target, transition.Value, cancellationToken);
            }
        }
    }
}
