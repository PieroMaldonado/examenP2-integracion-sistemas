using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar la cadena de conexión y el DbContext
builder.Services.AddDbContext<AgenciaViajesDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AgenciaViajesDB")));

// Agregar HttpClient
builder.Services.AddHttpClient();

// Agregar servicios necesarios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
