using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Company.Function.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Company.Function
{
    public static class GetRatingsFunctions
    {
        [FunctionName(nameof(GetRatings))]
        public static IActionResult GetRatings(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [CosmosDB(databaseName:@"mhmd-ratings-db", collectionName:@"ratingscontainer", ConnectionStringSetting = @"RatingsDatabase")] IEnumerable<RatingModel> allRatings)
        {
            string userId = null;

            if (req.GetQueryParameterDictionary()?.TryGetValue(@"userId", out userId) == true
                && !string.IsNullOrWhiteSpace(userId))
            {
                var userRatings = allRatings.Where(r => r.userId == userId);

                return !userRatings.Any() ? new NotFoundObjectResult($@"No ratings found for user '{userId}'") : (IActionResult)new OkObjectResult(userRatings);

            }
            else
            {
                return new BadRequestObjectResult(@"userId is required as a query parameter");
            }
        }
    }
}
