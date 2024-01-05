using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace examenP2.Controllers
{
    [ApiController]
    [Route("datos")]
    public class UsuariosController : ControllerBase
    {
        private readonly AgenciaViajesDbContext _dbContext;
        private readonly HttpClient _httpClient;

        public UsuariosController(AgenciaViajesDbContext dbContext, IHttpClientFactory httpClientFactory)
        {
            _dbContext = dbContext;
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet("{usuarioNombre}")]
        public async Task<IActionResult> GetCiudadByUsuario(string usuarioNombre)
        {
            try
            {
                // Buscar el usuario en la base de datos
                var usuario = _dbContext.Usuario
                    .FirstOrDefault(u => u.Usuario == usuarioNombre);

                if (usuario != null)
                {
                    // Construir la URL de la API externa con el parámetro ciudad
                    string apiUrl = $"https://geocode.xyz/{usuario.ciudad}?json=1";

                    // Realizar la solicitud GET a la API externa
                    HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        // Leer y guardar la respuesta si la solicitud fue exitosa
                        string responseBody = await response.Content.ReadAsStringAsync();

                        // Procesar el JSON de la respuesta
                        var json = JObject.Parse(responseBody);

                        // Extraer los datos necesarios
                        var ciudad = json["standard"]["city"]?.ToString();
                        var prov = json["standard"]["prov"]?.ToString();
                        var countryName = json["standard"]["countryname"]?.ToString();
                        var longt = json["longt"]?.ToString();
                        var latt = json["latt"]?.ToString();

                        // Crear una nueva entidad UsuarioGeoreferencia y guardarla en la base de datos
                        var nuevaGeoreferencia = new UsuarioGeoreferencia
                        {
                            Usuario = usuario.Usuario,
                            City = ciudad,
                            Prov = prov,
                            CountryName = countryName,
                            Longt = Convert.ToDouble(longt),
                            Latt = Convert.ToDouble(latt)
                        };

                        _dbContext.UsuarioGeoreferencia.Add(nuevaGeoreferencia);
                        await _dbContext.SaveChangesAsync();

                        return Ok(responseBody);
                    }
                    else
                    {
                        // Devolver un 500 si la solicitud no fue exitosa
                        return StatusCode(500, $"Error al realizar la solicitud a la API externa: {response.ReasonPhrase}");
                    }
                }
                else
                {
                    // Devolver un 404 si el usuario no se encuentra
                    return NotFound($"Usuario con nombre {usuarioNombre} no encontrado");
                }
            }
            catch (Exception ex)
            {
                // Manejar errores de manera apropiada
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
