using System;

namespace Noomyung.UI.Domain.Enums
{
    /// <summary>
    /// 이동 및 스케일 효과에서 적용할 축을 지정합니다.
    /// </summary>
    [Flags]
    public enum AxisMask
    {
        /// <summary>축 지정 없음</summary>
        None = 0,
        
        /// <summary>X축</summary>
        X = 1 << 0,
        
        /// <summary>Y축</summary>
        Y = 1 << 1,
        
        /// <summary>Z축</summary>
        Z = 1 << 2,
        
        /// <summary>X, Y축</summary>
        XY = X | Y,
        
        /// <summary>X, Y, Z축 모두</summary>
        XYZ = X | Y | Z
    }
}
