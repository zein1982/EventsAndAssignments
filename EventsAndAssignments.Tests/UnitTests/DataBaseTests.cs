using EventsAndAssignments.Db;
using EventsAndAssignments.Db.Repositories;
using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Data;
using EventsAndAssignments.Services.Sorts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace EventsAndAssignments.Tests.UnitTests
{
    internal class DataBaseTests
    {
        ILogger<ProtocolFoldersService> _logger;

        [SetUp]
        public void Setup()
        {
            Mock<ILogger<ProtocolFoldersService>> logMock = new();
            _logger = logMock.Object;
        }

        [Test]
        public void DataBaseConfigTest()
        {
            ApplicationDbContext ctx = TestHelper.GetTestDbContext();

            //ctx.Database.EnsureDeleted();
            //ctx.Database.EnsureCreated();
        }

        [Test]
        public void DataBaseGetByFilterTest()
        {
            ApplicationDbContext ctx = TestHelper.GetTestDbContext();

            RequestParams filter = new();

            filter.Filters.Add(new FieldFilter
            {
                Name = "Name",
                FilterType = Services.Enums.FilterEnum.Search
            });

            filter.Filters[0].Items.Add(new FilterItem
            {
                Value = "Папка1",
                Selected = true
            });

            //Тест получения списка
            string queryStr = ctx.ProtocolFolders.AsNoTracking().GetByFilter(filter).ToQueryString();

            Console.WriteLine(queryStr);
            //NUnit.Framework.

            List<ProtocolFolder> folders = ctx.ProtocolFolders.AsNoTracking().GetByFilter(filter).ToList();

            Assert.That(folders.Count, Is.EqualTo(1));
            Assert.That(folders?.FirstOrDefault()?.Name, Is.EqualTo("Папка1"));
        }
    }
}