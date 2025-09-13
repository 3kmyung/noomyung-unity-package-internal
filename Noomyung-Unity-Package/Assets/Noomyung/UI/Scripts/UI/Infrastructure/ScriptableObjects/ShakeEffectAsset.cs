using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using Noomyung.UI.Domain.Enums;
using Noomyung.UI.Domain.ValueObjects;
using Noomyung.UI.Domain.ValueObjects.Effects;

namespace Noomyung.UI.Infrastructure.ScriptableObjects
{
    /// <summary>
    /// 흔들림 효과 에셋입니다.
    /// </summary>
    public class ShakeEffectAsset : UIEffectAsset
    {
        [Header("흔들림 설정")]
        [SerializeField, Min(0f)] private float amplitude = 10f;
        [SerializeField, Min(0f)] private float frequency = 10f;
        [SerializeField] private bool useDurationOverride = false;
        [SerializeField, Min(0f)] private float durationOverride = 0.5f;

        /// <inheritdoc />
        public override EffectType EffectType => EffectType.Shake;

        /// <summary>진폭</summary>
        public float Amplitude => amplitude;

        /// <summary>주파수</summary>
        public float Frequency => frequency;

        /// <summary>지속시간 오버라이드 사용 여부</summary>
        public bool UseDurationOverride => useDurationOverride;

        /// <summary>오버라이드 지속시간</summary>
        public float DurationOverride => durationOverride;

        /// <inheritdoc />
        public override IReadOnlyDictionary<string, object> GetPayload()
        {
            return new Dictionary<string, object>
            {
                { "Amplitude", amplitude },
                { "Frequency", frequency },
                { "UseDurationOverride", useDurationOverride },
                { "DurationOverride", durationOverride }
            };
        }

        /// <summary>
        /// 새로운 Effect 구조에 맞는 Effect를 생성합니다.
        /// </summary>
        public ShakeEffect CreateEffect()
        {
            var strength = new Vector3(amplitude, amplitude, 0);
            var duration = useDurationOverride ? durationOverride : 1f;

            return new ShakeEffect(
                strength: strength,
                timing: new EffectTiming(duration: duration, delay: 0f, repeat: 1),
                easing: new EffectEasing(EasingType.Linear),
                frequency: frequency,
                vibrato: 10
            );
        }

        /// <inheritdoc />
        protected override void OnValidate()
        {
            base.OnValidate();

            amplitude = Mathf.Max(0f, amplitude);
            frequency = Mathf.Max(0f, frequency);
            durationOverride = Mathf.Max(0f, durationOverride);
        }
    }
}
