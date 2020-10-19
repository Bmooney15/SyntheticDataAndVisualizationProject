using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using SynthMVC.Models;
using System.Data.Entity;

namespace SynthMVC.DAL
{
    public class PersonInitializer : DropCreateDatabaseAlways<PersonContext>
    {

        protected override void Seed(PersonContext context)
        {
            // TODO get state data
            string url = "https://api.census.gov/data/2018/acs/acs1/profile?get=NAME,DP05_0001E,DP05_0002PE,DP05_0003PE,DP05_0005PE,DP05_0006PE,DP05_0007PE,DP05_0008PE,DP05_0009PE," +
                         "DP05_0010PE,DP05_0011PE,DP05_0012PE,DP05_0013PE,DP05_0014PE,DP05_0015PE,DP05_0016PE,DP05_0017PE,DP05_0037PE,DP05_0038PE,DP05_0039PE,DP05_0044PE,DP05_0052PE,DP05_0057PE," +
                         "DP05_0059PE,DP05_0060PE,DP05_0061PE,DP05_0062PE,DP03_0052PE,DP03_0053PE,DP03_0054PE,DP03_0055PE,DP03_0056PE,DP03_0057PE,DP03_0058PE,DP03_0059PE," +
                         "DP03_0060PE,DP03_0061PE&for=state:*"; // get selected statistics from 2018 American Community Survey (breakdown by gender, age, race, and income)
            string jsonUrl = "https://api.census.gov/data/2018/acs/acs1/profile/variables.json"; // url to get variable names for statistics
            DataTable dtDemographics = GetData.getData(url, jsonUrl); // parse census data into DataTable

            StateInitialization(dtDemographics).ForEach(s => context.States.Add(s)); // create a list of states and their actual demographic and economic statistics, used later to create a visualization to compare against generated data

            List<Person> people = PersonGenerator(2500, dtDemographics);
            people.ForEach(p => context.People.Add(p)); // generate a table of people based on census statistics

            CalculateGeneratedPopulationStats(people).ForEach(s => context.GeneratedStateStats.Add(s)); // create a list of states and their generated demographic and economic statistics, used later to create a visualization to compare against actual data

            context.SaveChanges();
        }

