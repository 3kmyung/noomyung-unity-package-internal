using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using Noomyung.UI.Domain.Enums;

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
    }
}
