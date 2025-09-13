using System;
using Noomyung.UI.Domain.Enums;

namespace Noomyung.UI.Domain.ValueObjects
{
    /// <summary>
    /// 효과의 이징 정보를 나타내는 불변 값 객체입니다.
    /// </summary>
    public readonly struct EffectEasing : IEquatable<EffectEasing>
    {
        /// <summary>이징 유형</summary>
        public EasingType Type { get; }
        
        /// <summary>사용자 정의 커브를 위한 핸들 (인프라스트럭처에서 AnimationCurve로 매핑)</summary>
        public object OptionalCurveHandle { get; }

        public EffectEasing(EasingType type, object optionalCurveHandle = null)
        {
            Type = type;
            OptionalCurveHandle = optionalCurveHandle;
        }

        /// <summary>기본 이징 설정 (선형)</summary>
        public static EffectEasing Linear => new(EasingType.Linear);
        
        /// <summary>EaseIn 설정</summary>
        public static EffectEasing EaseIn => new(EasingType.EaseIn);
        
        /// <summary>EaseOut 설정</summary>
        public static EffectEasing EaseOut => new(EasingType.EaseOut);
        
        /// <summary>EaseInOut 설정</summary>
        public static EffectEasing EaseInOut => new(EasingType.EaseInOut);

        public bool Equals(EffectEasing other) =>
            Type == other.Type &&
            Equals(OptionalCurveHandle, other.OptionalCurveHandle);

        public override bool Equals(object obj) => obj is EffectEasing other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Type, OptionalCurveHandle);

        public static bool operator ==(EffectEasing left, EffectEasing right) => left.Equals(right);
        public static bool operator !=(EffectEasing left, EffectEasing right) => !left.Equals(right);

        public override string ToString() => $"Type: {Type}, HasCustomCurve: {OptionalCurveHandle != null}";
    }
}
