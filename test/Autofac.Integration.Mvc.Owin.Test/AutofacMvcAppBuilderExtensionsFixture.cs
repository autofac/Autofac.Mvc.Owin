// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Web;
using Microsoft.Owin.Testing;
using NSubstitute;
using Owin;
using Xunit;
using OwinExtensions = Owin.AutofacMvcAppBuilderExtensions;

namespace Autofac.Integration.Mvc.Owin.Test;

public class AutofacMvcAppBuilderExtensionsFixture
{
    [Fact]
    public async Task UseAutofacMvcUpdatesHttpContextWithLifetimeScopeFromOwinContext()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<TestMiddleware>();
        var container = builder.Build();

        var items = new Hashtable();
        var httpContext = Substitute.For<HttpContextBase>();
        httpContext.Items.Returns(items);
        OwinExtensions.CurrentHttpContext = () => httpContext;

        using (var server = TestServer.Create(app =>
        {
            app.UseAutofacMiddleware(container);
            app.UseAutofacMvc();
            app.Run(context => context.Response.WriteAsync("Hello, world!"));
        }))
        {
            await server.HttpClient.GetAsync("/");
            Assert.IsAssignableFrom<ILifetimeScope>(items[typeof(ILifetimeScope)]);

            Assert.NotNull(TestMiddleware.LifetimeScope);
        }
    }
}
