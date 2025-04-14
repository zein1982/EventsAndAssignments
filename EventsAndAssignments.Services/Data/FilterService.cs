using System.Security.Claims;
using EventsAndAssignments.Models.DTO.Response;
using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Enums;
using EventsAndAssignments.Services.Interfaces;
using EventsAndAssignments.Services.Sorts;

namespace EventsAndAssignments.Services.Data
{
    public class FilterService : IFilterService
    {
        private readonly IEmployeeService _employeeService;
        private readonly IOrganizationService _organizationService;
        private readonly IAssignmentsService _assignmentService;
        private readonly IFilterGateway _filterGateway;
        private readonly IProtocolFoldersGateway _foldersGateway;

        public FilterService(
            IEmployeeService employeeService,
            IOrganizationService organizationService,
            IAssignmentsService assignmentService,
            IFilterGateway filterGateway,
            IProtocolFoldersGateway foldersGateway)
        {
            _employeeService = employeeService;
            _organizationService = organizationService;
            _assignmentService = assignmentService;
            _filterGateway = filterGateway;
            _foldersGateway = foldersGateway;
        }

        public async Task<IReadOnlyCollection<FieldFilter>> GetAssignmentFilters()
        {
            List<FieldFilter> filters = new()
            {
                new ()
                {
                    Name = nameof(FilterFieldName.UserAssignment),
                    FilterType = FilterEnum.RadioBox,
                    Items = GetFilterItemListForUserAssignmentFilter(),
                    Label = "Мои поручения"
                },
                new()
                {
                    Name = nameof(Assignment.Status),
                    FilterType = FilterEnum.CheckBox,
                    Items = await CreateStatusFilterList(),
                    Label = "Статус"
                },
                new()
                {
                    Name = nameof(FilterFieldName.Urgency),
                    FilterType = FilterEnum.RadioBox,
                    Label = "Срочность",
                    Items = CreateUrgencyFilterList()
                },
                new()
                {
                    Name =nameof(Assignment.Protocol.Folder),
                    FilterType = FilterEnum.CheckBox,
                    Label = "Направление совещания",
                    Items = CreateFolderFilterList()
                },
                new()
                {
                    Name =nameof(Assignment.Created),
                    FilterType = FilterEnum.Data,
                    Label = "Дата создания",
                    Items = new List<FilterItem>()
                },
                new()
                {
                    Name = nameof(Assignment.ResponsibleLeaderId),
                    FilterType = FilterEnum.CheckBox,
                    Label = "ФИО отв.руководителя",
                    Items = CreateResponsibleMangersNameList()
                },
                new()
                {
                    Name = nameof(Assignment.ResponsibleExecutor),
                    FilterType = FilterEnum.CheckBox,
                    Label = "ФИО отв.исполнитель",
                    Items = CreateResponsibleExecutorList()
                },
                new()
                {
                    Name = nameof(Assignment.ResponsibleInspector),
                    FilterType = FilterEnum.CheckBox,
                    Label = "ФИО отв.контролер",
                    Items = CreateInspectorList()
                },
                new()
                {
                    Name = nameof(Assignment.Organization),
                    FilterType = FilterEnum.CheckBox,
                    Label = "Предприятие",
                    Items = await CreateCompanyFilterList()
                },
                new()
                {
                    Name = nameof(FilterFieldName.Activity),
                    FilterType = FilterEnum.RadioBox,
                    Label = "Активность",
                    Items = GetActivityFilterList(),
                },

                //new()
                //{
                //    Name = nameof(Assignment.Protocol),
                //    FilterType = Enums.FilterEnum.CheckBox,
                //    Label = "Номер протокола"
                //},

                //new()
                //{
                //    Name = nameof(Assignment.Protocol),
                //    FilterType = Enums.FilterEnum.CheckBox
                //},
            };

            return filters;
        }

        private List<FilterItem> GetActivityFilterList()
        {
            return new()
                {
                    new()
                    {
                        Selected = true,
                        Value = "1",
                        Label = "Активные поручения",
                    },
                    new()
                    {
                        Selected = false,
                        Value = "0",
                        Label = "Архивные поручения",
                    },
                };
        }

        public IReadOnlyCollection<FieldFilter> GetFolderFilters()
        {
            List<FieldFilter> filters = new()
            {
                new()
                {
                    Name =nameof(DAO.ProtocolFolder.CreatedBy),
                    FilterType = FilterEnum.CheckBox,
                    Label = "Администратор",
                    Items = CreateAdministratorsList()
                },
                new()
                {
                    Name =nameof(DAO.ProtocolFolder.Created),
                    FilterType = FilterEnum.Data,
                    Label = "Дата создания",
                    Items = new List<FilterItem>()
                },
                new()
                {
                    Name =nameof(DAO.ProtocolFolder.Updated),
                    FilterType = FilterEnum.Data,
                    Label = "Дата изменения",
                    Items  = new List<FilterItem>()
                },
                new()
                {
                    Name =nameof(DAO.ProtocolFolder.Name),
                    FilterType = FilterEnum.Search,
                    Label = "поиск"
                }
            };

            return filters;
        }

