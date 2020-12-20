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
            Assert.Equal(oldCount + 1, dbContext.CommandItems.Count()); //checking that a new item has been added to the database/dbContext
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
            Assert.IsType<CreatedAtActionResult>(result.Result); //checks that a 201 is returned once new object is posted to database
        }

        [Fact]
        public void PutCommandItem_AttributeUpdated_WhenValidObject() //updating an existing item
        {
            //Arrange
            var command = new Command()
            {
                HowTo = "DoSomething",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };

            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();

            var cmdId = command.Id;

            command.HowTo = "UPDATED";

            //Act
            controller.PutCommandItem(cmdId, command);
            var result = dbContext.CommandItems.Find(cmdId);

            //Assert
            Assert.Equal(command.HowTo, result.HowTo);
        }

        [Fact]
        public void PutCommandItem_Returns204_WhenValidObject() //204 - indicates that the server has successfully fulfilled the request and no content response.
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

            command.HowTo = "UPDATED";

            //Act
            var result = controller.PutCommandItem(cmdId, command);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void PutCommandItem_Returns400_WhenInvalidObject()
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

            var cmdId = command.Id + 1;

            command.HowTo = "UPDATED";

            //Act
            var result = controller.PutCommandItem(cmdId, command);

            //Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void PutCommandItem_AttributeUnchanged_WhenInvalidObject()
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

            var command2 = new Command()
            {
                Id = command.Id,
                HowTo = "UPDATED",
                Platform = "UPDATED",
                CommandLine = "UPDATED"
            };

            //Act
            controller.PutCommandItem(command.Id + 1, command2);
            var result = dbContext.CommandItems.Find(command.Id);

            //Assert
            Assert.Equal(command.HowTo, result.HowTo);
        }

        [Fact]
        public void DeleteCommandItem_ObjectsDecrement_WhenValidObjectID()
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
            var objCount = dbContext.CommandItems.Count();

            //Act
            controller.DeleteCommandItem(cmdId);

            //Assert
            Assert.Equal(objCount - 1, dbContext.CommandItems.Count());
        }

        [Fact]
        public void DeleteCommandItem_Returns200OK_WhenValidObjectID()
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
            var result = controller.DeleteCommandItem(cmdId);

            //Assert        
            Assert.Null(result.Result);
        }

        [Fact]
        public void DeleteCommandItem_Returns404NotFound_WhenValidObjectID()
        {
            //Arrange

            //Act
            var result = controller.DeleteCommandItem(-1);

            //Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void DeleteCommandItem_ObjectCountNotDecremented_WhenValidObjectID()
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
            var objCount = dbContext.CommandItems.Count();

            //Act
            var result = controller.DeleteCommandItem(cmdId + 1);

            //Assert
            Assert.Equal(objCount, dbContext.CommandItems.Count());
        }
    }
}