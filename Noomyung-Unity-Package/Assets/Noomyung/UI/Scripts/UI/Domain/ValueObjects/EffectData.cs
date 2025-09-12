using System;
using System.Drawing;
using System.Numerics;
using Noomyung.UI.Domain.Enums;

namespace Noomyung.UI.Domain.ValueObjects
{
    /// <summary>
    /// 효과별 특화된 데이터 구조체들의 기본 인터페이스입니다.
    /// </summary>
    public interface IEffectData
    {
        EffectType Type { get; }
    }

    /// <summary>
    /// 페이드 효과 데이터입니다.
    /// </summary>
    public readonly struct FadeEffectData : IEffectData
    {
        public EffectType Type => EffectType.Fade;
        public float From { get; }
        public float To { get; }

        public FadeEffectData(float from, float to)
        {
            From = from;
            To = to;
        }
    }

    /// <summary>
    /// 스케일 효과 데이터입니다.
    /// </summary>
    public readonly struct ScaleEffectData : IEffectData
    {
        public EffectType Type => EffectType.Scale;
        public Vector3 From { get; }
        public Vector3 To { get; }
        public string AxisMask { get; }

        public ScaleEffectData(Vector3 from, Vector3 to, string axisMask = "XYZ")
        {
            From = from;
            To = to;
            AxisMask = axisMask;
        }
    }

    /// <summary>
    /// 이동 효과 데이터입니다.
    /// </summary>
    public readonly struct MoveEffectData : IEffectData
    {
        public EffectType Type => EffectType.Move;
        public Vector3 From { get; }
        public Vector3 To { get; }
        public string Space { get; }

        public MoveEffectData(Vector3 from, Vector3 to, string space = "Anchored")
        {
            From = from;
            To = to;
            Space = space;
        }
    }

    /// <summary>
    /// 색상 효과 데이터입니다.
    /// </summary>
    public readonly struct ColorEffectData : IEffectData
    {
        public EffectType Type => EffectType.Color;
        public Color From { get; }
        public Color To { get; }
        public string TargetMode { get; }

        public ColorEffectData(Color from, Color to, string targetMode = "Graphic")
        {
            From = from;
            To = to;
            TargetMode = targetMode;
        }
    }

    /// <summary>
    /// 머티리얼 Float 효과 데이터입니다.
    /// </summary>
    public readonly struct MaterialFloatEffectData : IEffectData
    {
        public EffectType Type => EffectType.MaterialFloat;
        public string PropertyName { get; }
        public float From { get; }
        public float To { get; }

        public MaterialFloatEffectData(string propertyName, float from, float to)
        {
            PropertyName = propertyName;
            From = from;
            To = to;
        }
    }

    /// <summary>
    /// 머티리얼 Color 효과 데이터입니다.
    /// </summary>
    public readonly struct MaterialColorEffectData : IEffectData
    {
        public EffectType Type => EffectType.MaterialColor;
        public string PropertyName { get; }
        public Color From { get; }
        public Color To { get; }

        public MaterialColorEffectData(string propertyName, Color from, Color to)
        {
            PropertyName = propertyName;
            From = from;
            To = to;
        }
    }

    /// <summary>
    /// 흔들림 효과 데이터입니다.
    /// </summary>
    public readonly struct ShakeEffectData : IEffectData
    {
        public EffectType Type => EffectType.Shake;
        public Vector3 Strength { get; }
        public float Frequency { get; }
        public int Vibrato { get; }

        public ShakeEffectData(Vector3 strength, float frequency = 10f, int vibrato = 10)
        {
            Strength = strength;
            Frequency = frequency;
            Vibrato = vibrato;
        }
    }
}
