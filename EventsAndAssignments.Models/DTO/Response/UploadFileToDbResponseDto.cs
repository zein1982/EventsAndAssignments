namespace EventsAndAssignments.Models.DTO.Response
{
    public class UploadFileToDbResponseDto
    {
        /// <summary>
        /// Идентификатор добавленного файла.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Название файла.
        /// </summary>
        public string OriginName { get; set; } = string.Empty;
    }
}