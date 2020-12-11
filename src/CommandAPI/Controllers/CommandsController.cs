using CommandAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CommandAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly CommandContext _context; //allows us to link directly to the database using EF
        
        public CommandsController(CommandContext context) 
        {
            _context = context;
        } 

        [HttpGet]
        public ActionResult<IEnumerable<Command>> Get()
        {
            return _context.CommandItems;
        }
    }
}

//CommandContext is first configured in the startup class - within configure services 
//it is provided with the credentials needed for accessing our database
//we then can pass this instance to the other classes using constructor dependency injection.
//So this is in effect dependency injection without an interface.