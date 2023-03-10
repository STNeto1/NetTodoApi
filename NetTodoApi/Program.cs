using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NetTodoApi.Models;
using NetTodoApi.Utils;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
    .AddJsonOptions(opts => { opts.JsonSerializerOptions.PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance; });
builder.Services.AddDbContext<DatabaseContext>(opt =>
    opt.UseInMemoryDatabase("TodoList")
);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
{
    opts.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    opts.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateAudience = false,
        ValidateIssuer = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            builder.Configuration.GetSection("AppSettings:Token").Value!))
    };
});

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