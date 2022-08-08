// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Autofac.Integration.Owin;
using Microsoft.Owin;

namespace Autofac.Integration.Mvc.Owin.Test;

public class TestMiddleware : OwinMiddleware
{
    public TestMiddleware(OwinMiddleware next)
        : base(next)
    {
        LifetimeScope = null;
    }

    public static ILifetimeScope LifetimeScope { get; private set; }

    public override Task Invoke(IOwinContext context)
    {
        LifetimeScope = context.GetAutofacLifetimeScope();
        return Next.Invoke(context);
    }
}
