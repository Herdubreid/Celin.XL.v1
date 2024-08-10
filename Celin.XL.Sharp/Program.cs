using BlazorState;
using Celin.XL.Sharp;
using Celin.XL.Sharp.Components;
using Celin.XL.Sharp.Service;
using Celin.XL.Sharp.Services;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.DetailedErrors = true;
    })
    .AddHubOptions(options =>
    {
        options.MaximumReceiveMessageSize = 10 * 1024 * 1024;
    });

builder.Services
    .AddLogging(config =>
    {
        config.AddConfiguration(builder.Configuration.GetSection("Logging"));
        config.AddDebug();
        config.AddConsole();
    })
    .AddMudServices()
    .AddBlazorState(options =>
    {
        options.UseCloneStateBehavior = false;
        options.Assemblies = [typeof(Program).Assembly];
    })
    .AddScoped<E1Services>()
    .AddScoped<JsService>()
    .AddScoped<SharpService>()
    .AddScoped<OutputWriterService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
