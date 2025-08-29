using System.Net;
using System.Text;
using System.Text.Json;
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
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            //not using below because using postman to send json array of object
            //PersonModel[] people =
            //[
            //    new() { FirstName = "Gia", LastName = "Wolanski" },
            //    new() { FirstName = "Chris", LastName = "Mersman"},
            //    new() { FirstName = "Bob", LastName = "Builder"}
            //];

            string conn = _config.GetConnectionString("Default");


            var data = await JsonSerializer.DeserializeAsync<List<PersonModel>>(req.Body);

            if (data is null || data.Count == 0)
            {
                _logger.LogError("No data found in request body.");
                return new OkObjectResult(HttpStatusCode.BadRequest);
            }

            _logger.LogInformation($"Processing GetPeople action for {data}", req.Body);

            bool? includeHeader = _config.GetValue<bool>("includeHeader");

            if (includeHeader is null)
            {
                _logger.LogError("No includeHeader value found in config.");
                return new OkObjectResult(HttpStatusCode.InternalServerError);
            }
            else if (includeHeader is false)
            {
                var csvBuilder = new StringBuilder();
                foreach (var person in data)
                {
                    csvBuilder.AppendLine($"{person.FirstName},{person.LastName}");
                }
                byte[] fileBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());

                return new FileContentResult(fileBytes, "text/csv")
                {
                    FileDownloadName = "people.csv"
                };
            }
            else
            {
                var csvBuilder = new StringBuilder();
                csvBuilder.AppendLine("FirstName,LastName");

                foreach (var person in data)
                {
                    csvBuilder.AppendLine($"{person.FirstName},{person.LastName}");
                }
                byte[] fileBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());

                return new FileContentResult(fileBytes, "text/csv")
                {
                    FileDownloadName = "people.csv"
                };
            }


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
