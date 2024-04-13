using FluentValidation;
using ForDD.Domain.Dto.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForDD.Application.Validations.FluentValidations.Report
{
    internal class UpdateReportValidator : AbstractValidator<UpdateReportDto>
    {
        public UpdateReportValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
