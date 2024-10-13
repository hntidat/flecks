using Aspire.Hosting.Lifecycle;

namespace flecks.AppHost
{
    internal static class Extensions
    {
        /// <summary>
        /// Adds a hook to set the ASPNETCORE_FORWARDEDHEADERS_ENABLED environment variable to true for all projects in the application.
        /// work with reverse proxy and cdn
        /// </summary>
        public static IDistributedApplicationBuilder AddForwardedHeaders(this IDistributedApplicationBuilder builder)
        {
            builder.Services.TryAddLifecycleHook<AddForwardHeadersHook>();
            return builder;
        }

        private class AddForwardHeadersHook : IDistributedApplicationLifecycleHook
        {
            public Task BeforeStartAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
            {
                foreach (var p in appModel.GetProjectResources())
                {
                    p.Annotations.Add(new EnvironmentCallbackAnnotation(context =>
                    {
                        context.EnvironmentVariables["ASPNETCORE_FORWARDEDHEADERS_ENABLED"] = "true";
                    }));
                }

                return Task.CompletedTask;
            }
        }
    }
}
