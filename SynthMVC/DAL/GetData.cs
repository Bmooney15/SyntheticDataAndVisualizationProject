using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;
using SynthMVC.Models;

namespace SynthMVC.DAL
{
    public class GetData
    {
        public static DataTable getSurnames(string url)
        {
            WebClient webClient = new WebClient();
            DataTable dataTable = new DataTable();

            int count = 0;
            bool readHeader = false;

            byte[] data = webClient.DownloadData(url);

            string str = Encoding.UTF8.GetString(data);

            str = str.Remove(0, 2);



            foreach (string row in str.Split(new string[] { "],\n[" }, StringSplitOptions.None))
            {
                DataRow dataRow = dataTable.NewRow();
                if (!readHeader)
                {
                    foreach (string header in row.Split(','))
                    {
                        string[] keyValue = header.Split('"');
                        string labelStr = keyValue[1];
                        dataTable.Columns.Add(labelStr);
                        count++; // get number of headers

                    }
                    readHeader = true;
                    continue;
                }
                int i = 0;
                DataRow dr = dataTable.NewRow();
                foreach (string cell in row.Split(','))
                {
                    if (i < count)
                    {
                        string[] keyValue = cell.Split('"');
                        dr[i] = keyValue[1];
                        i++;
                    }
                }

                dataTable.Rows.Add(dr);
            }
            return dataTable;
        }

        public static DataTable getData(string url, string jsonUrl)
        {
            WebClient webClient = new WebClient();
            DataTable dataTable = new DataTable();

            int count = 0;
            bool readHeader = false;

            byte[] data = webClient.DownloadData(url);
            byte[] jsonData = webClient.DownloadData(jsonUrl);  // json data containing variable names and descriptions

            string str = Encoding.UTF8.GetString(data);
            string jsonStr = Encoding.UTF8.GetString(jsonData);

            str = str.Remove(0, 2);

            JObject token = JObject.Parse(jsonStr);
            JToken variables = token["variables"];


            foreach (string row in str.Split(new string[] { "],\n[" }, StringSplitOptions.None))
            {
                DataRow dataRow = dataTable.NewRow();
                if (!readHeader)
                {
                    foreach (string header in row.Split(','))
                    {
                        string[] keyValue = header.Split('"');
                        string labelStr = keyValue[1];
                        if (labelStr != "NAME")
                        {
                            labelStr = (string)variables.SelectToken(labelStr + ".label"); // get description of variable from variable name
                        }
                        dataTable.Columns.Add(labelStr);
                        count++; // get number of headers

                    }
                    readHeader = true;
                    continue;
                }
                int i = 0;
                DataRow dr = dataTable.NewRow();
                foreach (string cell in row.Split(','))
                {
                    if (i < count)
                    {
                        string[] keyValue = cell.Split('"');
                        dr[i] = keyValue[1];
                        i++;
                    }
                }

                dataTable.Rows.Add(dr);
            }
            return dataTable;
        }

        public static double[] getCdf(double[] probabilities)
        {
            double[] cdf = new double[probabilities.Length];
            double lowerBound = 0;

            for (int i = 0; i < probabilities.Length; i++)
            {
                cdf[i] = lowerBound + probabilities[i];
                lowerBound = lowerBound + probabilities[i];
            }
            for (int j = 0; j < cdf.Length; j++)
            {
                cdf[j] = cdf[j] / 100;
            }

            return cdf;
        }

        public static double[] getPopProbabilities(int[] populations)
        {
            double[] probabilities = new double[populations.Length];
            double total = 0;

            for (int i = 0; i < populations.Length; i++)
            {
                total = total + populations[i];
            }
            for (int j = 0; j < probabilities.Length; j++)
            {
                probabilities[j] = (populations[j] / total) * 100;
            }
            return probabilities;
        }

        public static int getRowFromState(States stateName, DataTable demographics)
        {
            int rowNum = 0;
            for (int i = 0; i < 52; i++) // iterate through rows of census table
            {
                if ((string)demographics.Rows[i][0] == EnumExtensions.GetEnumDescription((States)stateName))  // if current row matches the state passed by stateName, use this row to calculate cdf for the person
                {
                    rowNum = i;
                }
            }
            return rowNum;
        }

        public static States getStateFromCdf(States[] stateList, double[] cdf, Random random)
        {
            // given list of states, and their respective cdf, return a state based on the outcome of a random number from 0-1
            double randNum = random.NextDouble();


            for (int i = 0; i < stateList.Length; i++)
            {
                if (randNum < cdf[i])
                {
                    return stateList[i];
                }
            }
            return stateList[stateList.Length];
        }

        // given a list of people, return a list containing all states with accurate statistics based on list of people TODO terrible description
        public static List<PeoplePerState> GetStateStatsFromPeople(List<Person> people)
        {
            List<PeoplePerState> stateList = new List<PeoplePerState>(53);
            
            for(int i = 1; i < 53; i++) // initialize stateList with proper states
            {
                PeoplePerState temp = new PeoplePerState();
                temp.State = (States)i;
                temp.PeopleInState = new List<Person>(people.Count());
                stateList.Add(temp);
            }
           
            for(int j = 0; j < people.Count(); j++)
            {
                stateList[(int)(people[j].State) - 1].PeopleInState.Add(people[j]); // add people into lists in their respective states
            }
            return stateList;
        }
    }
}