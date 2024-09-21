using FakeUserGenerator.Models;
using FakeUserGenerator.Services;
using Microsoft.AspNetCore.Mvc;

namespace FakeUserGenerator.Controllers
{
    [Route("api/data")]
    public class DataController : Controller
    {
        private readonly FakeUserService _fakeUserService;

        public DataController(FakeUserService fakeUserService)
        {
            _fakeUserService = fakeUserService;
        }

        [HttpGet]
        public IActionResult GetData(int page, string region, int errors, int seed)
        {
            // Call the service to generate user data
            var data = _fakeUserService.GenerateUserData(region, errors, seed, page);
            return new JsonResult(data);
        }

        [HttpGet("exportCsv")]
        public IActionResult ExportToCsv(string region, int errors, int seed)
        {
            // Call the service to generate user data for the CSV file (100 records)
            var data = _fakeUserService.GenerateUserData(region, errors, seed, 100);

            // Convert the data to CSV
            var csv = ConvertToCsv(data);

            // Return the CSV file as a download
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", "fake_user_data.csv");
        }

        private string ConvertToCsv(List<FakeUser> data)
        {
            var csv = new System.Text.StringBuilder();

            // Add the CSV header
            csv.AppendLine("Index,RandomId,Name,Address,Phone");

            // Add the data rows
            foreach (var user in data)
            {
                csv.AppendLine($"{user.Index},{user.RandomId},{user.Name},{user.Address},{user.Phone}");
            }

            return csv.ToString();
        }
    }
}
