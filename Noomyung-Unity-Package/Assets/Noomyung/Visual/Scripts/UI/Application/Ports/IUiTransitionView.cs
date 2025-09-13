using System.Threading;
using System.Threading.Tasks;

namespace Noomyung.UI.Application.Ports
{
    /// <summary>
    /// UI 요소의 전환 효과를 관리하는 인터페이스입니다.
    /// 페이드 인/아웃, 애니메이션 등의 전환 효과를 담당합니다.
    /// </summary>
    public interface IUiTransitionView
    {
        /// <summary>
        /// 표시 전환 효과를 재생합니다.
        /// </summary>
        /// <param name="cancellationToken">취소 토큰</param>
        /// <returns>비동기 작업</returns>
        Task PlayShowAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 숨김 전환 효과를 재생합니다.
        /// </summary>
        /// <param name="cancellationToken">취소 토큰</param>
        /// <returns>비동기 작업</returns>
        Task PlayHideAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 전환 효과가 현재 재생 중인지 여부를 확인합니다.
        /// </summary>
        bool IsTransitioning { get; }
    }
}
