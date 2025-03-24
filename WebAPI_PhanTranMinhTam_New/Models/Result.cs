namespace WebAPI_PhanTranMinhTam_New.Models
{
    public class Result
    {
        public bool IsSuccess { get; set; }  // Chỉ định trạng thái thành công
        public string ErrorMessage { get; set; }  // Thông báo lỗi nếu có
        public object Data { get; set; }  // Dữ liệu trả về nếu có

        public static Result Success() => new() { IsSuccess = true };
        public static Result Failure(string message) => new() { IsSuccess = false, ErrorMessage = message };
    }
}
