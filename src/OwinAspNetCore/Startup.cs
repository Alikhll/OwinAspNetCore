﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Owin;
using System;
using Microsoft.Owin.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Web.OData.Builder;
using System.Web.Http;
using System.Web.OData.Extensions;

namespace OwinAspNetCore
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMvc();
        }

        public void Configure(IApplicationBuilder aspNetCoreApp, IHostingEnvironment env)
        {
            //aspNetCoreApp.UseMvc();

            aspNetCoreApp.UseOwinApp(owinApp =>
            {
                if (env.IsDevelopment())
                {
                    owinApp.UseErrorPage(new ErrorPageOptions()
                    {
                        ShowCookies = true,
                        ShowEnvironment = true,
                        ShowExceptionDetails = true,
                        ShowHeaders = true,
                        ShowQuery = true,
                        ShowSourceCode = true
                    });
                }
                // owinApp.UseFileServer(); as like as asp.net core static files middleware
                // owinApp.UseStaticFiles(); as like as asp.net core static files middleware
                // owinApp.UseWebApi(); asp.net web api / odata / web hooks

                HttpConfiguration webApiConfig = new HttpConfiguration();
                ODataModelBuilder odataMetadataBuilder = new ODataConventionModelBuilder();
                odataMetadataBuilder.EntitySet<Product>("Products");
                webApiConfig.MapODataServiceRoute(
                    routeName: "ODataRoute",
                    routePrefix: "odata",
                    model: odataMetadataBuilder.GetEdmModel());
                owinApp.UseWebApi(webApiConfig);

                owinApp.MapSignalR();

                //owinApp.Use<AddSampleHeaderToResponseHeadersOwinMiddleware>();
            });

            //aspNetCoreApp.UseMiddleware<AddSampleHeaderToResponseHeadersAspNetCoreMiddlware>();
        }
    }

    public class AddSampleHeaderToResponseHeadersOwinMiddleware : OwinMiddleware
    {
        public AddSampleHeaderToResponseHeadersOwinMiddleware(OwinMiddleware next)
            : base(next)
        {

        }

        public async override Task Invoke(IOwinContext context)
        {
            //throw new InvalidOperationException("ErrorTest");

            //context.Response.Headers.Add("Test", new[] { context.Request.Uri.ToString() });

            await Next.Invoke(context);
        }
    }

    public class AddSampleHeaderToResponseHeadersAspNetCoreMiddlware
    {
        private readonly RequestDelegate Next;

        public AddSampleHeaderToResponseHeadersAspNetCoreMiddlware(RequestDelegate next)
        {
            Next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            //throw new InvalidOperationException("ErrorTest");

            //context.Response.Headers.Add("Test", new[] { context.Request.Path.ToString() });

            await Next.Invoke(context);
        }
    }
}
