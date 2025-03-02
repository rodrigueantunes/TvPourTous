using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting; // Ajouté pour accéder à ConfigureWebHostDefaults
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Text.Json.Serialization;
using TvPourTous; // Pour accéder à MainWindow

namespace TvPourTous.Api
{
    // Représente la requête POST pour changer de chaîne
    public class ChangeChannelRequest
    {
        [JsonPropertyName("ChannelName")]
        public string ChannelName { get; set; }
    }

    public static class ApiServer
    {
        // Renommage de la propriété pour éviter le conflit avec le type Host
        public static IHost ApiHost { get; private set; }

        // Démarre le serveur API sur le port 5000
        public static void Start(string[] args = null)
        {
            ApiHost = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args ?? new string[] { })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel()
                              .UseUrls("http://*:5000") // écoute sur le port 5000
                              .ConfigureServices(services =>
                              {
                                  // Autoriser l'accès CORS depuis n'importe quelle origine
                                  services.AddCors(options =>
                                  {
                                      options.AddPolicy("AllowAll", builder =>
                                      {
                                          builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                                      });
                                  });
                                  // Injection de l'instance principale de MainWindow
                                  services.AddSingleton(System.Windows.Application.Current.MainWindow as MainWindow);
                              })
                              .Configure(app =>
                              {
                                  app.UseCors("AllowAll");
                                  app.UseRouting();
                                  app.UseEndpoints(endpoints =>
                                  {
                                      // GET /api/channels : retourne la liste des chaînes
                                      endpoints.MapGet("/api/channels", async context =>
                                      {
                                          var mainWindow = context.RequestServices.GetService<MainWindow>();
                                          var channels = mainWindow.GetChannels().Select(kvp => new { Name = kvp.Key, Url = kvp.Value });
                                          context.Response.ContentType = "application/json";
                                          await context.Response.WriteAsJsonAsync(channels);
                                      });

                                      // POST /api/channelcontrol/change : change la chaîne en cours
                                      endpoints.MapPost("/api/channelcontrol/change", async context =>
                                      {
                                          var mainWindow = context.RequestServices.GetService<MainWindow>();
                                          var request = await context.Request.ReadFromJsonAsync<ChangeChannelRequest>();
                                          if (request == null || string.IsNullOrWhiteSpace(request.ChannelName))
                                          {
                                              context.Response.StatusCode = 400;
                                              await context.Response.WriteAsJsonAsync(new { Message = "Requête invalide" });
                                              return;
                                          }
                                          var channels = mainWindow.GetChannels();
                                          if (channels.TryGetValue(request.ChannelName, out string url))
                                          {
                                              // Changer la chaîne en appelant la méthode exposée
                                              mainWindow.ChangeChannel(url);
                                              await context.Response.WriteAsJsonAsync(new { Message = "Chaîne changée avec succès" });
                                          }
                                          else
                                          {
                                              context.Response.StatusCode = 404;
                                              await context.Response.WriteAsJsonAsync(new { Message = "Chaîne non trouvée" });
                                          }
                                      });
                                  });
                              });
                })
                .Build();

            ApiHost.Start();
        }
    }
}
