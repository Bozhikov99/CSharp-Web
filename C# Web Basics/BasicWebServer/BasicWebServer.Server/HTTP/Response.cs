using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.HTTP
{
    public class Response
    {
        public Response(StatusCode code)
        {
            StatusCode = code;
            Headers.Add(Header.Server, "My Web Server");
            Headers.Add(Header.Date, $"{DateTime.UtcNow:r}");
        }

        public StatusCode StatusCode { get; init; }

        public HeaderCollection Headers { get; set; } = new HeaderCollection();

        public string Body { get; set; }

        public Action<Request, Response> PreRenderAction { get; protected set; }
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"HTTP/1.1 {(int)StatusCode} {StatusCode}");

            foreach (var header in Headers)
            {
                stringBuilder.AppendLine(header.ToString());
            }

            stringBuilder.AppendLine();

            if (!string.IsNullOrEmpty(Body))
            {
                stringBuilder.Append(Body);
            }

            return stringBuilder.ToString();
        }
    }
}
