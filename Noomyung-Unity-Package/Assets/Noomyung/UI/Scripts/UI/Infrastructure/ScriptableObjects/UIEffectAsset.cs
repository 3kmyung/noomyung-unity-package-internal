using System;
using System.Collections.Generic;
using UnityEngine;
using Noomyung.UI.Domain.Enums;
using Noomyung.UI.Domain.ValueObjects;
using Noomyung.UI.Application.Ports;

namespace Noomyung.UI.Infrastructure.ScriptableObjects
{
    /// <summary>
    /// UI 효과를 정의하는 추상 ScriptableObject 베이스 클래스입니다.
    /// </summary>
    public abstract class UIEffectAsset : ScriptableObject, IUIEffectDefinition
    {
        [Header("타이밍 설정")]
        [SerializeField, Min(0f)] private float duration = 1f;
        [SerializeField, Min(0f)] private float delay = 0f;
        [SerializeField, Min(-1)] private int loops = 0;
        [SerializeField] private LoopType loopType = LoopType.None;

        [Header("이징 설정")]
        [SerializeField] private EasingType easingType = EasingType.Linear;
        [SerializeField] private AnimationCurve customCurve = AnimationCurve.Linear(0, 0, 1, 1);

        /// <summary>효과 유형</summary>
        public abstract EffectType EffectType { get; }

        /// <summary>지속 시간</summary>
        public float Duration => duration;

        /// <summary>지연 시간</summary>
        public float Delay => delay;

        /// <summary>반복 횟수</summary>
        public int Loops => loops;

        /// <summary>반복 유형</summary>
        public LoopType LoopType => loopType;

        /// <summary>이징 유형</summary>
        public EasingType EasingType => easingType;

        /// <summary>사용자 정의 커브</summary>
        public AnimationCurve CustomCurve => customCurve;

        /// <summary>Custom animation curve data for the effect.</summary>
        public IReadOnlyList<KeyValuePair<float, float>> CustomCurveData
        {
            get
            {
                var curveData = new List<KeyValuePair<float, float>>();
                if (customCurve != null && customCurve.keys.Length > 0)
                {
                    foreach (var key in customCurve.keys)
                    {
                        curveData.Add(new KeyValuePair<float, float>(key.time, key.value));
                    }
                }
                return curveData;
            }
        }

        /// <summary>
        /// Converts this effect definition to a domain EffectStep.
        /// </summary>
        /// <returns>Domain effect step</returns>
        public virtual EffectStep ToDomain()
        {
            var timing = new EffectTiming(duration, delay, loops, loopType);
            var easing = new EffectEasing(easingType, easingType == EasingType.CustomCurve ? customCurve : null);
            var payload = GetPayload();
            return new EffectStep(EffectType, timing, easing, payload);
        }

        /// <summary>
        /// 효과별 매개변수를 도메인 딕셔너리로 변환합니다.
        /// </summary>
        /// <returns>효과 매개변수 딕셔너리</returns>
        public abstract IReadOnlyDictionary<string, object> GetPayload();


        /// <summary>
        /// Inspector에서 설정 유효성을 검증합니다.
        /// </summary>
        protected virtual void OnValidate()
        {
            duration = Mathf.Max(0f, duration);
            delay = Mathf.Max(0f, delay);
            loops = Mathf.Max(-1, loops);

            if (customCurve == null)
                customCurve = AnimationCurve.Linear(0, 0, 1, 1);
        }
    }
}
