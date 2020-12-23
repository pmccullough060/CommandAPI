using CommandAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CommandAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly CommandContext _context; //allows us to link directly to the database using EF
        
        public CommandsController(CommandContext context) 
        {
            //random comment
            _context = context;
        } 

        [HttpGet]
        public ActionResult<IEnumerable<Command>> GetCommandsItems()
        {
            return _context.CommandItems;
        }

        [HttpGet("{id}")]
        public ActionResult<Command> GetCommandsItem(int id)
        {
            var commandItem = _context.CommandItems.Find(id);

            if(commandItem == null)
                return NotFound();

            return commandItem;
        }

        [HttpPost]
        public ActionResult<Command> PostCommandItem(Command command)
        {
            _context.CommandItems.Add(command);

            try
            {
                _context.SaveChanges(); //if it can't save something we wrong...
            }
            catch
            {
                return BadRequest();
            }
            return CreatedAtAction("GetCommandItem", new Command{Id = command.Id}, command);
        }

        [HttpPut("{id}")]
        public ActionResult PutCommandItem(int id, Command command)
        {
            if(id != command.Id)
            {
                return BadRequest();
            }

            _context.Entry(command).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult<Command> DeleteCommandItem(int id)
        {
            var commandItem = _context.CommandItems.Find(id);

            if(commandItem == null)
            {
                return NotFound();
            }

            _context.CommandItems.Remove(commandItem);
            _context.SaveChanges();

            return commandItem;
        }
    }
}

//CommandContext is first configured in the startup class - within configure services 
//it is provided with the credentials needed for accessing our database
//we then can pass this instance to the other classes using constructor dependency injection.
//So this is in effect dependency injection without an interface.