        private List<StateData> StateInitialization(DataTable dtDemographics) // create a list of state objects, initialize objects with data from census website
        {
            List<StateData> stateList = new List<StateData>(52);
            int count = 1;
            while(count < 52) // TODO optimize this search
            {
                for (int i = 0; i < 52; i++) // iterate through rows of census table
                {
                    if ((string)dtDemographics.Rows[i][0] == EnumExtensions.GetEnumDescription((States)count))  // if current row matches the state passed by stateName, use this row to calculate cdf for the person
                    {
                        StateData temp = new StateData
                        {
                            State = (States)count,
                            Population = int.Parse((string)dtDemographics.Rows[i][1]),
                            PercentSexMale = float.Parse((string)dtDemographics.Rows[i][2]),
                            PercentSexFemale = float.Parse((string)dtDemographics.Rows[i][3]),
                            PercentAgeUnder5 = float.Parse((string)dtDemographics.Rows[i][4]),
                            PercentAge5To9 = float.Parse((string)dtDemographics.Rows[i][5]),
                            PercentAge10To14 = float.Parse((string)dtDemographics.Rows[i][6]),
                            PercentAge15To19 = float.Parse((string)dtDemographics.Rows[i][7]),
                            PercentAge20To24 = float.Parse((string)dtDemographics.Rows[i][8]),
                            PercentAge25To34 = float.Parse((string)dtDemographics.Rows[i][9]),
                            PercentAge35To44 = float.Parse((string)dtDemographics.Rows[i][10]),
                            PercentAge45To54 = float.Parse((string)dtDemographics.Rows[i][11]),
                            PercentAge55To59 = float.Parse((string)dtDemographics.Rows[i][12]),
                            PercentAge60To64 = float.Parse((string)dtDemographics.Rows[i][13]),
                            PercentAge65To74 = float.Parse((string)dtDemographics.Rows[i][14]),
                            PercentAge75To84 = float.Parse((string)dtDemographics.Rows[i][15]),
                            PercentAge85OrOver = float.Parse((string)dtDemographics.Rows[i][16]),
                            PercentRaceWhite = float.Parse((string)dtDemographics.Rows[i][17]),
                            PercentRaceBlack = float.Parse((string)dtDemographics.Rows[i][18]),
                            PercentRaceAmericanIndianOrAlaskan = float.Parse((string)dtDemographics.Rows[i][19]),
                            PercentRaceAsian = float.Parse((string)dtDemographics.Rows[i][20]),
                            PercentRaceNativeHawaiianOrPacificIslander = float.Parse((string)dtDemographics.Rows[i][21]),
                            PercentRaceOther = float.Parse((string)dtDemographics.Rows[i][22]),
                            PercentRaceWhiteAndBlack = float.Parse((string)dtDemographics.Rows[i][23]),
                            PercentRaceWhiteAndAmericanIndianOrAlaskan = float.Parse((string)dtDemographics.Rows[i][24]),
                            PercentRaceWhiteAndAsian = float.Parse((string)dtDemographics.Rows[i][25]),
                            PercentRaceBlackAndAmericanIndianOrAlaskan = float.Parse((string)dtDemographics.Rows[i][26]),
                            PercentIncomeLessThan10k = float.Parse((string)dtDemographics.Rows[i][27]),
                            PercentIncome10kTo15k = float.Parse((string)dtDemographics.Rows[i][28]),
                            PercentIncome15kTo25k = float.Parse((string)dtDemographics.Rows[i][29]),
                            PercentIncome25kTo35k = float.Parse((string)dtDemographics.Rows[i][30]),
                            PercentIncome35kTo50k = float.Parse((string)dtDemographics.Rows[i][31]),
                            PercentIncome50kTo75k = float.Parse((string)dtDemographics.Rows[i][32]),
                            PercentIncome75kTo100k = float.Parse((string)dtDemographics.Rows[i][33]),
                            PercentIncome100kTo150k = float.Parse((string)dtDemographics.Rows[i][34]),
                            PercentIncome150kTo200k = float.Parse((string)dtDemographics.Rows[i][35]),
                            PercentIncomeOver200k = float.Parse((string)dtDemographics.Rows[i][36])
                        };
                        stateList.Add(temp);
                        
                    }
                }
                count++;
            }
            return stateList;
        }

        private List<Person> PersonGenerator(int personCount, DataTable dtDemographics)
        {
            string urlSurnames = "https://api.census.gov/data/2010/surname?get=NAME,COUNT,PCT2PRACE,PCTAIAN,PCTAPI,PCTBLACK,PCTHISPANIC,PCTWHITE,CUM_PROP100K,PROP100K&RANK=1:1000"; // get 1000 most popular surnames
           // string url = "https://api.census.gov/data/2018/acs/acs1/profile?get=NAME,DP05_0001E,DP05_0002PE,DP05_0003PE,DP05_0005PE,DP05_0006PE,DP05_0007PE,DP05_0008PE,DP05_0009PE," +
           //              "DP05_0010PE,DP05_0011PE,DP05_0012PE,DP05_0013PE,DP05_0014PE,DP05_0015PE,DP05_0016PE,DP05_0017PE,DP05_0037PE,DP05_0038PE,DP05_0039PE,DP05_0044PE,DP05_0052PE,DP05_0057PE," +
           //              "DP05_0059PE,DP05_0060PE,DP05_0061PE,DP05_0062PE,DP03_0052PE,DP03_0053PE,DP03_0054PE,DP03_0055PE,DP03_0056PE,DP03_0057PE,DP03_0058PE,DP03_0059PE," +
           //              "DP03_0060PE,DP03_0061PE&for=state:*"; // get selected statistics from 2018 American Community Survey (breakdown by gender, age, race, and income)
           // string jsonUrl = "https://api.census.gov/data/2018/acs/acs1/profile/variables.json"; // url to get variable names for statistics

            //DataTable dtDemographics = GetData.getData(url, jsonUrl); // parse census data into DataTable
            DataTable dtSurnames = GetData.getSurnames(urlSurnames); // parse census surname data into DataTable

            Random random = new Random();

            // demographics.Rows[0][1] == population
            States[] stateList = { (States)1, (States)2, (States)3, (States)4, (States)5, (States)6, (States)7, (States)8, (States)9, (States)10, (States)11, (States)12, (States)13,
                (States)14, (States)15, (States)16, (States)17, (States)18, (States)19, (States)20, (States)21, (States)22, (States)23, (States)24, (States)25, (States)26, (States)27,
                (States)28, (States)29, (States)30, (States)31, (States)32, (States)33, (States)34, (States)35, (States)36, (States)37, (States)38, (States)39, (States)40, (States)42,
                (States)43, (States)44, (States)45, (States)46, (States)47, (States)48, (States)49, (States)50, (States)51, (States)52 };

            //States[] stateList = { States.WY, States.CA, States.MA, States.TX };

            int[] popCounts = new int[stateList.Length];

            for (int j = 0; j < stateList.Length; j++)
            {

                int rowNum = GetData.getRowFromState(stateList[j], dtDemographics);
                popCounts[j] = int.Parse(dtDemographics.Rows[rowNum][1].ToString());
            }
            double[] probabilities = GetData.getPopProbabilities(popCounts);
            double[] populationCdf = GetData.getCdf(probabilities);

            List<Person> people = new List<Person>();

            for(int i = 0; i < personCount; i++)
            {
                people.Add(new Person(dtDemographics, dtSurnames, random, GetData.getStateFromCdf(stateList, populationCdf, random)));
            }

            return people;
        } 

