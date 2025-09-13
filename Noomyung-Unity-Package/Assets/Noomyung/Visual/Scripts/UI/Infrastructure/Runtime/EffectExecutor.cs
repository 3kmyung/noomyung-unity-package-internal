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

            // CycleCount 기반 반복 처리
            await ExecuteCyclesAsync(target, effect, cancellationToken);
        }

        private async UniTask ExecuteCyclesAsync(IUIElementHandle target, IEffect effect, CancellationToken cancellationToken)
        {
            var timing = effect.Timing;

            // RepeatMode에 따른 처리
            switch (timing.RepeatMode)
            {
                case RepeatMode.Once:
                    await ExecuteSingleCycle(target, effect, false, cancellationToken);
                    break;

                case RepeatMode.Finite:
                    await ExecuteFiniteCycles(target, effect, timing.CycleCount, cancellationToken);
                    break;

                case RepeatMode.Loop:
                    await ExecuteInfiniteCycles(target, effect, cancellationToken);
                    break;
            }
        }

        private async UniTask ExecuteSingleCycle(IUIElementHandle target, IEffect effect, bool reverse, CancellationToken cancellationToken)
        {
            await effect.ExecuteAsync(target, reverse, cancellationToken);
        }

        private async UniTask ExecuteFiniteCycles(IUIElementHandle target, IEffect effect, int cycleCount, CancellationToken cancellationToken)
        {
            for (int i = 0; i < cycleCount && !cancellationToken.IsCancellationRequested; i++)
            {
                bool reverse = ShouldReverse(effect.Timing.Direction, i);
                await ExecuteSingleCycle(target, effect, reverse, cancellationToken);
            }
        }

        private async UniTask ExecuteInfiniteCycles(IUIElementHandle target, IEffect effect, CancellationToken cancellationToken)
        {
            int cycleIndex = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                bool reverse = ShouldReverse(effect.Timing.Direction, cycleIndex);
                await ExecuteSingleCycle(target, effect, reverse, cancellationToken);
                cycleIndex++;
            }
        }

        private bool ShouldReverse(PlaybackDirection direction, int cycleIndex)
        {
            if (direction == PlaybackDirection.Forward)
                return false;

            // PingPong: 홀수 인덱스에서 역방향
            return direction == PlaybackDirection.PingPong && cycleIndex % 2 == 1;
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
