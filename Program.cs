using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System;


public class Program
{
	public static void Main()
	{
		///boot		
	}
}

public class NotificacionHub: Hub{
	//.. lo que haga mi hub
    public  Task EventoInvocablePorElUsuario(){
        var modelo = new { }; //modelo que contiene data
        return Clients.Caller.SendAsync("Evento", modelo);
    }
}

public class Servicio{
	public Servicio(IHubContext<NotificacionHub> hubContext) => _hubContext = hubContext;
	private readonly IHubContext<NotificacionHub> _hubContext;
	
	public Task EnviarLoQueSea(string IdUser){
		var tarea = new { }; /// lo que sea tu data;
		return _hubContext.Clients.User(IdUser).SendAsync("ActualizarGrafico", tarea);
	}
}

public class EmailBasedUserIdProvider : IUserIdProvider
    {
        /// <summary>
        /// Get user del hub connection context
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public virtual string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst("name")?.Value!;
        }
    }
public static class ConfigureAuthExtensions
{
	public static IServiceCollection ConfigureAuth(this IServiceCollection services, IConfiguration configuration){
	 services
			.AddAuthentication(opt=> {
				//...
			})
			.AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if ((context.Request.Path.Value!.StartsWith("/notificacion")
                           || context.Request.Path.Value!.StartsWith("/ActualizarGrafico")
                           )
                           && context.Request.Query.TryGetValue("access_token", out StringValues token)
                        )
                        {
                            context.Token = token;
                        }
                         
                        return Task.CompletedTask;
                    },
                };
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    //...
                };
                x.IncludeErrorDetails = true;
				///
            });
	return services;
	}	
}
