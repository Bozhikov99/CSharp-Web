using BasicWebServer.Demo.Controllers;
using BasicWebServer.Server;
using BasicWebServer.Server.HTTP;
using BasicWebServer.Server.Responses;
using BasicWebServer.Server.Routing;
using System.Text;
using System.Web;

public class Startup
{
    public static async Task Main()
    {
        await new HttpServer(routes => routes
        .MapGet<HomeController>("/", c => c.Index())
        .MapGet<HomeController>("/HTML", c => c.Html())
        .MapGet<HomeController>("/Redirect", c => c.Redirect())
        .MapPost<HomeController>("/HTML", c => c.HtmlFormPost())
        .MapGet<HomeController>("/Content", c => c.Content())
        .MapPost<HomeController>("/Content", c => c.DownloadContent())
        .MapGet<HomeController>("/Cookies", c => c.Cookies())
        .MapGet<HomeController>("/Session", c => c.Session())
        .MapGet<UsersController>("/Login", c => c.Login())
        .MapPost<UsersController>("/Login", c => c.LogInUser())
        .MapGet<UsersController>("/Logout", c => c.Logout())
        .MapGet<UsersController>("/UserProfile", c => c.GetUserDataAction()))
         .Start();
    }
}