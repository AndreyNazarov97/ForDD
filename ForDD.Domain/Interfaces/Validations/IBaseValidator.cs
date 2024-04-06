using ForDD.Domain.Result;

namespace ForDD.Domain.Interfaces.Validations
{
    public interface IBaseValidator<in T> where T : class
    {
        BaseResult ValidateOnNull (T model);
    }
    
}
