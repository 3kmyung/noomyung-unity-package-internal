using System.Threading;
using System.Threading.Tasks;

namespace Noomyung.UI.Application.Ports
{
    /// <summary>
    /// UI 요소의 호버 상호작용을 관리하는 인터페이스입니다.
    /// 마우스 진입/벗어남 효과를 담당합니다.
    /// </summary>
    public interface IUiHoverView
    {
        /// <summary>
        /// 호버 진입 효과를 재생합니다.
        /// </summary>
        /// <param name="cancellationToken">취소 토큰</param>
        /// <returns>비동기 작업</returns>
        Task HoverEnterAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 호버 벗어남 효과를 재생합니다.
        /// </summary>
        /// <param name="cancellationToken">취소 토큰</param>
        /// <returns>비동기 작업</returns>
        Task HoverExitAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 현재 호버 상태인지 여부를 확인합니다.
        /// </summary>
        bool IsHovered { get; }
    }
}
