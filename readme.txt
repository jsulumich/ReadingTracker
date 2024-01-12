Session outline 1/11/2024

    1) brief review to threading example I added to asnc code

    2) Introduce API programming by way of creating a use case for some API's.
    We'll do this by way of convert the book MVC app from direct data access in
    the controllers to an API backend call from:

        a) the existing controllers - this is a server side generated HTML
        apporach
        b) from the client side - this is a 'dynamic HTML' approach using
        API calls and java script.  We'll start with vanilla JavaScript but then
        touch briefly on JQuery which was used widely when this new approach of
        mixing server side and client side/dynsmic HTML

        Eventually we'll move to HTML generation from the the client side as we
        transition later into single page web apps e.g., SPA's

        Demo - new ApiBookController, things to note/discuss
                
                the base class ControllerBase,
                the ApiController attribute,
                the Route attribute, unlike non-API controllers routes are not
                    by "convention" butt by explicit rules/paths

                the controller's "Get" action method, things to note/discuss

                    the HttpGet attribute,
                    the return type, IActionResult, also async Task
                    the FromQuery attribute
                    the OK method, discuss all of the "helper" methods part of
                    Controller base for generation of ObjectResult which
                    supports returning data and a status code.  Walk through the
                    hierarchy via "F12"

        Now that we've got an API endpoint, let's discuss how to call it from
        the server side.  E.g.  HttpClient and all it's complexities.

        HttpClient class provides support for making API calls, but has caveats
        of socket exhaustion or failure to track DNS changes depending on how it
        is used.  This problem goes back to .NET full framework as well, see:

        https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/ 
        https://learn.microsoft.com/dotnet/fundamentals/networking/http/httpclient-guidelines#recommended-use
        https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory

            Discuss IDisposible, socket exhaustion and DNS issues
            find more links that got into details here

        Other Resources

            Pluralsight - Accessing APIs Using HttpClient in .NET 6  by Kevin
            Dockx

            https://andrewlock.net/exporing-the-code-behind-ihttpclientfactory/

        .NET provides a factory class to avoid both of these problems and is
        the preferred way to create HttpClient instances.  There are 3
        flavors.

            * creating clients
            * creating named clients
            * creating typed clients

      Creating clients (demo)

          builder.Services.AddHttpClient();

          Adding this line during your program startup means that any service that needs
          an HttpClient can "advertise" this to the built in ioc container by having a
          IHttpClientFactory dependency - in it's constructor for example.  The downside
          to this is the number of lines of code that are needed to make an API call.
          First you must instantiate an HttpClient using the factory, then you must
          configure the HttpClient with the URI that you want to make calls to.
          Tedious if this must be done in more than one place.

      Creating Named Clients (demo)

          builder.Services.AddHttpClient("name1", client => { // config })
          centralizes configuration, protentially reduces duplication

      Creating (HttpClient Wrapper/Adapter) Typed Clients (demo)