using BasicWebServer.Server.Controllers;
using BasicWebServer.Server.HTTP;
using System.Text;

namespace BasicWebServer.Demo.Controllers
{
    public class HomeController : Controller
    {
        private const string HtmlForm = @"<form action='/HTML' method='POST'>
Name: <input type='text' name='Name'/>
Age: <input type='text' name='Age'/>
<input type='submit' value='Save'>
</form>";

        private const string DownloadForm = @"<form action='/Content' method='POST'>
 <input type='submit' value='Download Sites Content' />
</form>";

        private const string FileName = "content.txt";

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

            await System.IO.File.WriteAllTextAsync(FileName, responsesString);
        }


        public HomeController(Request request)
            : base(request)
        {

        }
        public Response HtmlFormPost()
        {
            string formData = string.Empty;

            foreach (var (key, value) in request.Form)
            {
                formData += $"{key} - {value}";
                formData += Environment.NewLine;
            }

            return Text(formData);
        }

        public Response DownloadContent()
        {
            DownloadSitesAsTextFile(FileName,
                new string[] { "https://judge.softuni.org/", "https://softuni.org" })
                .Wait();

            return File(FileName);
        }

        public Response Index() => Text("Welcome to the server!");

        public Response Redirect() => Redirect("https://softuni.org");

        public Response Content() => Html(DownloadForm);

        public Response Html() => Html(HtmlForm);
    }
}
