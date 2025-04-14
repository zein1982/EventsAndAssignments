namespace EventsAndAssignments.Services.Interfaces
{
    /// <summary>
    /// Базовый интерфейс сущности
    /// </summary>
    /// <typeparam name="TId">Тип идентификатора сущности</typeparam>
    /// <typeparam name="TAuthor">Тип автора изменений сущности</typeparam>
    internal interface IBaseEntity<TId, TAuthor>
    {
        public TId? Id { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Removed { get; set; }
        public TAuthor? CreatedBy { get; set; }
        public TAuthor? UpdatedBy { get; set; }
    }
}