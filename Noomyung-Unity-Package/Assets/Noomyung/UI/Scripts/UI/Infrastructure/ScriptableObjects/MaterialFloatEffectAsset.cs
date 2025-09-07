using System.Collections.Generic;
using UnityEngine;
using Noomyung.UI.Domain.Enums;

namespace Noomyung.UI.Infrastructure.ScriptableObjects
{
    /// <summary>
    /// 머티리얼 float 속성 효과 에셋입니다.
    /// </summary>
    public class MaterialFloatEffectAsset : UIEffectAsset
    {
        [Header("머티리얼 Float 설정")]
        [SerializeField] private string propertyName = "_Alpha";
        [SerializeField] private float fromValue = 0f;
        [SerializeField] private float toValue = 1f;

        /// <inheritdoc />
        public override EffectType EffectType => EffectType.MaterialFloat;

        /// <summary>속성 이름</summary>
        public string PropertyName => propertyName;

        /// <summary>시작 값</summary>
        public float FromValue => fromValue;

        /// <summary>끝 값</summary>
        public float ToValue => toValue;

        /// <inheritdoc />
        public override IReadOnlyDictionary<string, object> GetPayload()
        {
            return new Dictionary<string, object>
            {
                { "PropertyName", propertyName },
                { "From", fromValue },
                { "To", toValue }
            };
        }

        /// <inheritdoc />
        protected override void OnValidate()
        {
            base.OnValidate();

            if (string.IsNullOrWhiteSpace(propertyName))
                propertyName = "_Alpha";
        }
    }
}
