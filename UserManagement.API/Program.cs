using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserManagement.Domain.DBContext;
using UserManagement.Domain.Hubs;
using UserManagement.Domain.Repositories.Interfaces;
using UserManagement.Domain.Repositories.Repositories;
using UserManagement.Domain.utils;
using UserManagement.Services.Interfaces;
using UserManagement.Services.JWT;
using UserManagement.Services.Services;

var builder = WebApplication.CreateBuilder(args);
var conn = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddControllers(); 
builder.Services.AddDbContext<UserDbContext>(options => options.UseNpgsql(conn));
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Test API",
        Version = "v1"
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", builder =>
    {
        builder.WithOrigins("http://localhost:5214") // Allow requests from the MVC application
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});
builder.Services.AddSignalR();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IGenerateJwt, GenerateJwt>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IForgotPasswordService, ForgotPasswordService>();
builder.Services.AddScoped<IChangePasswordService, ChangePasswordService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICLAService, CLAService>();
builder.Services.AddScoped<IWishListRepository, WishListRepository>();
builder.Services.AddScoped<IWishListService, WishListService>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ISubscribedUserRepository, SubscribedUserRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IValidationService, ValidationService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddSingleton<ResponseHandler>();
builder.Services.AddSingleton<PaginationService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "http://localhost:5114",
            ValidAudience = "http://localhost:5114",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("b14ca5898a4e4133bbce2ea2315a1916"))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
            string? token = context.Request.Cookies["token"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Request.Headers["Authorization"] = "Bearer " + token;
            }
            return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Test API V1");
    });
}
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub"); 
app.UseCors("AllowSpecificOrigins");
app.Run();

