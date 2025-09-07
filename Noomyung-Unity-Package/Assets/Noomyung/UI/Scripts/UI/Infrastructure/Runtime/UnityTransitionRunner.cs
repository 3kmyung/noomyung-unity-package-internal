using System;
using System.Threading;
using System.Threading.Tasks;
using Noomyung.UI.Domain.Interfaces;
using Noomyung.UI.Domain.ValueObjects;

namespace Noomyung.UI.Infrastructure.Runtime
{
    /// <summary>
    /// Unity 기반 전환 실행기입니다.
    /// </summary>
    public class UnityTransitionRunner : IUITransitionRunner
    {
        private readonly IUIEffectStepExecutor _stepExecutor;

        /// <summary>
        /// UnityTransitionRunner의 새 인스턴스를 초기화합니다.
        /// </summary>
        /// <param name="stepExecutor">효과 단계 실행기</param>
        public UnityTransitionRunner(IUIEffectStepExecutor stepExecutor)
        {
            _stepExecutor = stepExecutor ?? throw new ArgumentNullException(nameof(stepExecutor));
        }

        /// <inheritdoc />
        public async Task RunAsync(IUIElementHandle target, TransitionDefinition transition, CancellationToken cancellationToken = default)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (transition.IsEmpty)
                return;

            // 모든 단계를 순차적으로 실행
            foreach (var step in transition.Steps)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                try
                {
                    await _stepExecutor.ExecuteAsync(target, step, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // 취소 요청은 정상적인 종료로 처리
                    break;
                }
                catch (Exception ex)
                {
                    // 개별 단계 실행 실패 시 로그 출력 후 계속 진행
                    UnityEngine.Debug.LogError($"Effect step execution failed: {ex.Message}");
                }
            }
        }
    }
}
