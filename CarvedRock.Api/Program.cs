using CarvedRock.Data;
using CarvedRock.Domain;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
var tracePath = Path.Join(path,$"Log_CarvedRock_{DateTime.Now.ToString("yyyyMMdd-hhmm")}.txt");
Trace.Listeners.Add(new TextWriterTraceListener(File.CreateText(tracePath)));
Trace.AutoFlush = true;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<LocalContext>();
builder.Services.AddScoped<IProductLogic, ProductLogic>();
builder.Services.AddScoped<ICarvedRockRepository, CarvedRockRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<LocalContext>();
    context.MigrateAndCreateData();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapFallback(() => Results.Redirect("/swagger"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
