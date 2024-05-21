using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSE.Identity.API.Data;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.Run();