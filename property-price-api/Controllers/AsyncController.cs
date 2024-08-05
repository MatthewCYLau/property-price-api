using Microsoft.AspNetCore.Mvc;

namespace property_price_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AsyncController : ControllerBase
    {

        private static async Task<int> GetRandomIntegerAsync()
        {
            await Task.Yield();
            return new Random().Next(0, 1_000);
        }

        [HttpGet]
        public async Task<IActionResult> TaskWhenAll()
        {

            var randomOne = Task.Run(GetRandomIntegerAsync);
            var randomTwo = Task.Run(GetRandomIntegerAsync);

            await Task.WhenAll(randomOne, randomTwo);
            var sum = randomOne.Result + randomTwo.Result;
            return Ok(sum);
        }
    }
}


