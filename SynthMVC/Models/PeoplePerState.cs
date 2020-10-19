using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SynthMVC.DAL;
using SynthMVC.Models;

namespace SynthMVC.Models
{
    public class PeoplePerState // todo change name to something like GeneratedStateStats
    {
        [Key]
        public States State { get; set; }

        public int Population { get; set; }

        public int TotalSexMale { get; set; }
        public int TotalSexFemale { get; set; }

        public int TotalAgeUnder5 { get; set; }
        public int TotalAge5To9 { get; set; }
        public int TotalAge10To14 { get; set; }
        public int TotalAge15To19 { get; set; }
        public int TotalAge20To24 { get; set; }
        public int TotalAge25To34 { get; set; }
        public int TotalAge35To44 { get; set; }
        public int TotalAge45To54 { get; set; }
        public int TotalAge55To59 { get; set; }
        public int TotalAge60To64 { get; set; }
        public int TotalAge65To74 { get; set; }
        public int TotalAge75To84 { get; set; }
        public int TotalAge85OrOver { get; set; }

        public int TotalRaceWhite { get; set; }
        public int TotalRaceBlack { get; set; }
        public int TotalRaceAmericanIndianOrAlaskan { get; set; }
        public int TotalRaceAsian { get; set; }
        public int TotalRaceNativeHawaiianOrPacificIslander { get; set; }
        public int TotalRaceOther { get; set; }
        public int TotalRaceWhiteAndBlack { get; set; }
        public int TotalRaceWhiteAndAmericanIndianOrAlaskan { get; set; }
        public int TotalRaceWhiteAndAsian { get; set; }
        public int TotalRaceBlackAndAmericanIndianOrAlaskan { get; set; }

        public int TotalIncomeLessThan10k { get; set; }
        public int TotalIncome10kTo15k { get; set; }
        public int TotalIncome15kTo25k { get; set; }
        public int TotalIncome25kTo35k { get; set; }
        public int TotalIncome35kTo50k { get; set; }
        public int TotalIncome50kTo75k { get; set; }
        public int TotalIncome75kTo100k { get; set; }
        public int TotalIncome100kTo150k { get; set; }
        public int TotalIncome150kTo200k { get; set; }
        public int TotalIncomeOver200k { get; set; }


        public PeoplePerState()
        {

        }
    }
}