        private List<PeoplePerState> CalculateGeneratedPopulationStats(List<Person> people) // given a table of people, calculate the economic and demographic statistics of the population in the table
        {
            List<PeoplePerState> stateList = new List<PeoplePerState>(53);
            int totalPop = people.Count();

            for(int i = 1; i < 53; i++) // iterate through the list and initialize members with state names
            {
                PeoplePerState initState = new PeoplePerState();
                initState.State = (States)i;
                stateList.Add(initState);
            }
            for(int j = 0; j < people.Count(); j++)
            {
                stateList[(int)(people[j].State) - 1].Population = stateList[(int)(people[j].State) - 1].Population + 1;

                if (people[j].Gender == GenderTypes.Female)
                {
                    stateList[(int)(people[j].State) - 1].TotalSexFemale = stateList[(int)(people[j].State) - 1].TotalSexFemale + 1;
                } else
                {
                    stateList[(int)(people[j].State) - 1].TotalSexMale = stateList[(int)(people[j].State) - 1].TotalSexMale + 1;
                }

                if(people[j].Age < 5)
                {
                    stateList[(int)(people[j].State) - 1].TotalAgeUnder5 = stateList[(int)(people[j].State) - 1].TotalAgeUnder5 + 1;
                }
                else if(people[j].Age >= 5 && people[j].Age <= 9)
                {
                    stateList[(int)(people[j].State) - 1].TotalAge5To9 = stateList[(int)(people[j].State) - 1].TotalAge5To9 + 1;
                }
                else if (people[j].Age >= 10 && people[j].Age <= 14)
                {
                    stateList[(int)(people[j].State) - 1].TotalAge10To14 = stateList[(int)(people[j].State) - 1].TotalAge10To14 + 1;
                }
                else if (people[j].Age >= 15 && people[j].Age <= 19)
                {
                    stateList[(int)(people[j].State) - 1].TotalAge15To19 = stateList[(int)(people[j].State) - 1].TotalAge15To19 + 1;
                }
                else if (people[j].Age >= 20 && people[j].Age <= 24)
                {
                    stateList[(int)(people[j].State) - 1].TotalAge20To24 = stateList[(int)(people[j].State) - 1].TotalAge20To24 + 1;
                }
                else if (people[j].Age >= 25 && people[j].Age <= 34)
                {
                    stateList[(int)(people[j].State) - 1].TotalAge25To34 = stateList[(int)(people[j].State) - 1].TotalAge25To34 + 1;
                }
                else if (people[j].Age >= 35 && people[j].Age <= 44)
                {
                    stateList[(int)(people[j].State) - 1].TotalAge35To44 = stateList[(int)(people[j].State) - 1].TotalAge35To44 + 1;
                }
                else if (people[j].Age >= 45 && people[j].Age <= 54)
                {
                    stateList[(int)(people[j].State) - 1].TotalAge45To54 = stateList[(int)(people[j].State) - 1].TotalAge45To54 + 1;
                }
                else if (people[j].Age >= 55 && people[j].Age <= 59)
                {
                    stateList[(int)(people[j].State) - 1].TotalAge55To59 = stateList[(int)(people[j].State) - 1].TotalAge55To59 + 1;
                }
                else if (people[j].Age >= 60 && people[j].Age <= 64)
                {
                    stateList[(int)(people[j].State) - 1].TotalAge60To64 = stateList[(int)(people[j].State) - 1].TotalAge60To64 + 1;
                }
                else if (people[j].Age >= 65 && people[j].Age <= 74)
                {
                    stateList[(int)(people[j].State) - 1].TotalAge65To74 = stateList[(int)(people[j].State) - 1].TotalAge65To74 + 1;
                }
                else if (people[j].Age >= 75 && people[j].Age <= 84)
                {
                    stateList[(int)(people[j].State) - 1].TotalAge75To84 = stateList[(int)(people[j].State) - 1].TotalAge75To84 + 1;
                }
                else if (people[j].Age >= 85)
                {
                    stateList[(int)(people[j].State) - 1].TotalAge85OrOver = stateList[(int)(people[j].State) - 1].TotalAge85OrOver + 1;
                }

                if(people[j].Income < 10000)
                {
                    stateList[(int)(people[j].State) - 1].TotalIncomeLessThan10k++;
                }
                else if(people[j].Income < 15000)
                {
                    stateList[(int)(people[j].State) - 1].TotalIncome10kTo15k++;
                }
                else if (people[j].Income < 25000)
                {
                    stateList[(int)(people[j].State) - 1].TotalIncome15kTo25k++;
                }
                else if (people[j].Income < 35000)
                {
                    stateList[(int)(people[j].State) - 1].TotalIncome25kTo35k++;
                }
                else if (people[j].Income < 50000)
                {
                    stateList[(int)(people[j].State) - 1].TotalIncome35kTo50k++;
                }
                else if (people[j].Income < 75000)
                {
                    stateList[(int)(people[j].State) - 1].TotalIncome50kTo75k++;
                }
                else if (people[j].Income < 100000)
                {
                    stateList[(int)(people[j].State) - 1].TotalIncome75kTo100k++;
                }
                else if (people[j].Income < 150000)
                {
                    stateList[(int)(people[j].State) - 1].TotalIncome100kTo150k++;
                }
                else if (people[j].Income < 200000)
                {
                    stateList[(int)(people[j].State) - 1].TotalIncome150kTo200k++;
                }
                else
                {
                    stateList[(int)(people[j].State) - 1].TotalIncomeOver200k++;
                }

                switch (people[j].Race)
                {
                    case RaceTypes.White:
                        stateList[(int)(people[j].State) - 1].TotalRaceWhite++;
                        break;
                    case RaceTypes.Black:
                        stateList[(int)(people[j].State) - 1].TotalRaceBlack++;
                        break;
                    case RaceTypes.AmericanIndianOrAlaskan:
                        stateList[(int)(people[j].State) - 1].TotalRaceAmericanIndianOrAlaskan++;
                        break;
                    case RaceTypes.Asian:
                        stateList[(int)(people[j].State) - 1].TotalRaceAsian++;
                        break;
                    case RaceTypes.NativeHawaiianOrPacificIslander:
                        stateList[(int)(people[j].State) - 1].TotalRaceNativeHawaiianOrPacificIslander++;
                        break;
                    case RaceTypes.Other:
                        stateList[(int)(people[j].State) - 1].TotalRaceOther++;
                        break;
                    case RaceTypes.WhiteAndBlack:
                        stateList[(int)(people[j].State) - 1].TotalRaceWhiteAndBlack++;
                        break;
                    case RaceTypes.WhiteAndAmericanIndianOrAlaskan:
                        stateList[(int)(people[j].State) - 1].TotalRaceWhiteAndAmericanIndianOrAlaskan++;
                        break;
                    case RaceTypes.WhiteAndAsian:
                        stateList[(int)(people[j].State) - 1].TotalRaceWhiteAndAsian++;
                        break;
                    case RaceTypes.BlackAndAmericanIndianOrAlaskan:
                        stateList[(int)(people[j].State) - 1].TotalRaceBlackAndAmericanIndianOrAlaskan++;
                        break;
                }
            }
            return stateList;
        }

       
    }
}