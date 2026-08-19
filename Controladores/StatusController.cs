using Microsoft.AspNetCore.Mvc;

namespace TransparenciaBot.Controladores;

[ApiController]
[Route("api/health")]
public class StatusController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "ok" });
}
