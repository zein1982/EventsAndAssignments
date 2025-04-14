using EventsAndAssignments.Services.Data;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace EventsAndAssignments.Tests.UnitTests
{
    internal class FolderTests
    {
        ILogger<ProtocolFoldersService> _logger;
        ILogger<EmployeeService> _empLogger;
        IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            Mock<ILogger<ProtocolFoldersService>> logMock = new();
            Mock<ILogger<EmployeeService>> emplogMock = new();
            Mock<IMapper> mapperMock = new();
            _logger = logMock.Object;
            _mapper = TestHelper.GetMapper();
            _empLogger = emplogMock.Object;
        }

        //[Test]
        //public async Task FolderCreateUpdateDeleteTestAsync()
        //{
        //    ApplicationDbContext ctx = TestHelper.GetTestDbContext();
        //    ProtocolFoldersGateway repo = new(ctx);
        //    EmployeeGateway empRepo = new(ctx);
        //    EmployeeService emp = new(_empLogger, empRepo, _mapper);
        //    ProtocolFoldersService service = new(_logger, repo, _mapper, emp);

        //    string name1 = "папка тест " + DateTime.Now.Millisecond;
        //    string name2 = "папка тест " + DateTime.Now.Millisecond + 1;
        //    string name3 = "папка тест " + DateTime.Now.Millisecond + 2;

        //    //Тест создания
        //    ProtocolFolder cr1 = await service.CreateProtocolFolderAsync(name1, _mockMail);
        //    ProtocolFolder cr2 = await service.CreateProtocolFolderAsync(name2, _mockMail);
        //    ProtocolFolder cr3 = await service.CreateProtocolFolderAsync(name3, _mockMail);

        //    Assert.That(cr1.Name, Is.EqualTo(name1));
        //    Assert.That(cr2.Name, Is.EqualTo(name2));
        //    Assert.That(cr3.Name, Is.EqualTo(name3));

        //    Assert.That(cr1.Id, Is.Not.EqualTo(0));
        //    Assert.That(cr2.Id, Is.Not.EqualTo(0));
        //    Assert.That(cr3.Id, Is.Not.EqualTo(0));

        //    //Тест получения
        //    ProtocolFolder get1 = await service.GetProtocolFolderAsync(cr1.Id);
        //    ProtocolFolder get2 = await service.GetProtocolFolderAsync(cr2.Id);
        //    ProtocolFolder get3 = await service.GetProtocolFolderAsync(cr3.Id);

        //    Assert.That(get1.Name, Is.EqualTo(name1));
        //    Assert.That(get2.Name, Is.EqualTo(name2));
        //    Assert.That(get3.Name, Is.EqualTo(name3));

        //    //Тест изменени
        //    ProtocolFolder up1 = await service.UpdateProtocolFolderAsync(cr1.Id, name1.Replace("тест", "тест update"), _mockMail);
        //    ProtocolFolder up2 = await service.UpdateProtocolFolderAsync(cr2.Id, name2.Replace("тест", "тест update"), _mockMail);
        //    ProtocolFolder up3 = await service.UpdateProtocolFolderAsync(cr3.Id, name3.Replace("тест", "тест update"), _mockMail);

        //    Assert.That(up1.Name, Is.EqualTo(name1.Replace("тест", "тест update")));
        //    Assert.That(up2.Name, Is.EqualTo(name2.Replace("тест", "тест update")));
        //    Assert.That(up3.Name, Is.EqualTo(name3.Replace("тест", "тест update")));

        //    ////Тест получения списка
        //    //var list1 = await service.GetProtocolFoldersAsync(2, 1);

        //    //Assert.That(list1.Count, Is.EqualTo(2));

        //    //Тест получения количества элементов в списке
        //    int count1 = await service.GetProtocolFolderCountAsync(new List<FieldFilter>());

        //    Assert.That(count1, Is.GreaterThanOrEqualTo(3));

        //    //тест архивации, удаления
        //    List<long> idList = new(){ cr1.Id, cr2.Id, cr3.Id };

        //    await service.ArchiveProtocolFolderAsync(idList);
        //    ProtocolFolder ar1 = await service.GetProtocolFolderAsync(cr1.Id);

        //    Assert.That(ar1.IsArchived, Is.EqualTo(true));

        //    await service.RemoveProtocolFolderAsync(idList);

        //    Assert.ThrowsAsync<InvalidOperationException>(async () => await service.GetProtocolFolderAsync(cr1.Id));
        //}

        //[Test]
        //public async Task FolderSearchTestAsync()
        //{
        //    ApplicationDbContext ctx = TestHelper.GetTestDbContext();
        //    ProtocolFoldersGateway repo = new(ctx);
        //    EmployeeGateway empRepo = new(ctx);
        //    EmployeeService emp = new(_empLogger, empRepo, _mapper);
        //    ProtocolFoldersService service = new(_logger, repo, _mapper, emp);

        //    RequestParams filter = new();

        //    filter.Filters.Add(new FieldFilter
        //    {
        //        Name = "Name",
        //        FilterType = Services.Enums.FilterEnum.Search
        //    });

        //    filter.Filters[0].Items.Add(new FilterItem
        //    {
        //        Value = "45",
        //        Selected = true
        //    });

        //    //Тест получения списка
        //    IReadOnlyCollection<ProtocolFolder> list1 = await service.GetProtocolFoldersAsync(filter, _mockMail);
        //    Assert.That(list1, Is.Not.Null);
        //    Assert.That(list1.Count, Is.EqualTo(2));
        //    Assert.That(list1.FirstOrDefault()!.Name, Is.EqualTo("45"));
        //}

        //[Test]
        //public async Task FolderGetByFilterTestAsync()
        //{
        //    ApplicationDbContext ctx = TestHelper.GetTestDbContext();
        //    ProtocolFoldersGateway repo = new(ctx);
        //    EmployeeGateway empRepo = new(ctx);
        //    EmployeeService emp = new(_empLogger, empRepo, _mapper);
        //    ProtocolFoldersService service = new(_logger, repo, _mapper, emp);

        //    RequestParams requestParams = new();

        //    requestParams.Filters.Add(new FieldFilter
        //    {
        //        Name = nameof(ProtocolFolder.CreatedBy),
        //        FilterType = Services.Enums.FilterEnum.CheckBox
        //    });

        //    requestParams.Filters[0].Items.Add(new FilterItem
        //    {
        //        Value = "F2520887-A955-EC11-B3CB-0050569A16C2",
        //        Selected = true
        //    });

        //    requestParams.Filters[0].Items.Add(new FilterItem
        //    {
        //        Value = "F2520887-A955-EC11-B3CB-0050569A16C3",
        //        Selected = true
        //    });

        //    //Тест получения списка
        //    IReadOnlyCollection<ProtocolFolder> list1 = await service.GetProtocolFoldersAsync(requestParams, _mockMail);

        //    Assert.That(list1.Count, Is.EqualTo(3));
        //    Assert.That(list1.FirstOrDefault().UpdatedBy, Is.EqualTo(new Guid("F2520887-A955-EC11-B3CB-0050569A16C1")));
        //}

        //[Test]
        //public async Task FolderSortTestAsync()
        //{
        //    ApplicationDbContext ctx = TestHelper.GetTestDbContext();
        //    ProtocolFoldersGateway repo = new(ctx);
        //    EmployeeGateway empRepo = new(ctx);
        //    EmployeeService emp = new(_empLogger, empRepo, _mapper);
        //    ProtocolFoldersService service = new(_logger, repo, _mapper, emp);

        //    RequestParams requestParams = new();

        //    requestParams.Sorts.Add(new FieldSort
        //    {
        //        Name = "Id",
        //        SortDirection = "ascending"
        //    });

        //    IReadOnlyCollection<ProtocolFolder> list1 = await service.GetProtocolFoldersAsync(requestParams, _mockMail);

        //    long firstId = list1.First().Id;

        //    requestParams.Sorts[0].SortDirection = "descending";

        //    IReadOnlyCollection<ProtocolFolder> list2 = await service.GetProtocolFoldersAsync(requestParams, _mockMail);

        //    long secondId = list2.First().Id;

        //    Assert.That(secondId, Is.GreaterThanOrEqualTo(firstId));
        //}
    }
}