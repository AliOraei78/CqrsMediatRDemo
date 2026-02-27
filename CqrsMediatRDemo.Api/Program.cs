using CqrsMediatRDemo.Application.Behaviors;
using CqrsMediatRDemo.Application.Features.Products.Validators;
using CqrsMediatRDemo.Application.Interfaces;
using CqrsMediatRDemo.Application.Interfaces.Repositories;
using CqrsMediatRDemo.Infrastructure.Persistence;
using CqrsMediatRDemo.Infrastructure.Persistence.ReadRepositories;
using CqrsMediatRDemo.Infrastructure.Persistence.Repositories;
using CqrsMediatRDemo.Infrastructure.Services;
using FluentValidation;
using MediatR;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(); builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<WriteDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WriteDb"))
           .AddInterceptors(new OutboxInterceptor()));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddHostedService<OutboxProcessorBackgroundService>();

builder.Services.AddSingleton<ElasticsearchService>();

builder.Services.AddScoped<IProductReadRepository, ProductReadRepository>();
builder.Services.AddMemoryCache();

// Register MediatR with a Validation Behavior (optional but highly recommended)
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        typeof(Program).Assembly,
        typeof(CqrsMediatRDemo.Application.AssemblyReference).Assembly
    );

    cfg.AddOpenBehavior(typeof(CqrsMediatRDemo.Application.Behaviors.ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(CachingBehavior<,>));
});

builder.Services.AddValidatorsFromAssemblyContaining<CreateProductCommandValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();