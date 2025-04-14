namespace EventsAndAssignments_DataTransfer.Services
{
    /// <summary>
    /// Средство управления сервисом <see cref="DbTransferService"/>
    /// </summary>
    public class DbTransferServiceControl
    {
        /// <summary>
        /// Получить лог работы сервиса
        /// </summary>
        public delegate dynamic GetServiceLog();

        /// <summary>
        /// Делегат метода <see cref="GetServiceLog"/>
        /// </summary>
        public GetServiceLog? GetServiceLogDelegate { get; set; }

        /// <summary>
        /// Индикатор работы сервиса в данный момент
        /// </summary>
        public delegate bool GetServiceActivityStatus();

        /// <summary>
        /// Делегат метода <see cref="GetServiceActivityStatus"/>
        /// </summary>
        public GetServiceActivityStatus? GetServiceActivityStatusDelegate { get; set; }

        /// <summary>
        /// Приостановить работу сервиса
        /// </summary>
        /// <param name="comment">Комментарий о приостановке работы сервиса</param>
        public delegate void SuspendService(string? comment = null);

        /// <summary>
        /// Делегат метода <see cref="SuspendService"/>
        /// </summary>
        public SuspendService? SuspendServiceDelegate { get; set; }

        /// <summary>
        /// Продолжить работу сервиса (если она была приостановлена)
        /// </summary>
        /// <param name="comment">Комментарий о возобновлении работы сервиса</param>
        public delegate void ContinueService(string? comment = null);

        /// <summary>
        /// Делегат метода <see cref="ContinueService"/>
        /// </summary>
        public ContinueService? ContinueServiceDelegate { get; set; }
    }
}
