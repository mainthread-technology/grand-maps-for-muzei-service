namespace Server.Admin.Models
{
    using Server.Framework.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class PagedModel<T>
    {
        public IEnumerable<T> Items { get; set; }

        public int ItemsTotal { get; set; }
    }
}