using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;
namespace WebApplication1
{
    public class viewmodel
    {
        public string JobTitle { get; set; }
        public IEnumerable<ApplyforJob>Items{ get; set; }
    }
}