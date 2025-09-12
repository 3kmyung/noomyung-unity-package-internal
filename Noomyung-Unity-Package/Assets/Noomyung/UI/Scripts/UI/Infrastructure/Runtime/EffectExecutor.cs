using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Noomyung.UI.Application.Ports;
using Noomyung.UI.Domain.ValueObjects;
using Noomyung.UI.Domain.Enums;
using Noomyung.UI.Infrastructure.Runtime.EffectPorts;

namespace Noomyung.UI.Infrastructure.Runtime
{
    /// <summary>
    /// 효과를 처리하고 실행하는 실행기입니다.
    /// </summary>
    public class EffectExecutor : IUIEffectExecutor
    {
        private readonly bool _ignoreTimeScale;
        private readonly Dictionary<EffectType, IEffectPort> _effectPorts;

        /// <summary>
        /// EffectExecutor의 새 인스턴스를 초기화합니다.
        /// </summary>
        /// <param name="ignoreTimeScale">시간 스케일 무시 여부</param>
        public EffectExecutor(bool ignoreTimeScale = true)
        {
            _ignoreTimeScale = ignoreTimeScale;
            _effectPorts = InitializeEffectPorts();
        }

        /// <inheritdoc />
        public async UniTask ExecuteAsync(IUIElementHandle target, Effect effect, CancellationToken cancellationToken = default)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            // 지연 시간 처리
            if (effect.Timing.Delay > 0f)
            {
                await DelayAsync(effect.Timing.Delay, cancellationToken);
            }

            // 기본 실행
            await ExecuteEffectAsync(target, effect, cancellationToken);

            // 반복 처리
            for (int i = 0; i < effect.Timing.Loops && !cancellationToken.IsCancellationRequested; i++)
            {
                if (effect.Timing.LoopType == LoopType.Yoyo && i % 2 == 1)
                {
                    // Yoyo 효과를 위해 역방향 실행
                    await ExecuteReverseEffectAsync(target, effect, cancellationToken);
                }
                else
                {
                    await ExecuteEffectAsync(target, effect, cancellationToken);
                }
            }
        }

        private async UniTask ExecuteEffectAsync(IUIElementHandle target, Effect effect, CancellationToken cancellationToken)
        {
            if (_effectPorts.TryGetValue(effect.Type, out var effectPort))
            {
                await effectPort.ExecuteAsync(target, effect, false, cancellationToken);
            }
        }

        private async UniTask ExecuteReverseEffectAsync(IUIElementHandle target, Effect effect, CancellationToken cancellationToken)
        {
            if (_effectPorts.TryGetValue(effect.Type, out var effectPort))
            {
                await effectPort.ExecuteAsync(target, effect, true, cancellationToken);
            }
        }


        private Dictionary<EffectType, IEffectPort> InitializeEffectPorts()
        {
            return new Dictionary<EffectType, IEffectPort>
            {
                { EffectType.Fade, new FadeEffect(_ignoreTimeScale) },
                { EffectType.Scale, new ScaleEffect(_ignoreTimeScale) },
                { EffectType.Move, new MoveEffect(_ignoreTimeScale) },
                { EffectType.Color, new ColorEffect(_ignoreTimeScale) },
                { EffectType.MaterialFloat, new MaterialFloatEffect(_ignoreTimeScale) },
                { EffectType.MaterialColor, new MaterialColorEffect(_ignoreTimeScale) },
                { EffectType.Shake, new ShakeEffect(_ignoreTimeScale) }
            };
        }
    }
}
