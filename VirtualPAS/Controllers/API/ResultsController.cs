using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VirtualPAS.Models;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using VirtualPAS.ViewModels;

namespace VirtualPAS.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsController : Controller
    {
        //GET /api/customers
        [Route("Home")]
        [Route("/")]
        [Route("/Index")]
        [Route("Home/Index")]
        public IActionResult Index()
        {
            var results = GetResultsFromFile();

            results = results.OrderBy(m=>m.MapName).ThenBy(c => c.Course).ThenByDescending(p=>p.Points).ThenBy(t=>t.CompletedTime).ToList();

            var resultsVM = AddRank(results);

            var newresults = (from result in resultsVM
                             group result by result.MapName into mapgroup
                             from coursegroup in
                                 (from result in mapgroup
                                  group result by result.Course)
                             group coursegroup by mapgroup.Key).ToList();

            return View(newresults);
        }

        private List<ResultViewModel> AddRank(List<Result> results)
        {
            var resultsVMList = new List<ResultViewModel>();

            var lastCourse = string.Empty;
            int rank = 1;
            
            foreach (var item in results)
            {
                var resultsViewModel = new ResultViewModel();
                if (item.Course != lastCourse)
                {
                    rank = 1;
                    lastCourse = item.Course;
                }
                resultsViewModel.Rank = rank;
                resultsViewModel.RowStatus = item.RowStatus;
                resultsViewModel.Year = item.Year;
                resultsViewModel.Series = item.Series;
                resultsViewModel.Day = item.Day;
                resultsViewModel.CourseDate = item.CourseDate;
                resultsViewModel.MapName = item.MapName;
                resultsViewModel.FirstName = item.FirstName;
                resultsViewModel.LastName = item.LastName;
                resultsViewModel.Course = item.Course;
                resultsViewModel.StartTime = item.StartTime;
                resultsViewModel.FinishTime = item.FinishTime;
                resultsViewModel.CompletedTime = item.CompletedTime;
                resultsViewModel.NumberOfControls = item.NumberOfControls;
                resultsViewModel.Status = item.Status;
                resultsViewModel.NumberOfTwoPoints = item.NumberOfTwoPoints;
                resultsViewModel.NumberOfThreePoints = item.NumberOfThreePoints;
                resultsViewModel.NumberOfFourPoints = item.NumberOfFourPoints;
                resultsViewModel.NumberOfFivePoints = item.NumberOfFivePoints;
                resultsViewModel.NumberOfSixPoints = item.NumberOfSixPoints;
                resultsViewModel.NumberLateMinutes = item.NumberLateMinutes;
                resultsViewModel.TotalPoints = item.Points;
                resultsViewModel.Distance = item.Distance;

                resultsVMList.Add(resultsViewModel);
                rank++;
            }
            return resultsVMList;
        }

        private string CalculateTotalPoints(
            string numberOfTwoPoints, 
            string numberOfThreePoints, 
            string numberOfFourPoints, 
            string numberOfFivePoints, 
            string numberOfSixPoints, 
            string numberLateMinutes)
        {
            int iNumberTwoPoints = (numberOfTwoPoints == null) ? 0 : Convert.ToInt16(numberOfTwoPoints)*2;
            int iNumberThreePoints = (numberOfThreePoints == null) ? 0 : Convert.ToInt16(numberOfThreePoints)*3;
            int iNumberFourPoints = (numberOfFourPoints == null) ? 0 : Convert.ToInt16(numberOfFourPoints)*4;
            int iNumberFivePoints = (numberOfFivePoints == null) ? 0 : Convert.ToInt16(numberOfFivePoints)*5;
            int iNumberSixPoints = (numberOfSixPoints == null) ? 0 : Convert.ToInt16(numberOfSixPoints)*6;
            int iNumberLatePoints = (numberLateMinutes == null) ? 0 : Convert.ToInt16(numberLateMinutes)*3;

            var totalPoints =    
                iNumberTwoPoints +
                iNumberThreePoints +
                iNumberFourPoints +
                iNumberFivePoints +
                iNumberSixPoints -
                iNumberLatePoints;

            return totalPoints.ToString();
        }

        private List<ResultViewModel> ConvertToViewModel(List<IGrouping<string, IGrouping<string,Result>>> results)
        {
            var resultsVMList = new List<ResultViewModel>();

            var resultsViewModel = new ResultViewModel();
            foreach(var map in results)
            {
                foreach(var course in map)
                {
                    int rank = 1;
                    foreach(var item in course)
                    {
                        resultsViewModel.Rank = rank;
                        resultsViewModel.RowStatus = item.RowStatus;
                        resultsViewModel.Year = item.Year;
                        resultsViewModel.Series = item.Series;
                        resultsViewModel.Day = item.Day;
                        resultsViewModel.CourseDate = item.CourseDate;
                        resultsViewModel.MapName = item.MapName;
                        resultsViewModel.FirstName = item.FirstName;
                        resultsViewModel.LastName = item.LastName;
                        resultsViewModel.Course = item.Course;
                        resultsViewModel.StartTime = item.StartTime;
                        resultsViewModel.FinishTime = item.FinishTime;
                        resultsViewModel.CompletedTime = item.CompletedTime;
                        resultsViewModel.NumberOfControls = item.NumberOfControls;
                        resultsViewModel.Status = item.Status;
                        resultsViewModel.NumberOfTwoPoints = item.NumberOfTwoPoints;
                        resultsViewModel.NumberOfThreePoints = item.NumberOfThreePoints;
                        resultsViewModel.NumberOfFourPoints = item.NumberOfFourPoints;
                        resultsViewModel.NumberOfFivePoints = item.NumberOfFivePoints;
                        resultsViewModel.NumberOfSixPoints = item.NumberOfSixPoints;
                        resultsViewModel.NumberLateMinutes = item.NumberLateMinutes;
                        resultsViewModel.TotalPoints = item.Points;
                        resultsViewModel.Distance = item.Distance;

                        resultsVMList.Add(resultsViewModel);
                        rank++;
                    }
                }
            }
            return resultsVMList;
    }

        private List<Result> GetResultsFromFile()
        {
            Task<String> json = GetAPIDataAsync();

            var resultCollection = JsonConvert.DeserializeObject<ResultsCollection>(json.Result);

            return ParseRootCollection(resultCollection);
        }

        private List<Result> ParseRootCollection(ResultsCollection resultsCollection)
        {
            return ParseCollection(resultsCollection.Values);
        }

        private List<Result> ParseCollection(List<JArray> list)
        {
            List<Result> resultCollection = new List<Result>();
            int i = 0;
            foreach (var item in list)
            {
                var result = new Result();
                var x = item.ToList();
                result.Id = i;
                result.RowStatus = x[0].ToString();
                result.Year = x[1].ToString();
                result.Series = x[2].ToString();
                result.Day = x[3].ToString();
                result.CourseDate = x[4].ToString();
                result.MapName = x[5].ToString();
                result.FirstName = x[6].ToString();
                result.LastName = x[7].ToString();
                result.Course = x[8].ToString();
                result.StartTime = x[9].ToString();
                result.FinishTime = x[10].ToString();
                result.CompletedTime = x[11].ToString();
                result.NumberOfControls = x[12].ToString();
                result.Status = x[13].ToString();
                result.NumberOfTwoPoints = x[14].ToString();
                result.NumberOfThreePoints = x[15].ToString();
                result.NumberOfFourPoints = x[16].ToString();
                result.NumberOfFivePoints = x[17].ToString();
                result.NumberOfSixPoints = x[18].ToString();
                result.NumberLateMinutes = x[19].ToString();
                result.Points = x[20].ToString();
                result.Distance = 
                    (string.IsNullOrEmpty(x[21].ToString())||(x[21].ToString()=="0"))
                        ?string.Empty
                        : x[21].ToString() + " kms";
                resultCollection.Add(result);
                i++;
            }

            return resultCollection;
        }

        private async Task<string> GetAPIDataAsync()
        {
            string response = string.Empty;

            string googleSheetsApiKey = "AIzaSyB9REoHqxCbaJ0Qo1U1FGlGEpoJlENFhGo";
            string Baseurl = "https://sheets.googleapis.com/";

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                string add = "v4/spreadsheets/1cpTG9sLWk26fnG-lJnkxO2aBVrPboHdtSnhk_4GJTKo/values/Sheet1!A6:V43/?key=" + googleSheetsApiKey;
                HttpResponseMessage Res = await client.GetAsync(add);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    response = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    //EmpInfo = JsonConvert.DeserializeObject<List<Employee>>(EmpResponse);
                }
            }

            return response;
        }

        private string GetTestData()
        {
            return @"  
        {
          'range': 'Sheet1!A6: V43',
          'majorDimension': 'ROWS',
          'values': [
            [
              'OPEN',
              '2019-2020',
              'Summer Evening',
              '',
              '04/12/2020',
              'Wantirna South',
              'Mary',
              'Citizen',
              'C',
              '19:00:00',
              '19:59:58',
              '0:59:58',
              '',
              '',
              '0',
              '0',
              '0',
              '0',
              '0',
              '0',
              '0',
              '7.67'
            ],
            [
              'OPEN',
              '2019-2020',
              'Summer Evening',
              'Wednesday',
              '25/03/2020',
              'Bennettswood',
              'Ian',
              'Dodd',
              'C',
              '15:43:00',
              '16:12:33',
              '0:29:33',
              '12',
              'OK',
              '0',
              '0',
              '0',
              '0',
              '0',
              '0',
              '0'
            ],
            [
              'OPEN',
              '2019-2020',
              'Summer Evening',
              'Monday',
              '23/03/2020',
              'Wantirna South',
              'Bruce',
              'Paterson',
              'B',
              '19:00:00',
              '19:40:21',
              '0:40:21',
              '13',
              'OK',
              '0',
              '0',
              '0',
              '0',
              '0',
              '0',
              '0'
            ],
            [
              'OPEN',
              '2019-2020',
              'Summer Evening',
              'Monday',
              '23/03/2020',
              'Wantirna South',
              'Ian',
              'Davies',
              'A',
              '18:50:00',
              '19:38:08',
              '0:48:08',
              '16',
              'OK',
              '0',
              '0',
              '0',
              '0',
              '0',
              '0',
              '0'
            ],
            [
              'OPEN',
              '2019-2020',
              'Summer Evening',
              'Monday',
              '24/03/2020',
              'Highbury Hill',
              'Ian',
              'Greenwood',
              'PW',
              '10:04:00',
              '11:08:48',
              '1:04:48',
              '12',
              'OK',
              '1',
              '4',
              '5',
              '2',
              '0',
              '0',
              '44'
            ]
        ]}";
        }
    }
}