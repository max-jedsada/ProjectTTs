using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Project.API.Area.Home.Custom.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "project")]
    public class _CustomController : ControllerBase
    {
        protected const string BaseUrl = "api/[controller]";
    }
}
