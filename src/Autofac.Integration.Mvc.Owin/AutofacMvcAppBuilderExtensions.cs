// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using Autofac;
using Autofac.Integration.Owin;

namespace Owin
{
    /// <summary>
    /// Extension methods for configuring the OWIN pipeline.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class AutofacMvcAppBuilderExtensions
    {
        /// <summary>
        /// A factory method to allow for mocking in unit tests.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Required for unit testing")]
        internal static Func<HttpContextBase> CurrentHttpContext = () => new HttpContextWrapper(HttpContext.Current);

        /// <summary>
        /// Extends the Autofac lifetime scope added from the OWIN pipeline through to the MVC request lifetime scope.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <returns>The application builder.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static IAppBuilder UseAutofacMvc(this IAppBuilder app) =>
            app.Use(async (context, next) =>
            {
                var lifetimeScope = context.GetAutofacLifetimeScope();
                var httpContext = CurrentHttpContext();

                if (lifetimeScope != null && httpContext != null)
                {
                    httpContext.Items[typeof(ILifetimeScope)] = lifetimeScope;
                }

                await next().ConfigureAwait(false);
            });
    }
}
