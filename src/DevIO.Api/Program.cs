using DevIO.Api.Configuration;
using DevIO.Api.Data;
using DevIO.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddIdentityConfiguration(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.ResolveDependencies();

builder.Services.AddDbContext<MeuDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.Configure<ApiBehaviorOptions>(options => //Desabilitando a validação automática da ViewModel
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddIdentityConfig(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHsts();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
