using System.Threading;
using System.Threading.Tasks;

namespace Noomyung.UI.Application.Ports
{
    /// <summary>
    /// UI 요소의 상태를 나타내는 열거형입니다.
    /// </summary>
    public enum UiState
    {
        /// <summary>기본 상태</summary>
        Normal,
        /// <summary>호버 상태</summary>
        Hovered,
        /// <summary>누름 상태</summary>
        Pressed,
        /// <summary>비활성화 상태</summary>
        Disabled,
        /// <summary>선택된 상태</summary>
        Selected
    }

    /// <summary>
    /// UI 요소의 상태 전환을 관리하는 인터페이스입니다.
    /// Normal, Hovered, Pressed, Disabled, Selected 상태를 관리합니다.
    /// </summary>
    public interface IUiStateView
    {
        /// <summary>
        /// UI 요소의 상태를 변경합니다.
        /// </summary>
        /// <param name="state">새로운 상태</param>
        /// <param name="cancellationToken">취소 토큰</param>
        /// <returns>비동기 작업</returns>
        Task ChangeStateAsync(UiState state, CancellationToken cancellationToken = default);

        /// <summary>
        /// 현재 UI 요소의 상태를 가져옵니다.
        /// </summary>
        UiState CurrentState { get; }
    }
}
