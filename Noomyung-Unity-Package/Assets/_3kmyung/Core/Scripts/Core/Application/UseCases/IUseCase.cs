using System.Threading;
using System.Threading.Tasks;

namespace _3kmyung.Authentication.Application
{
    /// <summary>
    /// Query 인터페이스 - 데이터를 조회하는 작업
    /// </summary>
    /// <typeparam name="TResponse">응답 타입</typeparam>
    public interface IQuery<TResponse>
    {
        Task<TResponse> ExecuteAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Request를 받는 Query 인터페이스 - 데이터를 조회하는 작업
    /// </summary>
    /// <typeparam name="TRequest">요청 타입</typeparam>
    /// <typeparam name="TResponse">응답 타입</typeparam>
    public interface IQuery<in TRequest, TResponse>
    {
        Task<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Command 인터페이스 - 상태를 변경하는 작업
    /// </summary>
    public interface ICommand
    {
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Request를 받는 Command 인터페이스 - 상태를 변경하는 작업
    /// </summary>
    /// <typeparam name="TRequest">요청 타입</typeparam>
    public interface ICommand<in TRequest>
    {
        Task ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
    }
}