        public IReadOnlyCollection<FieldFilter> GetProtocolFilters()
        {
            List<FieldFilter> filters = new()
            {
                new()
                {
                    Name = nameof(Protocol.Created),
                    FilterType = FilterEnum.Data,
                    Label = "Дата создания",
                    Items = new List<FilterItem>()
                },
                new()
                {
                    Name = nameof(Protocol.Updated),
                    FilterType = FilterEnum.Data,
                    Label = "Дата изменения",
                    Items = new List<FilterItem>()
                },
            };
            return filters;
        }

        public IReadOnlyCollection<string> GetSortsAssignmentName()
        {
            return new List<string> { nameof(Assignment.Name), nameof(Assignment.ResponsibleExecutor) };
        }

        private List<FilterItem> CreateResponsibleMangersNameList()
        {
            List<FilterItem> ithems = new();
            IReadOnlyCollection<Employee> managers =  _filterGateway.GetResponsibleLeaders();

            foreach (var item in managers)
            {
                if (item is null)
                {
                    continue;
                }

                FilterItem filter = new()
                {
                    Selected = false,
                    Label = item.GetFullName()!,
                    Value = item.PositionId.ToString()!
                };
                ithems.Add(filter);
            }

            return ithems;
        }

        private List<FilterItem> CreateUrgencyFilterList()
        {
            List<FilterItem> ithems = new()
            {
                new()
                {
                    Selected = false,
                    Label = "Не просроченные",
                    Value = "1"
                },
                new()
                {
                    Selected = false,
                    Label = "Просроченные",
                    Value = "0"
                }
            };
            return ithems;
        }

        private List<FilterItem> CreateFolderFilterList()
        {
            List<FilterItem> ithems = new();
            IReadOnlyCollection<DAO.ProtocolFolder> folders = _filterGateway.GetProtocolFolders();

            foreach (var ith in folders)
            {
                FilterItem filter = new()
                {
                    Selected = false,
                    Label = ith.Name,
                    Value = ith.Id.ToString(),
                };
                ithems.Add(filter);
            }

            return ithems;
        }

        private async Task<List<FilterItem>> CreateCompanyFilterList()
        {
            List<FilterItem> ithems = new();
            IReadOnlyCollection<OrganizationResponseDto> organizations =  await _organizationService.GetOrganizations(null);
            foreach (var item in organizations)
            {
                FilterItem filter = new()
                {
                    Selected = false,
                    Label = item.Name,
                    Value = item.Id.ToString(),
                };
                ithems.Add(filter);
            }

            return ithems;
        }

        private List<FilterItem> CreateResponsibleExecutorList()
        {
            List<FilterItem> ithems = new();
            IReadOnlyCollection<Employee?> executors = _filterGateway.GetResponsibleExecutors();
            foreach (var item in executors)
            {
                if (item is null)
                {
                    continue;
                }

                FilterItem filter = new()
                {
                    Selected = false,
                    Label = item.GetFullName()!,
                    Value = item.PositionId.ToString()!
                };
                ithems.Add(filter);
            }

            return ithems;
        }

        private List<FilterItem> CreateInspectorList()
        {
            List<FilterItem> items = new();
            IReadOnlyCollection<Employee?> inspectors = _filterGateway.GetResponsibleInspectors();
            foreach (var item in inspectors)
            {
                if (item is null)
                {
                    continue;
                }

                FilterItem filter = new()
                {
                    Selected = false,
                    Label = item.GetFullName()!,
                    Value = item.PositionId.ToString()!
                };
                items.Add(filter);
            }

            return items;
        }

        private List<FilterItem> CreateAdministratorsList()
        {
            List<FilterItem> ithems = new();
            IReadOnlyCollection<Employee?> admins = _filterGateway.GetAdministrators();
            foreach (var item in admins)
            {
                if (item is null)
                {
                    continue;
                }

                FilterItem filter = new()
                {
                    Selected = false,
                    Label = item.GetFullName()!,
                    Value = item.PositionId.ToString()!
                };
                ithems.Add(filter);
            }

            return ithems;
        }

        private List<FilterItem> GetFilterItemListForUserAssignmentFilter()
        {
            List<FilterItem> items = new()
            {
                new() { Selected = true, Label = "Назначены мне", Value = nameof(FilterItemValue.OnlyMine), },
                new() { Selected = false, Label = "Я автор", Value = nameof(FilterItemValue.IamAuthor), }
            };

            return items;
        }

        private async Task<List<FilterItem>> CreateStatusFilterList()
        {
            IEnumerable<AssignmentStatusResponse> statuses =
                await _assignmentService
                    .GetAllAssignmentStatusesAsync(true);
            //пока что костыль, нужно убрать из бд статус проверено
            return statuses
                .Where(x => x.Id is not "5")
                .Select(status => new FilterItem { Selected = false, Label = status.Name!, Value = status.Id!, }).ToList();
        }
    }
}