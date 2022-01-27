using BasicWebServer.Server;
using BasicWebServer.Server.HTTP;
using BasicWebServer.Server.Responses;
using System.Text;
using System.Web;

public class Startup
{
    public static async Task Main()
    {
        //await DownloadSitesAsTextFile(FileName, new string[] { "https://judge.softuni.org/", "https://softuni.org/" });

        await new HttpServer(routes => routes
        .MapGet("/", new TextResponse("Hello from the server!"))
        .MapGet("/HTML", new HtmlResponse(HtmlForm))
        .MapGet("/Redirect", new RedirectResponse("https://softuni.org/"))
        .MapPost("/HTML", new TextResponse("", AddFormDataAction))
        .MapGet("/Content", new HtmlResponse(DownloadForm))
        .MapPost("/Content", new TextFileResponse(FileName))
        .MapGet("/Cookies", new HtmlResponse("", AddCookiesAction))
        .MapGet("/Session", new TextResponse("", DisplaySessionInfoAction))
        .MapGet("/Login", new HtmlResponse(LoginForm))
        .MapPost("/Login", new HtmlResponse("", LoginAction))
        .MapGet("/Logout", new HtmlResponse("", LogoutAction))
        .MapGet("/UserProfile", new HtmlResponse("", GetUserDataAction)))
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

    private const string HtmlForm = @"<form action='/HTML' method='POST'>
Name: <input type='text' name='Name'/>
Age: <input type='text' name='Age'/>
<input type='submit' value='Save'>
</form>";

    private const string DownloadForm = @"<form action='/Content' method='POST'>
 <input type='submit' value='Download Sites Content' />
</form>";

    private const string LoginForm = @"<form action='/Login' method='POST'>
   Username: <input type='text' name='Username'/>
   Password: <input type='text' name='Password'/>
   <input type='submit' value ='Log In' /> 
</form>";


    private const string FileName = "content.txt";

    private const string Username = "user";

    private const string Password = "user123";

    public static void AddFormDataAction(Request request, Response response)
    {
        response.Body = "";

        foreach (var (key, value) in request.Form)
        {
            response.Body += $"{key} - {value}";
            response.Body += Environment.NewLine;
        }
    }

    private static async Task<string> DownloadWebSiteContent(string url)
    {
        HttpClient httpClient = new HttpClient();

        using (httpClient)
        {
            var response = await httpClient.GetAsync(url);
            string html = await response.Content.ReadAsStringAsync();

            return html.Substring(0, 2000);
        }
    }

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

    private static async Task DownloadSitesAsTextFile(string fileName, string[] urls)
    {
        List<Task<string>> downloads = new List<Task<string>>();

        foreach (string url in urls)
        {
            downloads.Add(DownloadWebSiteContent(url));
        }

        var responses = await Task.WhenAll(downloads);

        var responsesString = string.Join(Environment.NewLine + new string('-', 100), responses);

        await File.WriteAllTextAsync(fileName, responsesString);
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