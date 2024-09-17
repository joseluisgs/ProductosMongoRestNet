using System.Text;
using Serilog;
using Serilog.Core;

// Iniciamos la configuración externa de la aplicación
var logger = InitLogConfig();

// Inicializamos los servicios de la aplicación
var builder = InitServices();

// Creamos la aplicación
var app = builder.Build();

// Swagger para documentar la API
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Usamos HTTPS redirection
app.UseHttpsRedirection();

// Habilitamos el middleware de Autorización
app.UseAuthorization();

// Mapeamos los controladores a la aplicación
app.MapControllers();

// Ejecutamos la aplicación
app.Run();


// Inicializa los servicios de la aplicación
WebApplicationBuilder InitServices()
{
    var myBuilder = WebApplication.CreateBuilder(args);

    // Configuramos los servicios de la aplicación

    // Poner Serilog como logger por defecto (otra alternativa)
    myBuilder.Services.AddLogging(logging =>
    {
        logging.ClearProviders(); // Limpia los proveedores de log por defecto
        logging.AddSerilog(logger, true); // Añade Serilog como un proveedor de log
    });
    logger.Debug("Serilog added as default logger");
    
    // Añadimos los controladores
    myBuilder.Services.AddControllers();
    
    
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    myBuilder.Services.AddEndpointsApiExplorer(); // para documentar la API
    myBuilder.Services.AddSwaggerGen(); // para documentar la API
    return myBuilder;
}

// Inicializa la configuración externa de la aplicación
Logger InitLogConfig()
{
    Console.OutputEncoding = Encoding.UTF8; // Necesario para mostrar emojis

// Leemos en qué entorno estamos
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "";
    Console.WriteLine($"Environment: {environment}");


// Configuramos Serilog
    var configuration = new ConfigurationBuilder()
        .AddJsonFile($"appsettings.{environment}.json", false, true)
        .Build();

// Creamos un logger con la configuración de Serilog
    return new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}