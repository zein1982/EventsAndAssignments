using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Sorts;
using Microsoft.EntityFrameworkCore;

namespace EventsAndAssignments.Db.Repositories
{
    public static class LinqExtension
    {
        public static IQueryable<T> NotRemoved<T>(this IQueryable<T> items) where T : BaseEntity
        {
            return items.Where(x => x.Removed == null);
        }

        public static IQueryable<T> Intersect<T>(this IQueryable<T> items, IReadOnlyCollection<long> ids) where T : BaseEntity
        {
            return items.NotRemoved().Where(x => ids.Contains(x.Id));
        }

        public static Task<T> GetById<T>(this IQueryable<T> items, long id) where T : BaseEntity
        {
            return items.NotRemoved().SingleAsync(x => x.Id == id);
        }

        public static IQueryable<T> GetPage<T>(this IQueryable<T> items, int count, int page)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (count < 0)
            {
                count = 0;
            }

            page--;

            return items.Skip(count * page).Take(count);
        }

        public static IQueryable<ProtocolFolder> GetByFilter(this IQueryable<ProtocolFolder> items, RequestParams filter)
        {
            IQueryable<ProtocolFolder> ret = items.NotRemoved().GetFiltered(filter.Filters);

            return ret;
        }

        //фильтр для папок
        public static IQueryable<T> GetSorted<T>(this IQueryable<T> items, List<FieldSort> sorts) where T : BaseEntity
        {
            foreach (var sort in sorts)
            {
                items = sort.SortDirection == "descending"
                    ? items.OrderByDescending(p => EF.Property<object>(p, sort.Name))
                    : items.OrderBy(p => EF.Property<object>(p, sort.Name));
            }

            return items;
        }

        public static IOrderedQueryable<T> ThenBySorted<T>(this IOrderedQueryable<T> items, List<FieldSort> sorts) where T : BaseEntity =>
            sorts.Aggregate(items, (current, sort) => sort.SortDirection == "descending"
                ? current.ThenByDescending(p => EF.Property<object>(p, sort.Name))
                : current.ThenBy(p => EF.Property<object>(p, sort.Name)));

        //фильтр для папок
        public static IQueryable<ProtocolFolder> GetFiltered(this IQueryable<ProtocolFolder> items, List<FieldFilter> filters)
        {
            foreach (var fieldFilter in filters.Where(x => x.Items.Any(i => i.Selected)))
            {
                //тут выбираем те фильры, которые выбраны
                if (fieldFilter.Name == nameof(ProtocolFolder.CreatedBy))
                {
                    List<Guid> filterItems = fieldFilter.Items.Where(p => p.Selected)
                        .Select(x=>new Guid(x.Value)).ToList();

                    items = items.Where(x => x.CreatedBy != null && filterItems.Contains(x.CreatedBy.Value));
                }

                if (fieldFilter.Name == nameof(ProtocolFolder.Created))
                {
                    if (fieldFilter.Items.Count(p => p.Selected) != 1)
                    {
                        throw new InvalidOperationException(
                            "Количество фильтров при фильтрации по дате должно быть равно 1");
                    }

                    string[] dates = fieldFilter.Items[0].Value.Split(',');

                    List<DateTime> filterItems = dates
                        .Select(DateTime.Parse).ToList();

                    items = items.Where(x => filterItems[0] < x.Created
                        && x.Created < filterItems[1]);

                    //items = items.Where(x => filterItems.Contains(x.Created.Date));
                }

                if (fieldFilter.Name == nameof(ProtocolFolder.Updated))
                {
                    if (fieldFilter.Items.Count(p => p.Selected) != 1)
                    {
                        throw new InvalidOperationException(
                            "Количество фильтров при фильтрации по дате должно быть равно 1");
                    }

                    string[] dates = fieldFilter.Items[0].Value.Split(',');

                    List<DateTime> filterItems = dates
                        .Select(DateTime.Parse).ToList();

                    items = items.Where(x => filterItems[0] < x.Updated
                        && x.Updated < filterItems[1]);
                }

                //Поиск по имени
                if (fieldFilter.Name == nameof(ProtocolFolder.Name) && fieldFilter.FilterType == Services.Enums.FilterEnum.Search)
                {
                    string searchText =  fieldFilter.Items?.FirstOrDefault()?.Value ?? string.Empty;

                    if (!string.IsNullOrEmpty(searchText))
                    {
                        items = items.Where(x => x.Name.Contains(searchText));
                    }
                }
            }

            return items;
        }
    }
}