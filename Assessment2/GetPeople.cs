using System.Text;
using Assessment2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Assessment2
{
    public class GetPeople
    {
        private readonly ILogger<GetPeople> _logger;
        private readonly IConfiguration _config;


        public GetPeople(ILogger<GetPeople> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [Function("GetPeople")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            PersonModel[] people =
            [
                new() { FirstName = "Gia", LastName = "Wolanski" },
                new() { FirstName = "Chris", LastName = "Mersman"},
                new() { FirstName = "Bob", LastName = "Builder"}
            ];

            string filePath = "people.csv";
            string conn = _config.GetConnectionString("Default");

            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("FirstName,LastName,ConnectionString");

            foreach (var person in people)
            {
                csvBuilder.AppendLine($"{person.FirstName},{person.LastName}, {conn}");
            }

            byte[] fileBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());

            return new FileContentResult(fileBytes, "text/csv")
            {
                FileDownloadName = "people.csv"
            };

            //using (StreamWriter sw = new(filePath))
            //{
            //    sw.WriteLine("FirstName,LastName,ConnectionString");

            //    foreach (var person in people)
            //    {
            //        sw.WriteLine(person.FirstName);
            //        sw.WriteLine(person.LastName);
            //        sw.WriteLine(conn);
            //    }
            //}
            
        }
    }
}
