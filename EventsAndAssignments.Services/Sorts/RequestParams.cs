using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EventsAndAssignments.Services.Sorts
{
    /// <summary>
    /// То, что мы ожидаем получить от frontend для фильрации, пагинации, сортировки
    /// </summary>
    public class RequestParams
    {
        /// <summary>
        /// Количество возвращаемых записей пунктов мероприятий
        /// 100 по умолчанию
        /// </summary>
        [DefaultValue(100)]
        public int Count { get; set; } = 100;

        /// <summary>
        /// Идентификатор Роли текущего пользователя
        /// </summary>
        [DefaultValue("null")]
        public long? RoleId { get; set; }

        /// <summary>
        /// Gets or sets the position identifier.
        /// </summary>
        [DefaultValue("34cbf4a3-a9e8-ed11-b3d6-0050569a16c1")]
        public Guid? PositionId { get; set; }

        /// <summary>
        /// Количество пропускаемых записей пунктов мероприятий
        /// 1 по умолчанию
        /// </summary>
        [DefaultValue(1)]
        public int Page { get; set; }

        [DefaultValue(null)]
        public long? ParentId { get; set; }

        [Range(2000, 2100)]
        public int Year { get; set; }

        /// <summary>
        /// Критерий сортировки
        /// </summary>
        [DefaultValue("[]")]
        public List<FieldSort> Sorts { get; set; } = new();

        /// <summary>
        /// Список фильтров для отбора поручений
        /// </summary>
        [DefaultValue("[]")]
        public List<FieldFilter> Filters { get; set; } = new();
    }
}