namespace Server.Admin.Attributes
{
    using System;
    using System.Net;
    using System.Web;
    using System.Web.Caching;
    using System.Web.Mvc;

    /// <summary>
    /// Decorates any MVC route that needs to have client requests limited by time.
    /// </summary>
    /// <remarks>
    /// Uses the current System.Web.Caching.Cache to store each client request to the decorated route.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ThrottleAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Gets or sets a unique name for this Throttle.
        /// </summary>
        /// <remarks>
        /// We'll be inserting a Cache record based on this name and client IP, e.g. "Name-192.168.0.1"
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the number of seconds clients must wait before executing this decorated route again.
        /// </summary>
        public int Seconds { get; set; }

        /// <summary>
        /// Hook onto the action executing event.
        /// </summary>
        /// <param name="actionContext">The actions context.</param>
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            Guard.NotNull(() => actionContext);

            var key = string.Concat(this.Name, "-", actionContext.HttpContext.Request.UserHostAddress);

            var allowExecute = false;

            if (HttpRuntime.Cache.Get(key) == null)
            {
                HttpRuntime.Cache.Add(
                    key,
                    true, // is this the smallest data we can have?
                    null, // no dependencies
                    DateTime.Now.AddSeconds(this.Seconds), // absolute expiration
                    Cache.NoSlidingExpiration,
                    CacheItemPriority.Low,
                    null); // no callback

                allowExecute = true;
            }

            if (!allowExecute)
            {
                throw new HttpException(420, "You have exceded the login throttle limit, please try again shortly.");
            }
        }
    }
}