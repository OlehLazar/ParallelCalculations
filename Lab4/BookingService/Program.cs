using BookingService.Context;
using BookingService.Models;
using BookingService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ������ ������
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = Microsoft.OpenApi.Models.ParameterLocation.Header,
		Description = "������ JWT ����� � ������: Bearer {your_token}"
	});

	options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
	{
		{
			new Microsoft.OpenApi.Models.OpenApiSecurityScheme
			{
				Reference = new Microsoft.OpenApi.Models.OpenApiReference
				{
					Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			new string[] {}
		}
	});
});

// ������������ InMemory Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseInMemoryDatabase("BookingDB"));

// ��������� BookingsService
builder.Services.AddScoped<BookingsService>();


// JWT Authentication (��� �������� ��������)
var jwtKey = Encoding.ASCII.GetBytes("your-256-bit-secret-key-must-be-long-enough");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
			ValidateIssuer = false,
			ValidateAudience = false,
			ValidateLifetime = true
		};
	});

// ������� �����������
builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("Client", policy => policy.RequireRole("Client"));
	options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
});

// CORS
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", builder =>
		builder.AllowAnyOrigin()
			   .AllowAnyMethod()
			   .AllowAnyHeader());
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseMiddleware<RoleAuthorizationMiddleware>();

app.MapControllers();

// ��������� ������ ���
using (var scope = app.Services.CreateScope())
{
	var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
	context.Database.EnsureCreated();

	if (!context.Rooms.Any())
	{
		context.Rooms.AddRange(
			new Room { RoomClass = "������", Capacity = 2 },
			new Room { RoomClass = "��������", Capacity = 3 },
			new Room { RoomClass = "����", Capacity = 4 }
		);
		await context.SaveChangesAsync();
	}
}

// ��������� �� ����� 5001
app.Urls.Add("http://localhost:5001");
app.Run();