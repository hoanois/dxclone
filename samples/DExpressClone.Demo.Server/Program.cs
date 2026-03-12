using DExpressClone.Components;
using DExpressClone.Demo.Server.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDxComponents();
builder.Services.AddSingleton<SampleDataService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<DExpressClone.Demo.Server.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
