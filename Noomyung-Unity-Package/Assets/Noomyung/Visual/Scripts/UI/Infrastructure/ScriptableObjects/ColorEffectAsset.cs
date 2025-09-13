using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Noomyung.UI.Domain.Enums;
using Noomyung.UI.Domain.ValueObjects;
using Noomyung.UI.Domain.ValueObjects.Effects;

namespace Noomyung.UI.Infrastructure.ScriptableObjects
{
    /// <summary>
    /// 색상 효과의 대상 모드를 정의합니다.
    /// </summary>
    public enum GraphicTargetMode
    {
        /// <summary>단일 그래픽 컴포넌트</summary>
        Graphic,

        /// <summary>자식 포함 모든 그래픽 컴포넌트</summary>
        AllGraphicsInChildren
    }

    /// <summary>
    /// 색상 효과 에셋입니다.
    /// </summary>
    public class ColorEffectAsset : UIEffectAsset
    {
        [Header("색상 설정")]
        [SerializeField] private GraphicTargetMode targetMode = GraphicTargetMode.Graphic;
        [SerializeField] private Color fromColor = Color.white;
        [SerializeField] private Color toColor = Color.white;

        /// <inheritdoc />
        public override EffectType EffectType => EffectType.Color;

        /// <summary>대상 모드</summary>
        public GraphicTargetMode TargetMode => targetMode;

        /// <summary>시작 색상</summary>
        public Color FromColor => fromColor;

        /// <summary>끝 색상</summary>
        public Color ToColor => toColor;

        /// <inheritdoc />
        public override IReadOnlyDictionary<string, object> GetPayload()
        {
            return new Dictionary<string, object>
            {
                { "TargetMode", targetMode.ToString() },
                { "From", Color.FromArgb((int)(fromColor.a * 255), (int)(fromColor.r * 255), (int)(fromColor.g * 255), (int)(fromColor.b * 255)) },
                { "To", Color.FromArgb((int)(toColor.a * 255), (int)(toColor.r * 255), (int)(toColor.g * 255), (int)(toColor.b * 255)) }
            };
        }

        /// <summary>
        /// 새로운 Effect 구조에 맞는 Effect를 생성합니다.
        /// </summary>
        public ColorEffect CreateEffect()
        {
            return new ColorEffect(
                from: Color.FromArgb((int)(fromColor.a * 255), (int)(fromColor.r * 255), (int)(fromColor.g * 255), (int)(fromColor.b * 255)),
                to: Color.FromArgb((int)(toColor.a * 255), (int)(toColor.r * 255), (int)(toColor.g * 255), (int)(toColor.b * 255)),
                timing: new EffectTiming(duration: 1f, delay: 0f),
                easing: new EffectEasing(EasingType.EaseInOut),
                targetMode: targetMode.ToString()
            );
        }
    }
}
