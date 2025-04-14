namespace EventsAndAssignments.Services.Enums
{
    /// <summary>
    /// type 0 – выборка, 1 – радиобокс, 2 – выборка с поиском,
    /// 3 дата, 4 дата с поиском, 5 поиск по строке Contains
    /// </summary>
    public enum FilterEnum
    {
        CheckBox = 0,
        RadioBox = 1,
        CheckBoxSearch = 2,
        Data = 3,
        DataSearch = 4,
        Search = 5,
    }
}