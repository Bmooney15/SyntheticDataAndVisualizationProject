using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using SynthMVC.DAL;
using SynthMVC.Models;

namespace SynthMVC.Models
{
    public class StateData 
    {
        [Key]
        public States State { get; set; }

        public int Population { get; set; }

        public float PercentSexMale { get; set; }
        public float PercentSexFemale { get; set; }

        public float PercentAgeUnder5 { get; set; }
        public float PercentAge5To9 { get; set; }
        public float PercentAge10To14 { get; set; }
        public float PercentAge15To19 { get; set; }
        public float PercentAge20To24 { get; set; }
        public float PercentAge25To34 { get; set; }
        public float PercentAge35To44 { get; set; }
        public float PercentAge45To54 { get; set; }
        public float PercentAge55To59 { get; set; }
        public float PercentAge60To64 { get; set; }
        public float PercentAge65To74 { get; set; }
        public float PercentAge75To84 { get; set; }
        public float PercentAge85OrOver { get; set; }
        
        public float PercentRaceWhite { get; set; }
        public float PercentRaceBlack { get; set; }
        public float PercentRaceAmericanIndianOrAlaskan { get; set; }
        public float PercentRaceAsian { get; set; }
        public float PercentRaceNativeHawaiianOrPacificIslander { get; set; }
        public float PercentRaceOther { get; set; }
        public float PercentRaceWhiteAndBlack { get; set; }
        public float PercentRaceWhiteAndAmericanIndianOrAlaskan { get; set; }
        public float PercentRaceWhiteAndAsian { get; set; }
        public float PercentRaceBlackAndAmericanIndianOrAlaskan { get; set; }

        public float PercentIncomeLessThan10k { get; set; }
        public float PercentIncome10kTo15k { get; set; }
        public float PercentIncome15kTo25k { get; set; }
        public float PercentIncome25kTo35k { get; set; }
        public float PercentIncome35kTo50k { get; set; }
        public float PercentIncome50kTo75k { get; set; }
        public float PercentIncome75kTo100k { get; set; }
        public float PercentIncome100kTo150k { get; set; }
        public float PercentIncome150kTo200k { get; set; }
        public float PercentIncomeOver200k { get; set; }


        public StateData()
        {

        }

    }
}