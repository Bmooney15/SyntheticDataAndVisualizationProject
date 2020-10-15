using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using SynthMVC.DAL;

namespace SynthMVC.Models
{
    public class Person
    {
        [Key]
        public int PersonID { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public GenderTypes Gender { get; set; }
        public RaceTypes Race { get; set; }
        public int Income { get; set; }
        public States State { get; set; }

        public Person() // parameterless constructor (any entity class which want to be included in DbSet<T> type parameter must have parameterless constructor to enable binding with EF)
        {

        }

        public Person(DataTable demographics, DataTable Surnames, Random random, States stateName) // passing random to avoid reusing seed due to tight loop/clock cycles TODO come up with better solution
        {
            int rowNum = 0;
            DataRow dr = Surnames.Rows[random.Next(0, Surnames.Rows.Count)];
            LastName = dr["NAME"].ToString();

            for (int i = 0; i < 52; i++) // iterate through rows of census table
            {
                if ((string)demographics.Rows[i][0] == EnumExtensions.GetEnumDescription((States)stateName))  // if current row matches the state passed by stateName, use this row to calculate cdf for the person
                {
                    rowNum = i;
                }
            }

            double[] agePercentages = { Convert.ToDouble(demographics.Rows[rowNum][4]), Convert.ToDouble(demographics.Rows[rowNum][5]),
                Convert.ToDouble(demographics.Rows[rowNum][6]), Convert.ToDouble(demographics.Rows[rowNum][7]), Convert.ToDouble(demographics.Rows[rowNum][8]), Convert.ToDouble(demographics.Rows[rowNum][9]),
                Convert.ToDouble(demographics.Rows[rowNum][10]), Convert.ToDouble(demographics.Rows[rowNum][11]), Convert.ToDouble(demographics.Rows[rowNum][12]), Convert.ToDouble(demographics.Rows[rowNum][13]),
                Convert.ToDouble(demographics.Rows[rowNum][14]), Convert.ToDouble(demographics.Rows[rowNum][15]), Convert.ToDouble(demographics.Rows[rowNum][16]) }; // convert datatable age values to doubles and place in double[]

            double[] ageCdf = GetData.getCdf(agePercentages); // generate cumulative density function from agePercentages, to allow for random selection of ages
            Age = getAgeFromCdf(ageCdf, random);

            double[] racePercentages = { Convert.ToDouble(demographics.Rows[rowNum][17]), Convert.ToDouble(demographics.Rows[rowNum][18]),
                Convert.ToDouble(demographics.Rows[rowNum][19]), Convert.ToDouble(demographics.Rows[rowNum][20]), Convert.ToDouble(demographics.Rows[rowNum][21]), Convert.ToDouble(demographics.Rows[rowNum][22]),
                Convert.ToDouble(demographics.Rows[rowNum][23]), Convert.ToDouble(demographics.Rows[rowNum][24]), Convert.ToDouble(demographics.Rows[rowNum][25]), Convert.ToDouble(demographics.Rows[rowNum][26])};
            double[] raceCdf = GetData.getCdf(racePercentages);
            Race = getRaceFromCdf(raceCdf, random);

            double genderPercent = random.NextDouble();
            if (genderPercent <= (Convert.ToDouble(demographics.Rows[rowNum][2]) / 100)) // use % male or female to generate gender
            {
                Gender = GenderTypes.Male;
            }
            else
            {
                Gender = GenderTypes.Female;
            }

            if (Age >= 16) // calculate income for those of working age (>=16yo)
            {
                double[] incomePercentages = { Convert.ToDouble(demographics.Rows[rowNum][27]), Convert.ToDouble(demographics.Rows[rowNum][28]),
                Convert.ToDouble(demographics.Rows[rowNum][29]), Convert.ToDouble(demographics.Rows[rowNum][30]), Convert.ToDouble(demographics.Rows[rowNum][31]), Convert.ToDouble(demographics.Rows[rowNum][32]),
                Convert.ToDouble(demographics.Rows[rowNum][33]), Convert.ToDouble(demographics.Rows[rowNum][34]), Convert.ToDouble(demographics.Rows[rowNum][35]), Convert.ToDouble(demographics.Rows[rowNum][36]) };
                double[] incomeCdf = GetData.getCdf(incomePercentages);
                Income = getIncomeFromCdf(incomeCdf, random);
            }

            State = stateName;
        }

        // returns cdf for age brackets
        public static int getAgeFromCdf(double[] ageCdf, Random random)
        {
            int ageInt = 0;
            double age = random.NextDouble();

            while (age > ageCdf[12]) // case when statistics don't add up to 100% due to margin of error and cdf doesn't go to 1.0, reroll
            {
                age = random.NextDouble();
            }

            if (age <= ageCdf[0])
            {
                ageInt = random.Next(0, 4);
            }
            else if (ageCdf[0] < age && age <= ageCdf[1])
            {
                ageInt = 5 + random.Next(0, 4);
            }
            else if (ageCdf[1] < age && age <= ageCdf[2])
            {
                ageInt = 10 + random.Next(0, 4);
            }
            else if (ageCdf[2] < age && age <= ageCdf[3])
            {
                ageInt = 15 + random.Next(0, 4);
            }
            else if (ageCdf[3] < age && age <= ageCdf[4])
            {
                ageInt = 20 + random.Next(0, 4);
            }
            else if (ageCdf[4] < age && age <= ageCdf[5])
            {
                ageInt = 25 + random.Next(0, 4);
            }
            else if (ageCdf[5] < age && age <= ageCdf[6])
            {
                ageInt = 35 + random.Next(0, 4);
            }
            else if (ageCdf[6] < age && age <= ageCdf[7])
            {
                ageInt = 45 + random.Next(0, 4);
            }
            else if (ageCdf[7] < age && age <= ageCdf[8])
            {
                ageInt = 55 + random.Next(0, 4);
            }
            else if (ageCdf[8] < age && age <= ageCdf[9])
            {
                ageInt = 60 + random.Next(0, 4);
            }
            else if (ageCdf[9] < age && age <= ageCdf[10])
            {
                ageInt = 65 + random.Next(0, 4);
            }
            else if (ageCdf[10] < age && age <= ageCdf[11])
            {
                ageInt = 75 + random.Next(0, 4);
            }
            else if (ageCdf[11] < age && age <= ageCdf[12])
            {
                ageInt = 85 + random.Next(0, 15);
            }
            return ageInt;
        }

        // returns cdf for race
        public static RaceTypes getRaceFromCdf(double[] raceCdf, Random random)
        {

            double race = random.NextDouble();

            while (race > raceCdf[9]) // case when statistics don't add up to 100% due to margin of error and cdf doesn't go to 1.0, reroll
            {
                race = random.NextDouble();
            }

            if (race <= raceCdf[0])
            {
                return RaceTypes.White;
            }
            else if (raceCdf[0] < race && race <= raceCdf[1])
            {
                return RaceTypes.Black;
            }
            else if (raceCdf[1] < race && race <= raceCdf[2])
            {
                return RaceTypes.AmericanIndianOrAlaskan;
            }
            else if (raceCdf[2] < race && race <= raceCdf[3])
            {
                return RaceTypes.Asian;
            }
            else if (raceCdf[3] < race && race <= raceCdf[4])
            {
                return RaceTypes.NativeHawaiianOrPacificIslander;
            }
            else if (raceCdf[4] < race && race <= raceCdf[5])
            {
                return RaceTypes.Other;
            }
            else if (raceCdf[5] < race && race <= raceCdf[6])
            {
                return RaceTypes.WhiteAndBlack;
            }
            else if (raceCdf[6] < race && race <= raceCdf[7])
            {
                return RaceTypes.WhiteAndAmericanIndianOrAlaskan;
            }
            else if (raceCdf[7] < race && race <= raceCdf[8])
            {
                return RaceTypes.WhiteAndAsian;
            }
            else
            {
                return RaceTypes.BlackAndAmericanIndianOrAlaskan;
            }

        }

        // returns cdf for income brackets
        public static int getIncomeFromCdf(double[] incomeCdf, Random random)
        {
            int incomeInt = 0;
            double income = random.NextDouble();

            while (income > incomeCdf[9]) // case when statistics don't add up to 100% due to margin of error and cdf doesn't go to 1.0, reroll
            {
                income = random.NextDouble();
            }

            if (income <= incomeCdf[0])
            {
                incomeInt = random.Next(0, 9999);
            }
            else if (incomeCdf[0] < income && income <= incomeCdf[1])
            {
                incomeInt = 10000 + random.Next(0, 4999);
            }
            else if (incomeCdf[1] < income && income <= incomeCdf[2])
            {
                incomeInt = 15000 + random.Next(0, 9999);
            }
            else if (incomeCdf[2] < income && income <= incomeCdf[3])
            {
                incomeInt = 25000 + random.Next(0, 9999);
            }
            else if (incomeCdf[3] < income && income <= incomeCdf[4])
            {
                incomeInt = 35000 + random.Next(0, 14999);
            }
            else if (incomeCdf[4] < income && income <= incomeCdf[5])
            {
                incomeInt = 50000 + random.Next(0, 24999);
            }
            else if (incomeCdf[5] < income && income <= incomeCdf[6])
            {
                incomeInt = 75000 + random.Next(0, 24999);
            }
            else if (incomeCdf[6] < income && income <= incomeCdf[7])
            {
                incomeInt = 100000 + random.Next(0, 49999);
            }
            else if (incomeCdf[7] < income && income <= incomeCdf[8])
            {
                incomeInt = 150000 + random.Next(0, 49999);
            }
            else if (incomeCdf[8] < income && income <= incomeCdf[9])
            {
                incomeInt = 200000; // + random.Next(0, 1000000); // todo make function to calculate incomes over 200k somewhat accurately, logarithm or something, econ function for this already exists?
            }
            return incomeInt;
        }


    }
}