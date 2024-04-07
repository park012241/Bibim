using Bibim.Extensions;
using Bibim.Models;
using HyperLapse.Bibim.Service.AudioQueue.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure the services
builder.Services.Configure<DiscordOptions>(builder.Configuration.GetSection(DiscordOptions.SectionName));

// Add services to the container.
builder.Services
    .AddAudioQueue()
    .AddDiscordClient();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();