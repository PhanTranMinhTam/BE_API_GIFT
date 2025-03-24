using FluentValidation;
using WebAPI_PhanTranMinhTam_New.Models;

namespace WebAPI_PhanTranMinhTam_New.Validations
{
    public class UserValidation : AbstractValidator<CreateDTO>
    {
        //private readonly IRepositoryWrapper _repositoryWrapper;
        //public UserValidation(IRepositoryWrapper repositoryWrapper)
        //{
        //    _repositoryWrapper = repositoryWrapper;

        //    RuleFor(x => x.Name)
        //        .NotEmpty().WithMessage("Name is required.")
        //        .MaximumLength(100).WithMessage("Name must not exceed 100 characters.")
        //        .Matches(@"^[a-zA-Z0-9\s]+$").WithMessage("Name can only contain letters, numbers, and spaces.")
        //        .CustomAsync(async (name, context, cancellationToken) =>
        //        {
        //            IQueryable<Data.User> existingName = _repositoryWrapper.User.FindByCondition(u => u.Name == name);
        //            if (existingName.Any())
        //            {
        //                context.AddFailure("Name", "A user with this name already exists.");
        //            }
        //        });

        //    RuleFor(x => x.Email)
        //        .NotEmpty().WithMessage("Email is required.")
        //        .EmailAddress().WithMessage("Invalid email address.")
        //        .CustomAsync(async (email, context, cancellationToken) =>
        //        {
        //            IQueryable<Data.User> existingEmail = _repositoryWrapper.User.FindByCondition(u => u.Email == email);
        //            if (existingEmail.Any())
        //            {
        //                context.AddFailure("Email", "A user with this email already exists.");
        //            }
        //        });

        //    RuleFor(x => x.PhoneNumber)
        //        .NotEmpty().WithMessage("Phone number is required.")
        //        .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number.")
        //        .CustomAsync(async (phoneNumber, context, cancellationToken) =>
        //        {
        //            IQueryable<Data.User> existingPhone = _repositoryWrapper.User.FindByCondition(u => u.PhoneNumber == phoneNumber);
        //            if (existingPhone.Any())
        //            {
        //                context.AddFailure("PhoneNumber", "A user with this phone number already exists.");
        //            }
        //        });

        //    RuleFor(x => x.Password)
        //        .NotEmpty().WithMessage("Password is required.")
        //        .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
        //        .MaximumLength(100).WithMessage("Password must not exceed 100 characters.");

        //    RuleFor(x => x.BirthDate)
        //        .NotEmpty().WithMessage("Birth Date is required.")
        //        .LessThan(DateTime.Now).WithMessage("Birth Date must be in the past.");

        //    //RuleFor(x => x.Point)
        //    //    .Matches(@"^\d+$").WithMessage("Point must be a positive integer.");



        //    RuleFor(x => x.Image)
        //        .NotNull().WithMessage("Image is required.")
        //        .Must(file => file.Length > 0).WithMessage("Image file is empty.")
        //        .Must(file => file.ContentType.StartsWith("image/")).WithMessage("The file must be an image.")
        //        .Must(file => file.Length <= 5 * 1024 * 1024) // Optional: Limit file size to 5 MB
        //        .WithMessage("Image file size must be less than or equal to 5 MB.");
        //}
    }
}
