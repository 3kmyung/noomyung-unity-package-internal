using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Noomyung.UI.Application.Ports;
using Noomyung.UI.Domain.ValueObjects;

namespace Noomyung.UI.Infrastructure.Runtime
{
    /// <summary>
    /// UI 전환을 실행하는 실행기입니다.
    /// </summary>
    public class TransitionRunner : IUITransitionRunner
    {
        private readonly IUIEffectExecutor _effectExecutor;

        /// <summary>
        /// TransitionRunner의 새 인스턴스를 초기화합니다.
        /// </summary>
        /// <param name="effectExecutor">효과 실행기</param>
        public TransitionRunner(IUIEffectExecutor effectExecutor)
        {
            _effectExecutor = effectExecutor ?? throw new ArgumentNullException(nameof(effectExecutor));
        }

        /// <inheritdoc />
        public async UniTask RunAsync(IUIElementHandle target, TransitionDefinition transition, CancellationToken cancellationToken = default)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (transition.IsEmpty)
                return;

            // 모든 효과를 순차적으로 실행
            foreach (var effect in transition.Effects)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                try
                {
                    await _effectExecutor.ExecuteAsync(target, effect, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // 취소 요청은 정상적인 종료로 처리
                    break;
                }
                catch (Exception ex)
                {
                    // 개별 효과 실행 실패 시 로그 출력 후 계속 진행
                    UnityEngine.Debug.LogError($"Effect execution failed: {ex.Message}");
                }
            }
        }
    }
}
