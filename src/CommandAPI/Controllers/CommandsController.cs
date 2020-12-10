using CommandAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CommandAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly CommandContext _context;
        
        public CommandsController(CommandContext context) => _context = context;

        [HttpGet]
        public ActionResult<IEnumerable<Command>> Get()
        {
            return _context.CommandItems;
        }
    }
}