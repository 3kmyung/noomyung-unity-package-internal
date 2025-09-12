using System;
using System.Drawing;
using System.Numerics;
using Noomyung.UI.Domain.Enums;

namespace Noomyung.UI.Domain.ValueObjects
{
    /// <summary>
    /// 개별 효과를 나타내는 불변 값 객체입니다.
    /// 타입 안전한 접근을 위해 제네릭을 활용합니다.
    /// </summary>
    public readonly struct Effect : IEquatable<Effect>
    {
        /// <summary>효과 유형</summary>
        public EffectType Type { get; }

        /// <summary>타이밍 정보</summary>
        public EffectTiming Timing { get; }

        /// <summary>이징 정보</summary>
        public EffectEasing Easing { get; }

        /// <summary>효과별 특화된 데이터</summary>
        public IEffectData Data { get; }

        public Effect(
            EffectType type,
            EffectTiming timing,
            EffectEasing easing,
            IEffectData data)
        {
            Type = type;
            Timing = timing;
            Easing = easing;
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        /// <summary>
        /// 타입 안전한 방식으로 효과 데이터를 가져옵니다.
        /// </summary>
        /// <typeparam name="T">효과 데이터 타입</typeparam>
        /// <returns>타입이 일치하면 데이터를 반환, 그렇지 않으면 기본값</returns>
        public T GetData<T>() where T : struct, IEffectData
        {
            if (Data is T data)
                return data;

            return default(T);
        }

        /// <summary>
        /// 타입 안전한 방식으로 효과 데이터를 가져옵니다.
        /// </summary>
        /// <typeparam name="T">효과 데이터 타입</typeparam>
        /// <param name="data">반환될 데이터</param>
        /// <returns>타입이 일치하면 true, 그렇지 않으면 false</returns>
        public bool TryGetData<T>(out T data) where T : struct, IEffectData
        {
            if (Data is T typedData)
            {
                data = typedData;
                return true;
            }

            data = default(T);
            return false;
        }

        public bool Equals(Effect other) =>
            Type == other.Type &&
            Timing.Equals(other.Timing) &&
            Easing.Equals(other.Easing) &&
            Equals(Data, other.Data);

        public override bool Equals(object obj) => obj is Effect other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Type, Timing, Easing, Data);

        public static bool operator ==(Effect left, Effect right) => left.Equals(right);

        public static bool operator !=(Effect left, Effect right) => !left.Equals(right);

        public override string ToString() => $"Type: {Type}, Timing: {Timing}, Easing: {Easing}, Data: {Data?.GetType().Name ?? "null"}";
    }
}
