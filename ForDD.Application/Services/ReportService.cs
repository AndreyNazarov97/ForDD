using ForDD.Application.Resources;
using ForDD.Domain.Dto;
using ForDD.Domain.Entity;
using ForDD.Domain.Enum;
using ForDD.Domain.Interfaces.Repositories;
using ForDD.Domain.Interfaces.Services;
using ForDD.Domain.Result;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ForDD.Application.Services
{
    /// <inheritdoc/>
    public class ReportService : IReportService
    {
        private readonly IBaseRepository<Report> _reportRepository;
        private readonly ILogger _logger;

        public ReportService(IBaseRepository<Report> reportRepository, ILogger logger)
        {
            _reportRepository = reportRepository;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<CollectionResult<ReportDto>> GetReportsAsync(long userId)
        {
            ReportDto[] reports;

            try
            {
                reports = await _reportRepository.GetAll()
                    .Where(x => x.UserId == userId)
                    .Select(r => new ReportDto(r.Id, r.Name, r.Description, r.CreatedAt.ToLongDateString()))
                    .ToArrayAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);

                return new CollectionResult<ReportDto>()
                {
                    ErrorMessage = ErrorMessages.InternalServerError,
                    ErrorCode = ((int)ErrorCodes.InternalServerError)
                };
            }

            if (!reports.Any())
            {
                _logger.Warning(ErrorMessages.ReportsNotFound, reports.Length);

                return new CollectionResult<ReportDto>()
                {
                    ErrorMessage = ErrorMessages.ReportsNotFound,
                    ErrorCode = ((int)ErrorCodes.ReportsNotFound)
                };
            }
            return new CollectionResult<ReportDto>()
            {
                Data = reports,
                Count = reports.Length
            };
        }

        /// <inheritdoc/>
        public async Task<BaseResult<ReportDto>> GetReportByIdAsync(long id)
        {
            ReportDto report;

            try
            {
                report = await _reportRepository.GetAll()
                    .Select(r => new ReportDto(r.Id, r.Name, r.Description, r.CreatedAt.ToLongDateString()))
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
            catch(Exception ex) 
            {
                _logger.Error(ex, ex.Message);

                return new BaseResult<ReportDto>()
                {
                    ErrorMessage = ErrorMessages.InternalServerError,
                    ErrorCode = ((int)ErrorCodes.InternalServerError)
                };
            }

            if (report == null)
            {
                _logger.Warning($"Report with {id} not found");

                return new BaseResult<ReportDto>()
                {
                    ErrorMessage = ErrorMessages.ReportNotFound,
                    ErrorCode = ((int)ErrorCodes.ReportNotFound)
                };
            }

        }
    }
}
