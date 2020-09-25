// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Testing;
using Moq;
using Owin;
using Xunit;
using OwinExtensions = Owin.AutofacMvcAppBuilderExtensions;

namespace Autofac.Integration.Mvc.Owin.Test
{
    public class AutofacMvcAppBuilderExtensionsFixture
    {
        [Fact]
        public async Task UseAutofacMvcUpdatesHttpContextWithLifetimeScopeFromOwinContext()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<TestMiddleware>();
            var container = builder.Build();

            var httpContext = new Mock<HttpContextBase>();
            httpContext.SetupSet(mock => mock.Items[typeof(ILifetimeScope)] = It.IsAny<ILifetimeScope>()).Verifiable();
            OwinExtensions.CurrentHttpContext = () => httpContext.Object;

            using (var server = TestServer.Create(app =>
            {
                app.UseAutofacMiddleware(container);
                app.UseAutofacMvc();
                app.Run(context => context.Response.WriteAsync("Hello, world!"));
            }))
            {
                await server.HttpClient.GetAsync("/");
                httpContext.VerifyAll();

                Assert.NotNull(TestMiddleware.LifetimeScope);
            }
        }
    }
}
