using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using _3kmyung.Effect.Application.Ports;
using _3kmyung.Effect.Domain.ValueObjects;
using _3kmyung.Effect.Domain.Enums;

namespace _3kmyung.Effect.Infrastructure.Runtime
{
    /// <summary>
    /// Executes UI effects with cycle-based repetition semantics.
    /// </summary>
    public class EffectExecutor : IEffectExecutor
    {
        private readonly bool _ignoreTimeScale;

        public EffectExecutor(bool ignoreTimeScale = true)
        {
            _ignoreTimeScale = ignoreTimeScale;
        }

        public async UniTask ExecuteAsync(IEffectElementPort target, IEffect effect, CancellationToken cancellationToken = default)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (effect == null)
                throw new ArgumentNullException(nameof(effect));

            if (effect.Timing.Delay > 0f)
            {
                await DelayAsync(effect.Timing.Delay, cancellationToken);
            }

            await ExecuteCyclesAsync(target, effect, cancellationToken);
        }

        private async UniTask ExecuteCyclesAsync(IEffectElementPort target, IEffect effect, CancellationToken cancellationToken)
        {
            var timing = effect.Timing;

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

        private async UniTask ExecuteSingleCycle(IEffectElementPort target, IEffect effect, bool reverse, CancellationToken cancellationToken)
        {
            await effect.ExecuteAsync(target, reverse, cancellationToken);
        }

        private async UniTask ExecuteFiniteCycles(IEffectElementPort target, IEffect effect, int cycleCount, CancellationToken cancellationToken)
        {
            for (int i = 0; i < cycleCount && !cancellationToken.IsCancellationRequested; i++)
            {
                bool reverse = ShouldReverse(effect.Timing.Direction, i);
                await ExecuteSingleCycle(target, effect, reverse, cancellationToken);
            }
        }

        private async UniTask ExecuteInfiniteCycles(IEffectElementPort target, IEffect effect, CancellationToken cancellationToken)
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
