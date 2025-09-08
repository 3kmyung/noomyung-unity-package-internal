using System.Collections.Generic;
using Noomyung.UI.Domain.ValueObjects;

namespace Noomyung.UI.Domain.Interfaces
{
    /// <summary>
    /// UI 요소에 대한 추상적인 핸들을 제공합니다.
    /// Unity 타입에 종속되지 않는 도메인 레벨 인터페이스입니다.
    /// </summary>
    public interface IUIElementHandle
    {
        /// <summary>투명도 값 (0.0 ~ 1.0)</summary>
        float Alpha { get; set; }
        
        /// <summary>앵커 기준 위치</summary>
        Vector3Value AnchoredPosition { get; set; }
        
        /// <summary>로컬 스케일</summary>
        Vector3Value LocalScale { get; set; }
        
        /// <summary>로컬 위치</summary>
        Vector3Value LocalPosition { get; set; }
        
        /// <summary>UI 요소가 활성화되어 있는지 여부</summary>
        bool IsActive { get; set; }
        
        /// <summary>그래픽 컴포넌트들의 색상 목록을 가져옵니다.</summary>
        IReadOnlyList<ColorValue> GetGraphicColors();
        
        /// <summary>지정된 인덱스의 그래픽 색상을 설정합니다.</summary>
        void SetGraphicColor(int index, ColorValue color);
        
        /// <summary>모든 그래픽 컴포넌트의 색상을 설정합니다.</summary>
        void SetAllGraphicColors(ColorValue color);
        
        /// <summary>머티리얼의 float 속성 값을 가져옵니다.</summary>
        float GetMaterialFloat(string propertyName);
        
        /// <summary>머티리얼의 float 속성 값을 설정합니다.</summary>
        void SetMaterialFloat(string propertyName, float value);
        
        /// <summary>머티리얼의 색상 속성 값을 가져옵니다.</summary>
        ColorValue GetMaterialColor(string propertyName);
        
        /// <summary>머티리얼의 색상 속성 값을 설정합니다.</summary>
        void SetMaterialColor(string propertyName, ColorValue color);
        
        /// <summary>원래 위치를 저장합니다 (흔들림 효과용).</summary>
        void StoreOriginalPosition();
        
        /// <summary>저장된 원래 위치로 복원합니다.</summary>
        void RestoreOriginalPosition();
    }
}
