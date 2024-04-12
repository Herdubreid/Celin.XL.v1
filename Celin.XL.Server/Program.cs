using BlazorState;
using Celin;
using Celin.XL.Server.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.DetailedErrors = true;
    });

builder.Services
    .AddHttpClient()
    .AddLogging(config =>
    {
        config.AddConfiguration(builder.Configuration.GetSection("Logging"));
        config.AddDebug();
        config.AddConsole();
    })
    .AddScoped<JsService>()
    .AddScoped<Celin.Query.E1Service>()
    .AddBlazorState(options => {
        options.UseStateTransactionBehavior = false;
        options.Assemblies = [typeof(Program).Assembly];
    });

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
