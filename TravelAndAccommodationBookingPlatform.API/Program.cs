using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Stripe;
using TravelAndAccommodationBookingPlatform.API.Common;
using TravelAndAccommodationBookingPlatform.API.Common.Interfaces;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Application.Services;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;
using TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;
using TravelAndAccommodationBookingPlatform.Infrastructure.Services;
using ReviewService = TravelAndAccommodationBookingPlatform.Application.Services.ReviewService;

var builder = WebApplication.CreateBuilder(args);

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER")))
{
    DotNetEnv.Env.Load(); 
}
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

string awsAccessKey = Environment.GetEnvironmentVariable("AWSACCESSKEYID")
                      ?? throw new InvalidOperationException("Missing environment variable: AWSACCESSKEYID");

string awsSecretKey = Environment.GetEnvironmentVariable("AWSSECRETACCESSKEY")
                      ?? throw new InvalidOperationException("Missing environment variable: AWSSECRETACCESSKEY");

string awsRegion = Environment.GetEnvironmentVariable("AWSREGION")
                   ?? throw new InvalidOperationException("Missing environment variable: AWSREGION");

string bucketName = Environment.GetEnvironmentVariable("AWSBUCKETNAME")
                    ?? throw new InvalidOperationException("Missing environment variable: BUCKETNAME");

string issuer = Environment.GetEnvironmentVariable("JWTISSUER")
                ?? throw new InvalidOperationException("Missing environment variable: ISSUER");

string audience = Environment.GetEnvironmentVariable("JWTAUDIENCE")
                  ?? throw new InvalidOperationException("Missing environment variable: AUDIENCE");

string secretKey = Environment.GetEnvironmentVariable("JWTSECRETKEY")
                   ?? throw new InvalidOperationException("Missing environment variable: SECRETKEY");

string sqlConnectionString = Environment.GetEnvironmentVariable("SQLSERVERCONNECTIONSTRING")
                             ?? throw new InvalidOperationException("Missing environment variable: SQLSERVERCONNECTIONSTRING");


StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("STRIPESECRETKEY")
                             ?? throw new InvalidOperationException("Missing environment variable: STRIPESECRETKEY");

var credentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);
var region = RegionEndpoint.GetBySystemName(awsRegion);


builder.Services.AddSingleton<IAmazonS3>(_ =>
    new AmazonS3Client(credentials, region)
);

builder.Services.AddScoped<IImageUploader>(sp =>
{
    var s3Client = sp.GetRequiredService<IAmazonS3>();
    return new S3ImageUploader(s3Client, bucketName);
});
builder.Services.AddSingleton<IJwtProvider>(new JwtProvider(secretKey, issuer, audience));
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAppDbContext, SqlServerDbContext>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddDbContext<SqlServerDbContext>(options => { options.UseSqlServer(sqlConnectionString); });

builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
{
    options.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Convert.FromBase64String(secretKey)),
    };
});
builder.Services.AddSwaggerGen(c =>
{
    var xmlCommentsFile = "TravelAndAccommodationBookingPlatform.xml";
    var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
    c.IncludeXmlComments(xmlCommentsFullPath);

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token in the text input below.\nExample: \"Bearer abc123xyz\""
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("MustBeAnAdmin", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("Role", "Admin");
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();