using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Omniverse.WinService
{
    public class BackendServiceClient : IDisposable
    {
        private readonly HttpClient _httpClient;

        public BackendServiceClient(string baseAddress)
        {
            if (Uri.TryCreate(baseAddress, UriKind.Absolute, out var baseAddressUri))
            {
                _httpClient = new HttpClient()
                {
                    BaseAddress = baseAddressUri
                };
            }
            else
            {
                throw new ArgumentException("Base address is not valid");
            }

        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        public async Task<Todo> GetTodoAsync(CancellationToken cancellationToken)
        {
            var uri = new Uri($"/todos/{new Random().Next(1, 10)}", UriKind.Relative);

            var response = await _httpClient.GetAsync(uri, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var todo = await response.Content.ReadAsAsync<Todo>(cancellationToken).ConfigureAwait(false);

            return todo;

        }

    }
}
