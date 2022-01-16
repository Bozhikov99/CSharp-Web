using BasicWebServer.Server;
using BasicWebServer.Server.HTTP;
using BasicWebServer.Server.Responses;

public class Startup
{
    public static void Main() => new HttpServer(routes => routes
       .MapGet("/", new TextResponse("Hello from the server!"))
   .MapGet("/HTML", new HtmlResponse(HtmlForm))
   .MapGet("/Redirect", new RedirectResponse("https://softuni.org/"))
   .MapPost("/HTML", new TextResponse("", AddFormDataAction)))
   .Start();

    private const string HtmlForm = @"<form action='/HTML' method='POST'>
Name: <input type='text' name='Name'/>
Age: <input type='text' name='Age'/>
<input type='submit' value='Save'>
</form>";

    public static void AddFormDataAction(Request request, Response response)
    {
        response.Body = "";

        foreach (var (key, value) in request.Form)
        {
            response.Body += $"{key} - {value}";
            response.Body += Environment.NewLine;
        }
    }
}