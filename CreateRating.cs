using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Company.Function.Models;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Company.Function
{
    public static class CreateRating
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        [FunctionName("CreateRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [CosmosDB(databaseName: "mhmd-ratings-db",collectionName: "ratingscontainer",
                        ConnectionStringSetting = @"RatingsDatabase")] IAsyncCollector<RatingModel> icecreamRatingOut,
            ILogger log)
        {
            string _userid = null;
            string _productid = null;

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            _userid = _userid ?? data?.userId;
            _productid = _productid ?? data?.productId;

            var result = await _httpClient.GetAsync($"https://serverlessohapi.azurewebsites.net/api/GetUser?userId={_userid}");
            var resultContent = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
            {
                var resultProduct = await _httpClient.GetAsync($"https://serverlessohapi.azurewebsites.net/api/GetProduct?productId={_productid}");
                var resultProductContent = await result.Content.ReadAsStringAsync();

                if (resultProduct.IsSuccessStatusCode)
                {
                    var newRatingGuid = Guid.NewGuid().ToString();
                    var _icecreamRating = new RatingModel
                    {
                        id = newRatingGuid,
                        userId = _userid,
                        productId = _productid,
                        locationName = data?.locationName,
                        rating = data?.rating,
                        userNotes = data?.userNotes,
                        timeStamp = DateTime.Now.ToString()
                    };

                    await icecreamRatingOut.AddAsync(_icecreamRating);
                    return new OkObjectResult(newRatingGuid);
                }
            }

            return new BadRequestResult();
        }
    }
}
