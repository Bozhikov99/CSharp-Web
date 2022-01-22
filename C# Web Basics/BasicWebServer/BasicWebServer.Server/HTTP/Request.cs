using System.Web;

namespace BasicWebServer.Server.HTTP
{
    public class Request
    {
        private static Dictionary<string, Session> Sessions = new();

        public Method Method { get; private set; }

        public string Url { get; private set; }

        public HeaderCollection Headers { get; private set; }

        public CookieCollection Cookies { get; private set; }

        public string Body { get; private set; }

        public Session Session { get; private set; }

        public IReadOnlyDictionary<string, string> Form { get; private set; }

        public static Request Parse(string request)
        {
            string[] lines = request.Split("\r\n");

            string[] startLine = lines.First().Split(" ");

            Method method = ParseMethod(startLine[0]);
            string url = startLine[1];

            HeaderCollection headers = ParseHeaders(lines.Skip(1));
            string[] bodyLines = lines.Skip(headers.Count + 2).ToArray();
            string body = string.Join("\r\n", bodyLines);

            CookieCollection cookies = ParseCookies(headers);
            Session session = GetSession(cookies);

            var form = ParseForm(headers, body);

            return new Request
            {
                Method = method,
                Url = url,
                Body = body,
                Headers = headers,
                Cookies = cookies,
                Session = session,
                Form = form,
            };
        }

        private static Session GetSession(CookieCollection cookies)
        {
            string sessionId = cookies.Contains(Session.SessionCookieName)
                ? cookies[Session.SessionCookieName]
                : Guid.NewGuid().ToString();


            if (!Sessions.ContainsKey(Session.SessionCookieName))
            {
                Sessions[sessionId] = new Session(sessionId);
            }

            return Sessions[sessionId];
        }

        private static CookieCollection ParseCookies(HeaderCollection headers)
        {
            CookieCollection cookies = new CookieCollection();

            if (headers.Contains(Header.Cookie))
            {
                string cookieHeader = headers[Header.Cookie];
                string[] allCookies = cookieHeader.Split(';');

                foreach (string cookieText in allCookies)
                {
                    string[] cookieParts = cookieText.Split('=');

                    string cookieName = cookieParts[0].Trim();
                    string cookieValue = cookieParts[1].Trim();

                    cookies.Add(cookieName, cookieValue);
                }
            }

            return cookies;
        }

        private static Dictionary<string, string> ParseForm(HeaderCollection headers, string body)
        {
            var formCollection = new Dictionary<string, string>();

            if (headers.Contains(Header.ContentType) &&
                headers[Header.ContentType] == ContentType.FormUrlEncoded)
            {
                var parsedResult = ParseFormData(body);

                foreach (var (name, value) in parsedResult)
                {
                    formCollection.Add(name, value);
                }
            }

            return formCollection;
        }

        private static Dictionary<string, string> ParseFormData(string bodyLines)
            => HttpUtility.UrlDecode(bodyLines)
            .Split('&')
            .Select(part => part.Split('='))
            .Where(part => part.Length == 2)
            .ToDictionary(
                part => part[0],
                part => part[1],
                StringComparer.InvariantCultureIgnoreCase);

        private static Method ParseMethod(string method)
        {
            try
            {
                return Enum.Parse<Method>(method);
            }
            catch (Exception)
            {
                throw new InvalidOperationException($"Method {method} is not supported");
            }
        }

        private static HeaderCollection ParseHeaders(IEnumerable<string> lines)
        {
            HeaderCollection headers = new HeaderCollection();

            foreach (string line in lines)
            {
                if (line == string.Empty)
                {
                    break;
                }

                string[] headerParts = line.Split(":", 2);

                if (headerParts.Length != 2)
                {
                    throw new InvalidOperationException("Request is not valid");
                }

                string headerName = headerParts[0];
                string headerValue = headerParts[1];

                headers.Add(headerName, headerValue);
            }

            return headers;
        }
    }
}
