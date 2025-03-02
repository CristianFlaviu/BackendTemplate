using Microsoft.AspNetCore.Mvc;

namespace BackendTemplate.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;

    public TestController(ILogger<TestController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Route("TestEndpoint")]
    public ActionResult<string> Get()
    {
        _logger.LogInformation("Get method called.");

        throw new Exception("This is a test exception.");

        return Ok("Hello World!");
    }
}
