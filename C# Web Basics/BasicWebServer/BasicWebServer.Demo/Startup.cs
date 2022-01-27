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
        .MapPost<HomeController>("/Content", c => c.DownloadContent()))
        //.MapGet<HomeController>("/Cookies", c => c.Cookies())
        //.MapGet<HomeController>("/Session", c => c.Session()))
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

    

    private static void AddCookiesAction(Request request, Response response)
    {
        bool requestHasCookies = request.Cookies.Any(c => c.Name != Session.SessionCookieName);
        string bodyText = "";

        if (requestHasCookies)
        {
            StringBuilder cookieText = new StringBuilder();
            cookieText.AppendLine("<h1>Cookies</h1>");

            cookieText.Append("<table border='1'><tr><th>Name</th><th>Value</th></tr>");

            foreach (var cookie in request.Cookies)
            {
                cookieText.Append("<tr>");
                cookieText.Append($"<td>{HttpUtility.HtmlEncode(cookie.Name)}</td>");
                cookieText.Append($"<td>{HttpUtility.HtmlEncode(cookie.Value)}</td>");
                cookieText.Append("</tr>");
            }

            cookieText.Append("</table>");

            bodyText = cookieText.ToString();
        }
        else
        {
            bodyText = "<h1>Cookies set!</h1>";
            response.Cookies.Add("My-Cookie", "My-Value");
            response.Cookies.Add("My-Second-Cookie", "My-Second-Value");
        }

        response.Body = bodyText;
    }

    

    private static void DisplaySessionInfoAction(Request request, Response response)
    {
        bool sessionExists = request.Session
            .ContainsKey(Session.SessionCurrentDateKey);

        string bodyText = "";

        if (sessionExists)
        {
            var currentDate = request.Session[Session.SessionCurrentDateKey];
            bodyText = $"Stored date: {currentDate}";
        }
        else
        {
            bodyText = "Current date stored!";
        }

        response.Body = "";
        response.Body += bodyText;
    }

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