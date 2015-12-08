using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ButlerSite.Models
{
    public class InformationViewModel
    {
        [Display(Name = "Images")]
        public IEnumerable<Uri> Images { get; set; }
    }
}
