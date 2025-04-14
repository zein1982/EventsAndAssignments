using EventsAndAssignments.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventsAndAssignments.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class DataSeedController : ControllerBase
    {
        private readonly IDataSeedService _dataSeedService;

        public DataSeedController(IDataSeedService dataSeedService)
        {
            _dataSeedService = dataSeedService;
        }

        /// <summary>
        /// Возвращает поручение по идентификатору
        /// </summary>
        /// <remarks>
        /// Заполнить БД начальным набором данных. Метод заполняет данными только если таблица пустая.
        /// Пароль по умолчанию только вставляет данные если их нет, если введен пароль,
        /// то БД будет удалена и вновь создана с данными по умолчанию.
        /// </remarks>
        [HttpGet]
        [Route(nameof(Seed))]
        public ActionResult Seed()
        {
            _dataSeedService.Seed();

            return Ok();
        }
    }
}