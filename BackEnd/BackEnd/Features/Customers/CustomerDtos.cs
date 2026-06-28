using System.ComponentModel.DataAnnotations;

namespace BackEnd.Features.Customers;

public record CustomerHistoryItem(
    string Date,
    string Note,
    int Points
);

public record CustomerListItem(
    int Id,
    string Name,
    string Phone,
    string? Email,
    string Tier,
    int Points,
    decimal TotalSpend,
    string LastVisit,
    int Visits
);

public record CustomerDetail(
    int Id,
    string Name,
    string Phone,
    string? Email,
    string? Note,
    string Tier,
    int Points,
    decimal TotalSpend,
    string LastVisit,
    int Visits,
    IEnumerable<CustomerHistoryItem> History
);

public record CreateCustomerRequest(
    [Required(ErrorMessage = "Họ tên là bắt buộc.")]
    [MaxLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự.")]
    string Name,

    [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    [MaxLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự.")]
    string Phone,

    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [MaxLength(100, ErrorMessage = "Email tối đa 100 ký tự.")]
    string? Email,

    [MaxLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự.")]
    string? Note
);

public record UpdateCustomerRequest(
    [Required(ErrorMessage = "Họ tên là bắt buộc.")]
    [MaxLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự.")]
    string Name,

    [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    [MaxLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự.")]
    string Phone,

    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [MaxLength(100, ErrorMessage = "Email tối đa 100 ký tự.")]
    string? Email,

    [MaxLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự.")]
    string? Note
);

public record RewardDto(
    int Id,
    string Name,
    int Cost,
    string? Description
);
