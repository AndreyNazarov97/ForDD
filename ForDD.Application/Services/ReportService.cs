using AutoMapper;
using ForDD.Application.Extensions;
using ForDD.Application.Resources;
using ForDD.Domain.Dto.Report;
using ForDD.Domain.Entity;
using ForDD.Domain.Enum;
using ForDD.Domain.Interfaces.Repositories;
using ForDD.Domain.Interfaces.Services;
using ForDD.Domain.Interfaces.Validations;
using ForDD.Domain.Result;
using ForDD.Domain.Settings;
using ForDD.Producer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Serilog;

namespace ForDD.Application.Services
{
    /// <inheritdoc/>
    public class ReportService : IReportService
    {
        private readonly IBaseRepository<Report> _reportRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IReportValidator _reportValidator;
        private readonly IMessageProducer _messageProducer;
        private readonly IOptions<RabbitMqSettings> _rabbitMqSettings;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;

        public ReportService(IBaseRepository<Report> reportRepository,
            IBaseRepository<User> userRepository,
            IReportValidator reportValidator,
            ILogger logger,
            IMapper mapper,
            IMessageProducer messageProducer,
            IOptions<RabbitMqSettings> rabbitMqSettings,
            IDistributedCache cache)
        {
            _reportRepository = reportRepository;
            _logger = logger;
            _userRepository = userRepository;
            _reportValidator = reportValidator;
            _mapper = mapper;
            _messageProducer = messageProducer;
            _rabbitMqSettings = rabbitMqSettings;
            _cache = cache;
        }

        /// <inheritdoc/>
        public async Task<CollectionResult<ReportDto>> GetReportsAsync(long userId)
        {
            ReportDto[] reportsDto;
            
            var recordKey = $"User_Reports_Id{userId}";

            var reportsCache = await _cache.GetRecordAsync<ReportDto[]>(recordKey);

            if (reportsCache == null)
            {
                try
                {
                    var reports = await _reportRepository.GetAll()
                        .Where(x => x.UserId == userId)
                        .ToArrayAsync();

                    reportsDto = reports.Select(r => new ReportDto(r.Id, r.Name, r.Description, r.CreatedAt.ToLongDateString())).ToArray();
                    await _cache.SetRecordAsync<ReportDto[]>(recordKey, reportsDto);
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

                if (!reportsDto.Any())
                {
                    _logger.Warning(ErrorMessages.ReportsNotFound, reportsDto.Length);

                    return new CollectionResult<ReportDto>()
                    {
                        ErrorMessage = ErrorMessages.ReportsNotFound,
                        ErrorCode = ((int)ErrorCodes.ReportsNotFound)
                    };
                }
                return new CollectionResult<ReportDto>()
                {
                    Data = reportsDto,
                    Count = reportsDto.Length
                };
            }
            else
            {
                return new CollectionResult<ReportDto>()
                {
                    Data = reportsCache,
                    Count = reportsCache.Length
                };
            }
        }

        /// <inheritdoc/>
        public  async Task<BaseResult<ReportDto>> GetReportByIdAsync(long id)
        {
            ReportDto reportDto;
            var recordKey = $"Report_Id{id}";
            var reportCache = await _cache.GetRecordAsync<ReportDto>(recordKey);

            if (reportCache == null)
            {


                try
                {
                    var report = await _reportRepository.GetAll()
                        .FirstOrDefaultAsync(x => x.Id == id);

                    reportDto = new ReportDto(report.Id, report.Name, report.Description, report.CreatedAt.ToLongDateString());

                    await _cache.SetRecordAsync(recordKey, reportDto);
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

                if (reportDto == null)
                {
                    _logger.Warning($"Report with id:{id} not found", id);

                    return new BaseResult<ReportDto>()
                    {
                        ErrorMessage = ErrorMessages.ReportNotFound,
                        ErrorCode = ((int)ErrorCodes.ReportNotFound)
                    };
                }
                return new BaseResult<ReportDto>()
                {
                    Data = reportDto,
                };
            }

            else
            {
                return new BaseResult<ReportDto>()
                {
                    Data = reportCache,
                };
            }
            
        }

        /// <inheritdoc/>
        public async Task<BaseResult<ReportDto>> CreateReportAsync(CreateReportDto dto)
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
                UserId = user.Id,
            };
            await _reportRepository.CreateAsync(report);
            await _reportRepository.SaveChangesAsync();

            _messageProducer.SendMessage(report, _rabbitMqSettings.Value.RoutingKey, _rabbitMqSettings.Value.ExchangeName);

            return new BaseResult<ReportDto>()
            {
                Data = _mapper.Map<ReportDto>(report),
            };

        }

        /// <inheritdoc/>
        public async Task<BaseResult<ReportDto>> DeleteReportByIdAsync(long id)
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
            _reportRepository.Delete(report);
            await _reportRepository.SaveChangesAsync();

            return new BaseResult<ReportDto>()
            {
                Data = _mapper.Map<ReportDto>(report),
            };
        }

        /// <inheritdoc/>
        public async Task<BaseResult<ReportDto>> UpdateReportAsync(UpdateReportDto dto)
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

            var updatedReport = _reportRepository.Update(report);
            await _reportRepository.SaveChangesAsync();

            return new BaseResult<ReportDto>()
            {
                Data = _mapper.Map<ReportDto>(report),
            };

        }
    }
}
