using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;

namespace TestScaffolderExtension.Extensions
{
    public static class IAsyncServiceProviderExtensions
    {
        public static async Task<T> GetAsync<T>(this IAsyncServiceProvider serviceProvider)
            where T : class
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            return await serviceProvider.GetServiceAsync(typeof(T)) as T;
        }

        public static async Task<U> GetAsAsync<T, U>(this IAsyncServiceProvider serviceProvider)
            where T : class
            where U : class
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            return await serviceProvider.GetServiceAsync(typeof(T)) as U;
        }
    }
}