using UserManagement.Domain.ViewModels;

namespace UserManagement.Services.Interfaces;

public interface IValidationService
{
    public List<ValidationError> ValidateAuthModel(AuthViewModel authViewModel);
}
