using eVoucher_Entities.EntityModels;
using eVoucher_Repo;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connectionString = builder.Configuration.GetConnectionString("eStoreConnection");
builder.Services.AddDbContext<eVoucherContext>(
options =>
{
    options.UseMySql(connectionString,
    //Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.23-mysql"));
    ServerVersion.AutoDetect(connectionString));
});
builder.Services.AddScoped<IRepositories, Repositories>();
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
