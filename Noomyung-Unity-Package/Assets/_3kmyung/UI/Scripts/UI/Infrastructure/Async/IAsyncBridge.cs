using System.Threading.Tasks;

namespace Noomyung.UI.Infrastructure.Async
{
    /// <summary>
    /// Task와 UniTask 간의 변환을 제공하는 브리지 인터페이스입니다.
    /// </summary>
    public interface IAsyncBridge
    {
        /// <summary>
        /// UniTask를 Task로 변환합니다.
        /// </summary>
        /// <param name="uniTask">변환할 UniTask</param>
        /// <returns>변환된 Task</returns>
        Task ToTask(object uniTask);
        
        /// <summary>
        /// Task를 UniTask로 변환합니다.
        /// </summary>
        /// <param name="task">변환할 Task</param>
        /// <returns>변환된 UniTask</returns>
        object ToUniTask(Task task);
    }
}
