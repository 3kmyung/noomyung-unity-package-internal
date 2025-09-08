namespace Noomyung.UI.Domain.Enums
{
    /// <summary>
    /// UI 효과가 트리거되는 시점을 정의합니다.
    /// </summary>
    public enum EffectTrigger
    {
        /// <summary>UI 요소가 표시될 때</summary>
        Show,
        
        /// <summary>UI 요소가 숨겨질 때</summary>
        Hide,
        
        /// <summary>마우스가 UI 요소에 진입할 때</summary>
        HoverEnter,
        
        /// <summary>마우스가 UI 요소에서 벗어날 때</summary>
        HoverExit,
        
        /// <summary>UI 요소가 클릭될 때</summary>
        Click
    }
}
