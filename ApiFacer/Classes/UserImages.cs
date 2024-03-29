﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ConsoleFaiserScript.Classes
{
    public class UserImages
    {
        [Key]
        public int id { get; set; }
        public int userId { get; set; }
        public string yandexPath { get; set; }
        public string localPath { get; set; }
        public int eventId { get; set; }
    }

    public class PersonVk
    {
        [Key]
        public int userImagesId { get; set; }
        public string tag { get; set; }
        [NotMapped]
        public int[] coord { get; set; }
        public double confidence { get; set; }
        public double awesomeness { get; set; }
        public double similarity { get; set; }
        public string sex { get; set; }
        public string emotion { get; set; }
        public int age { get; set; }
        public double valence { get; set; }
        public double arousal { get; set; }
        public double frontality { get; set; }
        public double visibility { get; set; }
    }
}
