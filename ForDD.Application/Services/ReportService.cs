﻿using AutoMapper;
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
            ReportDto[] reportsDto;

            try
            {
                var reports = await _reportRepository.GetAll()
                    .Where(x => x.UserId == userId)
                    .ToArrayAsync();

                reportsDto = reports.Select(r => new ReportDto(r.Id, r.Name, r.Description, r.CreatedAt.ToLongDateString())).ToArray();
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

        /// <inheritdoc/>
        public  async Task<BaseResult<ReportDto>> GetReportByIdAsync(long id)
        {
            ReportDto reportDto;

            try
            {
                var report = await _reportRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == id);

                reportDto = new ReportDto(report.Id, report.Name, report.Description, report.CreatedAt.ToLongDateString());
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
