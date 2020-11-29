using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Models
{

    [Bind("Name")]
    public class Pets
    {
        public string Name { get; set; }

        [NotMapped]
        public string Message { get; set; }
    }
}
