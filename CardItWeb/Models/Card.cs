using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CardItWebApp.Models
{
    public class Card
    {
        public int Id { get; set; }

        public int userId { get; set; }

        public int merchantId { get; set; }

        public string CardNumber { get; set; }

    }
}