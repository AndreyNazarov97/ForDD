using ForDD.Domain.Entity;
using ForDD.Domain.Result;

namespace ForDD.Domain.Interfaces.Validations
{
    public interface IReportValidator : IBaseValidator<Report>    
    {
        /// <summary>
        /// Проверяется наличие отчета, если отчет с таким именем есть в БД, то создать такой же отчет нельзя
        /// Проверяется пользователь, есди пользователь с UserId не найден, такого пользователя нет
        /// </summary>
        /// <param name="report"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        BaseResult CreateValidator(Report report, User user);
    }
}
