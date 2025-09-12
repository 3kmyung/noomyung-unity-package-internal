using System;
using System.Collections.Generic;
using System.Numerics;
using Noomyung.UI.Domain.Enums;

namespace Noomyung.UI.Domain.ValueObjects
{
    /// <summary>
    /// 개별 효과를 나타내는 불변 값 객체입니다.
    /// </summary>
    public readonly struct Effect : IEquatable<Effect>
    {
        /// <summary>효과 유형</summary>
        public EffectType Type { get; }

        /// <summary>타이밍 정보</summary>
        public EffectTiming Timing { get; }

        /// <summary>이징 정보</summary>
        public EffectEasing Easing { get; }

        /// <summary>효과별 매개변수들</summary>
        public IReadOnlyDictionary<string, object> Payload { get; }

        public Effect(
            EffectType type,
            EffectTiming timing,
            EffectEasing easing,
            IReadOnlyDictionary<string, object> payload = null)
        {
            Type = type;
            Timing = timing;
            Easing = easing;
            Payload = payload ?? new Dictionary<string, object>();
        }

        /// <summary>지정된 키의 float 값을 가져옵니다.</summary>
        public float GetFloat(string key, float defaultValue = 0f)
        {
            if (Payload.TryGetValue(key, out var value))
            {
                return value switch
                {
                    float f => f,
                    double d => (float)d,
                    int i => i,
                    _ => defaultValue
                };
            }
            return defaultValue;
        }

        /// <summary>지정된 키의 bool 값을 가져옵니다.</summary>
        public bool GetBool(string key, bool defaultValue = false)
        {
            return Payload.TryGetValue(key, out var value) && value is bool b ? b : defaultValue;
        }

        /// <summary>지정된 키의 string 값을 가져옵니다.</summary>
        public string GetString(string key, string defaultValue = "")
        {
            return Payload.TryGetValue(key, out var value) && value is string s ? s : defaultValue;
        }

        /// <summary>지정된 키의 Vector3 값을 가져옵니다.</summary>
        public Vector3 GetVector3(string key, Vector3 defaultValue = default)
        {
            return Payload.TryGetValue(key, out var value) && value is Vector3 v ? v : defaultValue;
        }

        /// <summary>지정된 키의 ColorValue 값을 가져옵니다.</summary>
        public ColorValue GetColor(string key, ColorValue defaultValue = default)
        {
            return Payload.TryGetValue(key, out var value) && value is ColorValue c ? c : defaultValue;
        }

        public bool Equals(Effect other) =>
            Type == other.Type &&
            Timing.Equals(other.Timing) &&
            Easing.Equals(other.Easing) &&
            PayloadEquals(Payload, other.Payload);

        private static bool PayloadEquals(IReadOnlyDictionary<string, object> left, IReadOnlyDictionary<string, object> right)
        {
            if (left.Count != right.Count) return false;

            foreach (var kvp in left)
            {
                if (!right.TryGetValue(kvp.Key, out var rightValue) || !Equals(kvp.Value, rightValue))
                    return false;
            }
            return true;
        }

        public override bool Equals(object obj) => obj is Effect other && Equals(other);

        public override int GetHashCode()
        {
            var hash = HashCode.Combine(Type, Timing, Easing);
            foreach (var kvp in Payload)
            {
                hash = HashCode.Combine(hash, kvp.Key, kvp.Value);
            }
            return hash;
        }

        public static bool operator ==(Effect left, Effect right) => left.Equals(right);

        public static bool operator !=(Effect left, Effect right) => !left.Equals(right);

        public override string ToString() => $"Type: {Type}, Timing: {Timing}, Easing: {Easing}, PayloadCount: {Payload.Count}";
    }
}
