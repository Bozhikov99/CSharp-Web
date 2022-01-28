using BasicWebServer.Server.HTTP;
using BasicWebServer.Server.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.Controllers
{
    public class Controller
    {
        protected Request request { get; private init; }

        public Controller(Request request)
        {
            this.request = request;
        }

        protected Response Text(string text) => new TextResponse(text);
        protected Response Html(string html, CookieCollection cookies = null)
        {
            Response response = new HtmlResponse(html);

            if (cookies != null)
            {
                foreach (var cookie in cookies)
                {
                    response.Cookies.Add(cookie.Name, cookie.Value);
                }
            }

            return response;
        }
        protected Response BadRequest() => new BadRequestResponse();
        protected Response Unauthorized() => new UnauthorizedResponse();
        protected Response NotFound() => new NotFoundResponse();
        protected Response File(string fileName) => new FileResponse(fileName);
        protected Response Redirect(string location) => new RedirectResponse(location);
    }
}
