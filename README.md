# Autofac.Mvc.Owin

OWIN support for the ASP.NET MVC integration for [Autofac](https://autofac.org).

[![Build status](https://ci.appveyor.com/api/projects/status/3fh0r8x7qtbfmv08?svg=true)](https://ci.appveyor.com/project/Autofac/autofac-mvc-owin)

Please file issues and pull requests for this package [in this repository](https://github.com/autofac/Autofac.Mvc.Owin/issues) rather than in the Autofac core repo.

**If you're working with ASP.NET Core, you want [Autofac.Extensions.DependencyInjection](https://www.nuget.org/packages/Autofac.Extensions.DependencyInjection), not this package.**

- [Documentation](https://autofac.readthedocs.io/en/latest/integration/mvc.html)
- [NuGet](https://www.nuget.org/packages/Autofac.Mvc5.Owin)
- [Contributing](https://autofac.readthedocs.io/en/latest/contributors.html)
- [Open in Visual Studio Code](https://open.vscode.dev/autofac/Autofac.Mvc.Owin)

## Quick Start

If you are using MVC [as part of an OWIN application](https://autofac.readthedocs.io/en/latest/integration/owin.html), you need to:

- Do [all the stuff for standard MVC integration](https://autofac.readthedocs.io/en/latest/integration/mvc.html) - register controllers, set the dependency resolver, etc.
- Set up your app with the [base Autofac OWIN integration](https://autofac.readthedocs.io/en/latest/integration/owin.html).
- Add a reference to the [`Autofac.Mvc5.Owin`](https://www.nuget.org/packages/Autofac.Mvc5.Owin/) NuGet package.
- In your application startup class, register the Autofac MVC middleware after registering the base Autofac middleware.

```c#
public class Startup
{
  public void Configuration(IAppBuilder app)
  {
    var builder = new ContainerBuilder();

    // STANDARD MVC SETUP:

    // Register your MVC controllers.
    builder.RegisterControllers(typeof(MvcApplication).Assembly);

    // Run other optional steps, like registering model binders,
    // web abstractions, etc., then set the dependency resolver
    // to be Autofac.
    var container = builder.Build();
    DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

    // OWIN MVC SETUP:

    // Register the Autofac middleware FIRST, then the Autofac MVC middleware.
    app.UseAutofacMiddleware(container);
    app.UseAutofacMvc();
  }
}
```

**Minor gotcha: MVC doesn't run 100% in the OWIN pipeline.** It still needs `HttpContext.Current` and some other non-OWIN things. At application startup, when MVC registers routes, it instantiates an `IControllerFactory` that ends up creating two request lifetime scopes. It only happens during app startup at route registration time, not once requests start getting handled, but it's something to be aware of. This is an artifact of the two pipelines being mangled together.

Check out the [Autofac ASP.NET MVC integration documentation](https://autofac.readthedocs.io/en/latest/integration/mvc.html) for more information.

## Get Help

**Need help with Autofac?** We have [a documentation site](https://autofac.readthedocs.io/) as well as [API documentation](https://autofac.org/apidoc/). We're ready to answer your questions on [Stack Overflow](https://stackoverflow.com/questions/tagged/autofac) or check out the [discussion forum](https://groups.google.com/forum/#forum/autofac).
