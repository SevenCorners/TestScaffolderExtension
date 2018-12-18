namespace TestScaffolderExtension.Extensions
{
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.Shell;

    public static class IAsyncServiceProviderExtensions
    {
        public static async Task<T> GetAsync<T>(this IAsyncServiceProvider serviceProvider)
            where T : class
        {
            return await serviceProvider.GetServiceAsync(typeof(T)) as T;
        }

        public static async Task<U> GetAsAsync<T, U>(this IAsyncServiceProvider serviceProvider)
            where T : class
            where U : class
        {
            return await serviceProvider.GetServiceAsync(typeof(T)) as U;
        }
    }
}