using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;

namespace TestScaffolderExtension.Extensions
{
    public static class IServiceProviderExtensions
    {
        public static T Get<T>(this IServiceProvider serviceProvider)
            where T : class
        {
            return serviceProvider.GetService(typeof(T)) as T;
        }

        public static U GetAs<T, U>(this IServiceProvider serviceProvider)
            where T : class
            where U : class
        {
            return serviceProvider.GetService(typeof(T)) as U;
        }
    }

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