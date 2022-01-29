﻿using BasicWebServer.Server.Controllers;
using BasicWebServer.Server.HTTP;

namespace BasicWebServer.Demo.Controllers
{
    public class UsersController : Controller
    {
        private const string LoginForm = @"<form action='/Login' method='POST'>
   Username: <input type='text' name='Username'/>
   Password: <input type='text' name='Password'/>
   <input type='submit' value ='Log In' /> 
</form>";


        private const string Username = "user";

        private const string Password = "user123";

        public UsersController(Request request)
            : base(request)
        {

        }

        public Response Login() => Html(LoginForm);

        public Response Logout()
        {
            request.Session.Clear();

            return Html("<h3>Logged out successfully!</h3>");
        }

        public Response LogInUser()
        {
            request.Session.Clear();

            var usernameMatches = request.Form["Username"] == Username;
            var passwordMatches = request.Form["Password"] == Password;

            if (usernameMatches && passwordMatches)
            {
                if (!request.Session.ContainsKey(Session.SessionUserKey))
                {
                    request.Session[Session.SessionUserKey] = "MyUserId";

                    CookieCollection cookies = new CookieCollection();
                    cookies.Add(Session.SessionCookieName, request.Session.Id);

                    return Html("<h3>Logged successfully!</h3>", cookies);
                }

                return Html("<h3>Logged successfully!</h3>");
            }

            return Redirect("/Login");
        }

        public Response GetUserDataAction()
        {
            if (request.Session.ContainsKey(Session.SessionUserKey))
            {
                return Html($"<h3>Currently logged-in user is with username: {Username}</h3>");
            }

            return Redirect("/Login");
        }
    }
}