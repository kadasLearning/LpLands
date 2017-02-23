using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LandsDepartment.Models
{
    public class NewsViewModel
    {
        public int NewsID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public HttpPostedFileBase imgNews { get; set; }
     
    }
}