using FluentValidation;
using ForDD.Domain.Dto.Report;

namespace ForDD.Application.Validations.FluentValidations.Report
{
    public class CreateReportValidator : AbstractValidator<CreateReportDto>
    {
        public CreateReportValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Description).NotEmpty();

        }


    }
}
