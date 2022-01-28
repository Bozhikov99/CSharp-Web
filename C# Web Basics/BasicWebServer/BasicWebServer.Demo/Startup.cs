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
        //await DownloadSitesAsTextFile(FileName, new string[] { "https://judge.softuni.org/", "https://softuni.org/" });

        await new HttpServer(routes => routes
        .MapGet<HomeController>("/", c => c.Index())
        .MapGet<HomeController>("/HTML", c => c.Html())
        .MapGet<HomeController>("/Redirect", c => c.Redirect())
        .MapPost<HomeController>("/HTML", c => c.HtmlFormPost())
        .MapGet<HomeController>("/Content", c => c.Content())
        .MapPost<HomeController>("/Content", c => c.DownloadContent())
        .MapGet<HomeController>("/Cookies", c => c.Cookies())
        .MapGet<HomeController>("/Session", c => c.Session()))
        //.MapGet<HomeController>("/Login", new HtmlResponse(LoginForm))
        //.MapPost<HomeController>("/Login", new HtmlResponse("", LoginAction))
        //.MapGet<HomeController>("/Logout", new HtmlResponse("", LogoutAction))
        //.MapGet<HomeController>("/UserProfile", new HtmlResponse("", GetUserDataAction)))
         .Start();
    }

    private static void GetUserDataAction(Request request, Response response)
    {
        if (request.Session.ContainsKey(Session.SessionUserKey))
        {
            response.Body = "";
            response.Body += $"<h3> Currently logged-in user \nis with username '{Username}'</h3>";
        }
        else
        {
            response.Body = "";
            response.Body += "<h3>You should first log in \n- <a href='/Login'>Login</a><h3>";
        }
    }



    private const string LoginForm = @"<form action='/Login' method='POST'>
   Username: <input type='text' name='Username'/>
   Password: <input type='text' name='Password'/>
   <input type='submit' value ='Log In' /> 
</form>";


    private const string FileName = "content.txt";

    private const string Username = "user";

    private const string Password = "user123";

    private static void LoginAction(Request request, Response response)
    {
        request.Session.Clear();

        string bodyText = "";

        var usernameMatches = request.Form["Username"] == Username;
        var passwordMatches = request.Form["Password"] == Password;

        if (usernameMatches && passwordMatches)
        {
            request.Session[Session.SessionUserKey] = "MyUserId";
            response.Cookies.Add(Session.SessionCookieName, request.Session.Id);

            bodyText = "<h3>Logged successfully!</h3>";
        }
        else
        {
            bodyText = LoginForm;
        }

        response.Body = "";
        response.Body += bodyText;
    }

    private static void LogoutAction(Request request, Response response)
    {
        request.Session.Clear();

        response.Body = "";
        response.Body += "<h3>You've logged out successfully!</h3>";
    }
}