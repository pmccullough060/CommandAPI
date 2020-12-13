using System;
using Xunit;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using CommandAPI.Controllers;
using CommandAPI.Models;

namespace CommandAPI.Tests
{
    public class CommandsControllerTests : IDisposable
    {
        DbContextOptionsBuilder<CommandContext> optionsBuilder;
        CommandContext dbContext;
        CommandsController controller;
        public CommandsControllerTests()
        {
            optionsBuilder = new DbContextOptionsBuilder<CommandContext>();
            optionsBuilder.UseInMemoryDatabase("UnitTestInMemDB");
            dbContext = new CommandContext(optionsBuilder.Options);

            controller = new CommandsController(dbContext); //subbing out the "real" dbContext
        }

        public void Dispose() //used to clean up before tests
        {
            optionsBuilder = null;
            foreach (var cmd in dbContext.CommandItems)
            {
                dbContext.CommandItems.Remove(cmd);
            }

            dbContext.SaveChanges();
            dbContext.Dispose();
            controller = null;
        }

        [Fact]
        public void GetCommandItems_ReturnZeroItems_WhenDBIsEmpty()
        {
            //Act
            var result = controller.GetCommandsItems();

            //Assert
            Assert.Empty(result.Value);
        }

        [Fact]
        public void GetCommandItems_ReturnOneItem_WhenDBHasOneObject()
        {
            var command = new Command()
            {
                HowTo = "Do Something",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };

            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();

            //Act
            var result = controller.GetCommandsItems();

            //Assert
            Assert.Single(result.Value);
        }

        [Fact]
        public void GetCommandItems_ReturnNItems_WhenDBHasNObjects()
        {
            var command_1 = new Command()
            {
                HowTo = "Do Something",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };

            var command_2 = new Command()
            {
                HowTo = "Do Something",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };

            dbContext.CommandItems.Add(command_1);
            dbContext.CommandItems.Add(command_2);
            dbContext.SaveChanges();

            //Act
            var result = controller.GetCommandsItems();

            //Assert
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public void GetCommandItems_ReturnsTheCorrectType()
        {
            //Arrange

            //Act
            var result = controller.GetCommandsItems();

            //Assert
            Assert.IsType<ActionResult<IEnumerable<Command>>>(result);
        }

        [Fact]
        public void GetCommandItem_ReturnsNullresult_WhenInvalidID()
        {
            //Arrange 
            //DB should be empty, any ID will be invalid

            //Act
            var result = controller.GetCommandsItem(0);

            //Assert
            Assert.Null(result.Value); //the result value is null
        }

        [Fact]
        public void GetCommandItem_Return404NotFound_WhenInvalidID()
        {
            //Arrange
            //DB should be empty, any ID will be invalid

            //Act
            var result = controller.GetCommandsItem(0);

            //Assert
            Assert.IsType<NotFoundResult>(result.Result); //the action result returned ie 404
        }

        [Fact]
        public void GetCommandItem_returnsTheCorrectType()
        {
            //Arrange
            var command = new Command()
            {
                HowTo = "Do Something",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };

            dbContext.CommandItems.Add(command); //automatically updates the Id property in command when it is added to the in memory database
            dbContext.SaveChanges();

            var cmdId = command.Id; 

            //Act
            var result = controller.GetCommandsItem(cmdId);

            //Assert
            Assert.IsType<ActionResult<Command>>(result);
        }

        [Fact]
        public void GetCommandItems_ReturnTheCorrectResource()
        {
            //Arrange
            var command = new Command()
            {
                HowTo = "Do Something",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };

            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();

            var cmdId = command.Id;

            //Act
            var result = controller.GetCommandsItem(cmdId);

            //Assert
            Assert.Equal(cmdId, result.Value.Id); //making sure the correct item has been returned
        }

        [Fact]
        public void PostCommandItem_ObjectCountIncrement_WhenValidObject()
        {
            //Arrange 
            var command = new Command()
            {
                HowTo = "Do Something",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };

            var oldCount = dbContext.CommandItems.Count();

            //act
            var result = controller.PostCommandItem(command);

            //Assert
            Assert.Equal(oldCount + 1, dbContext.CommandItems.Count());
        }

        [Fact]
        public void PostCommandItem_Returns201Created_WhenValidObject()
        {
            //Arrange 
            var command = new Command()
            {
                HowTo = "Do Something",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };

            //Act
            var result = controller.PostCommandItem(command);

            //Assert
            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        ///pg146
    }
}