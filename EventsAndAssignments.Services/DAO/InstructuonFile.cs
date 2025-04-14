namespace EventsAndAssignments.Services.DAO
{
    public class InstructuonFile
    {
        public int Id { get; set; }

        /// <summary>
        /// Оригинальное имя файла
        /// </summary>
        public string? OriginName { get; set; } = string.Empty;

        /// <summary>
        /// Имя файла присвоеное системой
        /// </summary>
        public string? SafetyName { get; set; } = string.Empty;

        /// <summary>
        /// Данные
        /// </summary>
        public byte[] Content { get; set; } = Array.Empty<byte>();
    }
}