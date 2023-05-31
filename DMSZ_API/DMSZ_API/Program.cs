using DMSZ_API.Data;
using DMSZ_API.Roles;
using DMSZ_API.Swagger;
using LinqToDB.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(j =>
{
    j.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = config["JwtSettings:Issuer"],
        ValidAudience = config["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(config["JwtSettings:Key"]!)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
    };
});

builder.Services.AddAuthorization(opt =>
{
    foreach (var policyClaim in IdentityData.Identity)
    {
        opt.AddPolicy(policyClaim.Item1,
            cl => cl.RequireClaim("userRole", policyClaim.Item2));
    }

    foreach (var extendedPolicies in IdentityData.Roles())
    {
        opt.AddPolicy(extendedPolicies.Item1, policy =>
            policy.RequireAssertion(context => context.User.HasClaim(c => c.Type.Equals("userRole") ? IdentityData.IsInRoles(c.Value, extendedPolicies.Item1) : false)));
    }


});

builder.Services.AddSingleton<IConfiguration>(x => config);

#region Allow CORS origins

builder.Services.AddCors(cor =>
{
    cor.AddDefaultPolicy(opt => opt.AllowAnyOrigin()
                                    .AllowAnyHeader()
                                    .AllowAnyMethod());
});

#endregion

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

#region Middleware

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

#endregion