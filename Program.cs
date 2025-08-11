using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuration already picks up appsettings.json
builder.Services.AddControllers();

// Register IHttpClientFactory
builder.Services.AddHttpClient();

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://aaravgotcookies.onrender.com"
) // React dev server URL
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FirebaseCounter API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FirebaseCounter API V1");
    });
}

app.UseHttpsRedirection();

// Enable CORS middleware before authorization and endpoints
app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
