using AI_Awards.Configurations;
using AI_Awards.Services;
using AI_Awards.Services.Implementation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.Configure<PythonScriptsConfiguration>(
    builder.Configuration.GetRequiredSection("PythonScriptsConfiguration"));
builder.Services.AddScoped<IPythonService, PythonService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ITextService, TextService>();

builder.Services.AddSignalR(options =>
{
    options.MaximumReceiveMessageSize = 32 * 1024 * 1024;
    options.StreamBufferCapacity = 100;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub(options =>
{
    options.ApplicationMaxBufferSize = 32 * 1024 * 1024;
    options.TransportMaxBufferSize = 32 * 1024 * 1024;
});

app.MapFallbackToPage("/_Host");

app.Run();
