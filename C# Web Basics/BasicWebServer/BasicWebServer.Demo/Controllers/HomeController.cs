using BasicWebServer.Server.Controllers;
using BasicWebServer.Server.HTTP;
using System.Text;
using System.Web;

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

        public Response Cookies()
        {
            if (request.Cookies.Any(c => c.Name != Server.HTTP.Session.SessionCookieName))
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

                return Html(cookieText.ToString());
            }

            CookieCollection cookies = new CookieCollection();
            cookies.Add("My-Cookie", "My-Value");
            cookies.Add("My-Second-Cookie", "My-Second-Value");

            return Html("<h1>Cookies set!</h1>", cookies);
        }

        public Response DownloadContent()
        {
            DownloadSitesAsTextFile(FileName,
                new string[] { "https://judge.softuni.org/", "https://softuni.org" })
                .Wait();

            return File(FileName);
        }

        public Response Session()
        {
            string currentDateKey = "CurrentDate";

            bool sessionExists = request.Session
                .ContainsKey(currentDateKey);

            if (sessionExists)
            {
                var currentDate = request.Session[currentDateKey];

                return Text($"Stored date: {currentDate}");
            }

            return Text("Current date stored!");
        }

        public Response Index() => Text("Welcome to the server!");

        public Response Redirect() => Redirect("https://softuni.org");

        public Response Content() => Html(DownloadForm);

        public Response Html() => Html(HtmlForm);
    }
}
