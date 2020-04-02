﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Web;

namespace VirtualPAS.Models
{
 
    public class Result
    {
        public int Id { get; set; }
        public string RowStatus { get; set; }
        public string Year { get; set; }
        public string Series { get; set; }
        public string Day { get; set; }
        public string CourseDate { get; set; }
        public string MapName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Course { get; set; }
        public string StartTime { get; set; }
        public string FinishTime { get; set; }
        public string CompletedTime { get; set; }
        public string NumberOfControls { get; set; }
        public string Status { get; set; }
        public string NumberOfTwoPoints { get; set; }
        public string NumberOfThreePoints { get; set; }
        public string NumberOfFourPoints { get; set; }
        public string NumberOfFivePoints { get; set; }
        public string NumberOfSixPoints { get; set; }
        public string NumberLateMinutes { get; set; }
        public string Points { get; set; }
        public string Distance { get; set; }

    }
}
