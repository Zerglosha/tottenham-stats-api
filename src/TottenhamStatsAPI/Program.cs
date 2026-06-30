using Microsoft.EntityFrameworkCore;
using TottenhamStatsAPI.Data;
using TottenhamStatsAPI.Endpoints;
using TottenhamStatsAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<RequestLoggingMiddleware>();

app.MapPlayerEndpoints();
app.MapClubEndpoints();
app.MapMatchEndpoints();
app.MapCompetitionStandingEndpoints();
app.MapDashboardEndpoints();

app.Run();

public partial class Program { }