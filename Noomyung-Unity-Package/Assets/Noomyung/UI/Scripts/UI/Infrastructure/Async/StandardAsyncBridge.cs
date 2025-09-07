using System.Threading.Tasks;

namespace Noomyung.UI.Infrastructure.Async
{
    /// <summary>
    /// 표준 Task 기반 비동기 브리지 구현체입니다.
    /// UniTask가 없는 환경에서 사용됩니다.
    /// </summary>
    public class StandardAsyncBridge : IAsyncBridge
    {
        /// <inheritdoc />
        public Task ToTask(object uniTask)
        {
            // UniTask가 없는 환경에서는 이미 Task이거나 변환이 불가능
            return uniTask as Task ?? Task.CompletedTask;
        }

        /// <inheritdoc />
        public object ToUniTask(Task task)
        {
            // UniTask가 없는 환경에서는 Task를 그대로 반환
            return task;
        }
    }
}
