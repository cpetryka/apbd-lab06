using Microsoft.AspNetCore.Mvc;

namespace Tutorial6.Controller;

[ApiController]
[Route(("api/[controller]"))]
public class AnimalsController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AnimalsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
}