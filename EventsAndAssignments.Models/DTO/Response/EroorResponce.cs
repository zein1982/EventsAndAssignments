namespace EventsAndAssignments.Models.DTO.Response
{
    /// <summary>
    /// Содержит инфомрацию о возникшей ошибке
    /// </summary>
    public class ErrorResponse
    {
        public ErrorResponse(string error)
        {
            Error = error;
        }

        /// <summary>
        /// Текст ошибки
        /// </summary>
        public string Error { get; set; }
    }
}