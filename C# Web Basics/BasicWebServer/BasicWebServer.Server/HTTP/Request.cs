using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.HTTP
{
    public class Request
    {
        public Method Method { get; private set; }

        public string Url { get; private set; }

        public HeaderCollection Headers { get; private set; }

        public string Body { get; private set; }

        public static Request Parse(string request)
        {
            string[] lines = request.Split("\r\n");

            string[] startLine = lines.First().Split(" ");

            Method method = ParseMethod(startLine[0]);
            string url = startLine[1];

            HeaderCollection headers = ParseHeaders(lines.Skip(1));
            string[] bodyLines = lines.Skip(headers.Count + 2).ToArray();
            string body = string.Join("\r\n", bodyLines);

            return new Request
            {
                Method = method,
                Url = url,
                Body = body,
                Headers = headers
            };
        }

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
