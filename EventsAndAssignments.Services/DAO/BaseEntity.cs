namespace EventsAndAssignments.Services.DAO
{
    /// <summary>
    /// Базовая сущность, c идентификатором типа <see cref="long"/> и автором изменений типа <see cref="Guid"/>
    /// </summary>
    //public abstract class BaseEntity : IBaseEntity<long?, Guid?>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Идентификатор сущности
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Если сущность не создана, <see cref="Created"/> будет <see cref="null"/>,
        /// если создана - время создания
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Если сущность не была обновлена, <see cref="Updated"/> будет <see cref="Created"/>, //TODO Решили с командой что будет заполнена при создании
        /// если обновлена - дату последнего обновления
        /// </summary>
        public DateTime? Updated { get; set; }

        /// <summary>
        /// Если сущность не помечена удаленной, <see cref="Removed"/> будет <see cref="null"/>,
        /// если помечена удаленной - время пометки, что она удалена
        /// </summary>
        public DateTime? Removed { get; set; }

        /// <summary>
        /// Идентификатор пользователя, создавшего сущность
        /// </summary>
        public Guid? CreatedBy { get; set; }

        /// <summary>
        /// Идентификатор пользователя, который последний вносил изменения в сущность
        /// </summary>
        public Guid? UpdatedBy { get; set; }
    }
}