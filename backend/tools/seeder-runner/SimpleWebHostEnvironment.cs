using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

public class SimpleWebHostEnvironment : IWebHostEnvironment
{
    public string EnvironmentName { get; set; } = "Development";
    public string ApplicationName { get; set; } = AppDomain.CurrentDomain.FriendlyName;
    public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
    public IFileProvider ContentRootFileProvider { get; set; }
    public string WebRootPath { get; set; }
    public IFileProvider WebRootFileProvider { get; set; }
}
