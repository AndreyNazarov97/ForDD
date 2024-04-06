using ForDD.Application.Resources;
using ForDD.Domain.Entity;
using ForDD.Domain.Enum;
using ForDD.Domain.Interfaces.Validations;
using ForDD.Domain.Result;

namespace ForDD.Application.Validations
{
    public class ReportValidator : IReportValidator
    {
        public BaseResult CreateValidator(Report report, User user)
        {
            if(report != null)
            {
                return new BaseResult()
                {
                    ErrorMessage = ErrorMessages.ReportAlreadyExist,
                    ErrorCode = (int?)ErrorCodes.ReportAlreadyExist,
                };
            }
            if(user ==  null)
            {
                return new BaseResult()
                {
                    ErrorMessage = ErrorMessages.UserNotFound,
                    ErrorCode = (int)ErrorCodes.UserNotFound,
                };
            }
            return new BaseResult();
        }

        public BaseResult ValidateOnNull(Report model)
        {
            if(model == null)
            {
                return new BaseResult()
                {
                    ErrorMessage = ErrorMessages.ReportNotFound,
                    ErrorCode = (int)ErrorCodes.ReportNotFound,
                };
            }
            return new BaseResult();
        }
    }
}
