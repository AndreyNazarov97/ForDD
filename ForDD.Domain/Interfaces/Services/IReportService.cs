using ForDD.Domain.Dto;
using ForDD.Domain.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForDD.Domain.Interfaces.Services
{
    /// <summary>
    /// Сервис отвечающий за работу с доменной частью отчёта(Report)
    /// </summary>
    public interface IReportService
    {
        /// <summary>
        /// Получение всех отчетов пользователя
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <returns></returns>
        public Task<CollectionResult<ReportDto>> GetReportsAsync(long userId);

        /// <summary>
        /// Получение отчета по идентификатору
        /// </summary>
        /// <param name="id">идентификатор</param>
        /// <returns></returns>
        public Task<BaseResult<ReportDto>> GetReportByIdAsync(long id);



    }
}
