using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Company.Function.Models;
using System.Collections.Generic;

namespace Company.Function
{
    public static class GetRating
    {
        [FunctionName("GetRating")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ratings/{id}")] HttpRequest req,
            [CosmosDB("mhmd-ratings-db", "ratingscontainer", ConnectionStringSetting = "RatingsDatabase", SqlQuery = "Select * from ratings r where r.id = {id}")]IEnumerable<RatingModel> rating,
            ILogger log)
        {
            log.LogInformation("Gettingss Ratings");           
            if (rating == null)
            {   
                return new NotFoundResult();
            }  
            else
            {
                return new OkObjectResult(rating);                                
            }
        }
    }
}
