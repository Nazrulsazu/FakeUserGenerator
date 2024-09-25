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
            int recordsPerPage = 20;
            int startIndex = page * recordsPerPage + 1;

            // Use the same seed for all pages
            var random = new Random(seed);

            // Skip random numbers for previous pages
            SkipRandomNumbers(random, page * recordsPerPage);

            // Generate the data with the appropriate start index
            var data = _fakeUserService.GenerateUserData(region, errors, random, recordsPerPage, startIndex);
            return new JsonResult(data);
        }

        [HttpGet("exportCsv")]
        public IActionResult ExportToCsv(string region, int errors, int seed)
        {
            // Generate 100 records for the CSV export
            var random = new Random(seed);

            // Skip numbers for CSV generation consistency
            SkipRandomNumbers(random, 0); // Skip 0 numbers since we want all 100 records

            // Generate 100 records
            var data = _fakeUserService.GenerateUserData(region, errors, random, 100, 1); // Start index from 1 for CSV

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

        // Function to skip a certain number of random numbers
        private void SkipRandomNumbers(Random random, int count)
        {
            for (int i = 0; i < count; i++)
            {
                random.Next();
            }
        }
    }
}
