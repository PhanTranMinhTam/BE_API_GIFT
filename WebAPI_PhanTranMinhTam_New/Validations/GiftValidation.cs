using FluentValidation;
using WebAPI_PhanTranMinhTam_New.Models;

namespace WebAPI_PhanTranMinhTam_New.Validations
{
    public class GiftValidation : AbstractValidator<CreateGiftDTO>
    {
        //private readonly IRepositoryWrapper _repositoryWrapper;

        //public GiftValidation(IRepositoryWrapper repositoryWrapper)
        //{
        //    _repositoryWrapper = repositoryWrapper;

        //    // Kiểm tra Name không được rỗng và có độ dài tối thiểu
        //    RuleFor(gift => gift.Name)
        //        .NotEmpty().WithMessage("Tên quà tặng không được để trống.")
        //        .MinimumLength(3).WithMessage("Tên quà tặng phải có ít nhất 3 ký tự.");

        //    // Kiểm tra Price không được rỗng và là số hợp lệ
        //    RuleFor(gift => gift.Price)
        //        .NotEmpty().WithMessage("Giá quà tặng không được để trống.")
        //        .Matches(@"^\d+(\.\d{1,2})?$").WithMessage("Giá quà tặng phải là số và có thể chứa tối đa 2 chữ số thập phân.");

        //    // Kiểm tra DateEnd lớn hơn DateStart
        //    RuleFor(gift => gift.DateEnd)
        //        .GreaterThan(gift => gift.DateStart)
        //        .When(gift => gift.DateStart.HasValue && gift.DateEnd.HasValue)
        //        .WithMessage("Ngày kết thúc phải sau ngày bắt đầu.");

        //    // Kiểm tra Quantity lớn hơn hoặc bằng 0
        //    RuleFor(gift => gift.Quantity)
        //        .GreaterThanOrEqualTo(0).WithMessage("Số lượng quà tặng phải lớn hơn hoặc bằng 0.");

        //    // Kiểm tra QuantityRequired lớn hơn hoặc bằng 0
        //    RuleFor(gift => gift.QuantityRequired)
        //        .GreaterThanOrEqualTo(0).WithMessage("Số lượng yêu cầu phải lớn hơn hoặc bằng 0.");

        //    // Kiểm tra nếu Idparents không null, nó phải tồn tại trong cơ sở dữ liệu
        //    RuleFor(gift => gift.Idparents)
        //        .MustAsync(async (id, cancellation) =>
        //        {
        //            if (id.HasValue)
        //            {
        //                return await _repositoryWrapper.Gift.ExistsAsync(g => g.IdGift == id.Value);
        //            }
        //            return true;
        //        })
        //        .WithMessage("Id cha không tồn tại.");

        //    // Kiểm tra RoleId có tồn tại trong cơ sở dữ liệu
        //    RuleFor(gift => gift.RoleId)
        //        .MustAsync(async (roleId, cancellation) =>
        //            await _repositoryWrapper.Role.ExistsAsync(r => r.IdRoles == roleId))
        //        .WithMessage("Role không tồn tại.");
        //}
    }
}
