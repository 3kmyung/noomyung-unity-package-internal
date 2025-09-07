using System.Threading;
using System.Threading.Tasks;
using Noomyung.UI.Domain.Interfaces;

namespace Noomyung.UI.Application.Interfaces
{
    /// <summary>
    /// UI 전환 서비스 인터페이스입니다.
    /// </summary>
    public interface IUITransitionService
    {
        /// <summary>
        /// UI 요소를 표시하는 전환을 실행합니다.
        /// </summary>
        /// <param name="target">대상 UI 요소 핸들</param>
        /// <param name="cancellationToken">취소 토큰</param>
        /// <returns>완료 작업</returns>
        Task ShowAsync(IUIElementHandle target, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// UI 요소를 숨기는 전환을 실행합니다.
        /// </summary>
        /// <param name="target">대상 UI 요소 핸들</param>
        /// <param name="cancellationToken">취소 토큰</param>
        /// <returns>완료 작업</returns>
        Task HideAsync(IUIElementHandle target, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 마우스 진입 시 전환을 실행합니다.
        /// </summary>
        /// <param name="target">대상 UI 요소 핸들</param>
        /// <param name="cancellationToken">취소 토큰</param>
        /// <returns>완료 작업</returns>
        Task HoverEnterAsync(IUIElementHandle target, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 마우스 벗어날 시 전환을 실행합니다.
        /// </summary>
        /// <param name="target">대상 UI 요소 핸들</param>
        /// <param name="cancellationToken">취소 토큰</param>
        /// <returns>완료 작업</returns>
        Task HoverExitAsync(IUIElementHandle target, CancellationToken cancellationToken = default);
    }
}
