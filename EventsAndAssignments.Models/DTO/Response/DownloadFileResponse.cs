namespace EventsAndAssignments.Models.DTO.Response
{
    public class DownloadFileResponse : BaseDTO
    {
        /// <summary>
        /// Оригинальное имя файла
        /// </summary>
        public string? OriginName { get; set; } = string.Empty;

        /// <summary>
        /// Данные
        /// </summary>
        public byte[] Content { get; set; } = Array.Empty<byte>();
    }
}