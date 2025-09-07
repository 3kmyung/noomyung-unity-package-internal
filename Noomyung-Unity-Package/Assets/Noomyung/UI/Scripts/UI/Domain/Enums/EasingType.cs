namespace Noomyung.UI.Domain.Enums
{
    /// <summary>
    /// 애니메이션 이징 유형을 정의합니다.
    /// </summary>
    public enum EasingType
    {
        /// <summary>선형 보간</summary>
        Linear,
        
        /// <summary>시작 시 가속</summary>
        EaseIn,
        
        /// <summary>끝날 때 감속</summary>
        EaseOut,
        
        /// <summary>시작과 끝에서 가감속</summary>
        EaseInOut,
        
        /// <summary>사용자 정의 커브</summary>
        CustomCurve
    }
}
