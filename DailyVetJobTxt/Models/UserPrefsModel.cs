using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DailyVetJobTxt.Models
{
    public class UserPrefsModel
    {
        [Required]
        public int ZipCode { get; set; }

        [Required]
        public string KeyWords { get; set; }
    }
}