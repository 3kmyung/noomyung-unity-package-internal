namespace Noomyung.UI.Domain.Enums
{
    /// <summary>
    /// 적용 가능한 UI 효과의 유형을 정의합니다.
    /// </summary>
    public enum EffectType
    {
        /// <summary>투명도 효과</summary>
        Fade,
        
        /// <summary>크기 변경 효과</summary>
        Scale,
        
        /// <summary>위치 이동 효과</summary>
        Move,
        
        /// <summary>색상 변경 효과</summary>
        Color,
        
        /// <summary>머티리얼 float 속성 변경</summary>
        MaterialFloat,
        
        /// <summary>머티리얼 색상 속성 변경</summary>
        MaterialColor,
        
        /// <summary>흔들림 효과</summary>
        Shake
    }
}
