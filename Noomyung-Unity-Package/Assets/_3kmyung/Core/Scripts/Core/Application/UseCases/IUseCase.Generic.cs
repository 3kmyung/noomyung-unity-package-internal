using System.Threading;
using System.Threading.Tasks;

namespace _3kmyung.Authentication.Application
{
    public interface IUseCase<in TRequest, TResponse>
    {
        Task<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
    }
}
