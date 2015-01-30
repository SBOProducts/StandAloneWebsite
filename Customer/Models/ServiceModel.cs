using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Customer.Models
{
    public class Service
    {
        public int ID { get; set; }

        [Required]
        [MaxLength(60)]
        [Display(Name = "Menu Name")]
        public string MenuName { get; set; }

        /// <summary>
        /// usually the MenuName with spaces and special characters converted to dashes, but can
        /// be typed in by the user
        /// </summary>
        public string Url { get; set; }

        [Required]
        [Display(Name = "Page Title")]
        public string PageTitle { get; set; }

        /// <summary>
        /// for page meta data description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// for page meta data key words
        /// </summary>
        [Display(Name = "Key Words")]
        public string KeyWords { get; set; }

        // public or private
        [Display(Name = "Keep this page private?")]
        public bool IsPrivate { get; set; }

        public Guid ContentID { get; set; }

        public int Sequence { get; set; }

    }
}