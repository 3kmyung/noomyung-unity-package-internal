using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Noomyung.UI.Application.Ports;
using Noomyung.UI.Domain.ValueObjects;
using Noomyung.UI.Domain.ValueObjects.Effects;
using Noomyung.UI.Domain.Enums;

namespace Noomyung.UI.Infrastructure.Runtime
{
    /// <summary>
    /// 효과를 처리하고 실행하는 실행기입니다.
    /// </summary>
    public class EffectExecutor : IUIEffectExecutor
    {
        private readonly bool _ignoreTimeScale;

        /// <summary>
        /// EffectExecutor의 새 인스턴스를 초기화합니다.
        /// </summary>
        /// <param name="ignoreTimeScale">시간 스케일 무시 여부</param>
        public EffectExecutor(bool ignoreTimeScale = true)
        {
            _ignoreTimeScale = ignoreTimeScale;
        }

        /// <inheritdoc />
        public async UniTask ExecuteAsync(IUIElementHandle target, IEffect effect, CancellationToken cancellationToken = default)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (effect == null)
                throw new ArgumentNullException(nameof(effect));

            // 지연 시간 처리
            if (effect.Timing.Delay > 0f)
            {
                await DelayAsync(effect.Timing.Delay, cancellationToken);
            }

            // 기본 실행
            await ExecuteEffectAsync(target, effect, false, cancellationToken);

            // 반복 처리
            for (int i = 0; i < effect.Timing.Loops && !cancellationToken.IsCancellationRequested; i++)
            {
                if (effect.Timing.LoopType == LoopType.Yoyo && i % 2 == 1)
                {
                    // Yoyo 효과를 위해 역방향 실행
                    await ExecuteEffectAsync(target, effect, true, cancellationToken);
                }
                else
                {
                    await ExecuteEffectAsync(target, effect, false, cancellationToken);
                }
            }
        }

        private async UniTask ExecuteEffectAsync(IUIElementHandle target, IEffect effect, bool reverse, CancellationToken cancellationToken)
        {
            await effect.ExecuteAsync(target, reverse, cancellationToken);
        }

        private async UniTask DelayAsync(float delay, CancellationToken cancellationToken)
        {
            float elapsed = 0f;
            while (elapsed < delay && !cancellationToken.IsCancellationRequested)
            {
                await UniTask.Yield(cancellationToken: cancellationToken);
                elapsed += _ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            }
        }
    }
}
