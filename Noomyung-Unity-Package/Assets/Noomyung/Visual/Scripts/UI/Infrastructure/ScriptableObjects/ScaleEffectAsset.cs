using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using Noomyung.UI.Domain.Enums;
using Noomyung.UI.Domain.ValueObjects;
using Noomyung.UI.Domain.ValueObjects.Effects;

namespace Noomyung.UI.Infrastructure.ScriptableObjects
{
    /// <summary>
    /// 스케일 효과 에셋입니다.
    /// </summary>
    public class ScaleEffectAsset : UIEffectAsset
    {
        [Header("스케일 설정")]
        [SerializeField] private Vector3 fromScale = Vector3.zero;
        [SerializeField] private Vector3 toScale = Vector3.one;
        [SerializeField] private AxisMask axisMask = AxisMask.XYZ;

        /// <inheritdoc />
        public override EffectType EffectType => EffectType.Scale;

        /// <summary>시작 스케일</summary>
        public Vector3 FromScale => fromScale;

        /// <summary>끝 스케일</summary>
        public Vector3 ToScale => toScale;

        /// <summary>적용할 축</summary>
        public AxisMask AxisMask => axisMask;

        /// <inheritdoc />
        public override IReadOnlyDictionary<string, object> GetPayload()
        {
            return new Dictionary<string, object>
            {
                { "From", new Vector3(fromScale.x, fromScale.y, fromScale.z) },
                { "To", new Vector3(toScale.x, toScale.y, toScale.z) },
                { "AxisMask", axisMask }
            };
        }

        /// <summary>
        /// 새로운 Effect 구조에 맞는 Effect를 생성합니다.
        /// </summary>
        public ScaleEffect CreateEffect()
        {
            return new ScaleEffect(
                from: new Vector3(fromScale.x, fromScale.y, fromScale.z),
                to: new Vector3(toScale.x, toScale.y, toScale.z),
                timing: new EffectTiming(duration: 1f, delay: 0f),
                easing: new EffectEasing(EasingType.EaseInOut),
                axisMask: axisMask.ToString()
            );
        }
    }
}
