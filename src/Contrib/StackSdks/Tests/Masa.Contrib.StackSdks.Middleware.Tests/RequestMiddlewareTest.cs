// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Middleware.Tests;

[TestClass]
public class RequestMiddlewareTest
{
    private IServiceProvider _serviceProvider;

    public RequestMiddlewareTest()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddMasaStackConfig()
            .AddStackMiddleware();

        _serviceProvider = builder.Services.BuildServiceProvider();
    }

    [TestMethod]
    public async Task TestRequestMiddleware()
    {
        DefaultHttpContext defaultContext = new DefaultHttpContext();
        defaultContext.Response.Body = new MemoryStream();
        defaultContext.Request.Path = "/";

        var requestMiddleware = _serviceProvider.GetRequiredService<DisabledRequestMiddleware>();
        await requestMiddleware.InvokeAsync(defaultContext, (innerHttpContext) =>
        {
            innerHttpContext.Response.WriteAsync("content");
            return Task.CompletedTask;
        });

        //Don't forget to rewind the stream
        defaultContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = new StreamReader(defaultContext.Response.Body).ReadToEnd();

        Assert.AreEqual("content", body);
    }

    [TestMethod]
    public void TestDisabledRequestMiddleware()
    {
        DefaultHttpContext defaultContext = new DefaultHttpContext();
        defaultContext.SetEndpoint(new Endpoint(c => Task.CompletedTask, new EndpointMetadataCollection(new DisabledRouteAttribute()), "myapp"));

        var requestMiddleware = _serviceProvider.GetRequiredService<DisabledRequestMiddleware>();

        Assert.ThrowsExceptionAsync<UserFriendlyException>(async () =>
        await requestMiddleware.InvokeAsync(defaultContext, (innerHttpContext) =>
        {
            return Task.CompletedTask;
        }));

        //Assert
        var endpoint = defaultContext.GetEndpoint();
        Assert.IsNotNull(endpoint);
    }
}