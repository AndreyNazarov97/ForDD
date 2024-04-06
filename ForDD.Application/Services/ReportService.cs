using AutoMapper;
using ForDD.Application.Resources;
using ForDD.Domain.Dto.Report;
using ForDD.Domain.Entity;
using ForDD.Domain.Enum;
using ForDD.Domain.Interfaces.Repositories;
using ForDD.Domain.Interfaces.Services;
using ForDD.Domain.Interfaces.Validations;
using ForDD.Domain.Result;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ForDD.Application.Services
{
    /// <inheritdoc/>
    public class ReportService : IReportService
    {
        private readonly IBaseRepository<Report> _reportRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IReportValidator _reportValidator;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public ReportService(IBaseRepository<Report> reportRepository,
            IBaseRepository<User> userRepository,
            IReportValidator reportValidator,
            ILogger logger, 
            IMapper mapper )
        {
            _reportRepository = reportRepository;
            _logger = logger;
            _userRepository = userRepository;
            _reportValidator = reportValidator;
            _mapper = mapper;
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

            return new BaseResult<ReportDto>()
            {
                Data = report,
            };
        }

        /// <inheritdoc/>
        public async Task<BaseResult<ReportDto>> CreateReportByIdAsync(CreateReportDto dto)
        {
            try
            {
                var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.UserId);
                var report = await _reportRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.Name);
                var result = _reportValidator.CreateValidator(report, user);
                if (!result.IsSuccess)
                {
                    return new BaseResult<ReportDto>()
                    {
                        ErrorMessage = result.ErrorMessage,
                        ErrorCode = result.ErrorCode,
                    };
                }
                report = new Report()
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    UserId = user.Id
                };
                await _reportRepository.CreateAsync(report);

                return new BaseResult<ReportDto>()
                {
                    Data = _mapper.Map<ReportDto>(report),
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);

                return new BaseResult<ReportDto>()
                {
                    ErrorMessage = ErrorMessages.InternalServerError,
                    ErrorCode = ((int)ErrorCodes.InternalServerError)
                };
            }

        }

        /// <inheritdoc/>
        public async Task<BaseResult<ReportDto>> DeleteReportByIdAsync(long id)
        {
            try
            {
                var report = await _reportRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
                var result = _reportValidator.ValidateOnNull(report);
                if (!result.IsSuccess)
                {
                    return new BaseResult<ReportDto>()
                    {
                        ErrorMessage = result.ErrorMessage,
                        ErrorCode = result.ErrorCode,
                    };
                }
                await _reportRepository.DeleteAsync(report);
                return new BaseResult<ReportDto>()
                {
                    Data = _mapper.Map<ReportDto>(report),
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);

                return new BaseResult<ReportDto>()
                {
                    ErrorMessage = ErrorMessages.InternalServerError,
                    ErrorCode = ((int)ErrorCodes.InternalServerError)
                };
            }
        }

        /// <inheritdoc/>
        public async Task<BaseResult<ReportDto>> UpdateReportByIdAsync(UpdateReportDto dto)
        {
            try
            {
                var report = await _reportRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id);
                var result = _reportValidator.ValidateOnNull(report);
                if (!result.IsSuccess)
                {
                    return new BaseResult<ReportDto>()
                    {
                        ErrorMessage = result.ErrorMessage,
                        ErrorCode = result.ErrorCode,
                    };
                }

                report.Name = dto.Name;
                report.Description = dto.Description;

                await _reportRepository.UpdateAsync(report);

                return new BaseResult<ReportDto>()
                {
                    Data = _mapper.Map<ReportDto>(report),
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);

                return new BaseResult<ReportDto>()
                {
                    ErrorMessage = ErrorMessages.InternalServerError,
                    ErrorCode = ((int)ErrorCodes.InternalServerError)
                };
            }

        }
    }
}
