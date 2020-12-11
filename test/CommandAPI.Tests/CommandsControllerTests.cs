using System;
using Xunit;
using Microsoft.EntityFrameworkCore;
using CommandAPI.Controllers;
using CommandAPI.Models;

namespace CommandAPI.Tests
{
    public class CommandsControllerTests
    {
        [Fact]
        public void GetCommandItems_ReturnZeroItems_WhenDBIsEmpty()
        {
            //Arrange
            //DBContext
            var optionsBuilder = new DbContextOptionsBuilder<CommandContext>();
            optionsBuilder.UseInMemoryDatabase("UnitTestInMemDB");
            var dbContext = new CommandContext(optionsBuilder.Options);
        }
    }d
}