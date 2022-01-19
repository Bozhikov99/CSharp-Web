using BasicWebServer.Server;
using BasicWebServer.Server.HTTP;
using BasicWebServer.Server.Responses;

public class Startup
{
    public static async Task Main()
    {
        await DownloadSitesAsTextFile(FileName, new string[] { "https://judge.softuni.org/", "https://softuni.org/" });

        await new HttpServer(routes => routes
        .MapGet("/", new TextResponse("Hello from the server!"))
        .MapGet("/HTML", new HtmlResponse(HtmlForm))
        .MapGet("/Redirect", new RedirectResponse("https://softuni.org/"))
        .MapPost("/HTML", new TextResponse("", AddFormDataAction))
        .MapGet("/Content", new HtmlResponse(DownloadForm))
         .MapPost("/Content", new TextFileResponse(FileName)))
         .Start();
    }
    private const string HtmlForm = @"<form action='/HTML' method='POST'>
Name: <input type='text' name='Name'/>
Age: <input type='text' name='Age'/>
<input type='submit' value='Save'>
</form>";

    private const string DownloadForm = @"<form action='/Content' method='POST'>
 <input type='submit' value='Download Sites Content' />
</form>";

    private const string FileName = "content.txt";

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
}