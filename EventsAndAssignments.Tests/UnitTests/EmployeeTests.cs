using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Data;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace EventsAndAssignments.Tests.UnitTests
{
    internal class EmployeeTests
    {
        ILogger<EmployeeService> _logger;

        [SetUp]
        public void Setup()
        {
            Mock<ILogger<EmployeeService>> logMock = new();
            _logger = logMock.Object;
        }
    }
}