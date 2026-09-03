const XLSX = require('xlsx');

const rows = [
  ['1','Xem toàn bộ cài đặt (có quyền)','Cài đặt hệ thống','Đăng nhập Admin (quyền CAIDAT_QUANLY)','1. GET /api/settings','(không)','Trả 200 + toàn bộ cấu hình quán (tên, VAT, phí, ngân hàng...)','Admin','Pass','Tín','20/7/2026',''],
  ['2','Xem cài đặt khi chưa đăng nhập','Cài đặt hệ thống','Không gửi token','1. GET /api/settings (no token)','(không)','Trả 401 Unauthorized','Admin','Pass','Tín','20/7/2026',''],
  ['3','Xem thông tin quán công khai','Cài đặt hệ thống','Không cần đăng nhập','1. GET /api/settings/store-info','(không)','Trả 200 + thông tin công khai (tên, địa chỉ, giờ mở cửa...)','Khách hàng','Pass','Tín','20/7/2026',''],
  ['4','Xem trạng thái bảo trì công khai','Cài đặt hệ thống','Không cần đăng nhập','1. GET /api/settings/maintenance','(không)','Trả 200 + {isMaintenance, message}','Khách hàng','Pass','Tín','20/7/2026',''],
  ['5','Cập nhật cài đặt hợp lệ','Cài đặt hệ thống','Đăng nhập Admin','1. PUT /api/settings với đầy đủ trường hợp lệ','tenQuan="f6", VAT="8", phiDichVu="0", tyLeTichDiem="1"','Trả 204, lưu cài đặt thành công','Admin','Pass','Tín','20/7/2026',''],
  ['6','Cập nhật cài đặt khi chưa đăng nhập','Cài đặt hệ thống','Không gửi token','1. PUT /api/settings (no token)','payload hợp lệ','Trả 401 Unauthorized','Admin','Pass','Tín','20/7/2026',''],
  ['7','Cập nhật với tên quán rỗng','Cài đặt hệ thống','Đăng nhập Admin','1. PUT /api/settings với tenQuan rỗng','tenQuan=""','Trả 400 "Tên quán không được để trống."','Admin','Pass','Tín','20/7/2026',''],
  ['8','Cập nhật VAT lớn hơn 100','Cài đặt hệ thống','Đăng nhập Admin','1. PUT /api/settings với VAT=150','thueVatMacDinh="150"','Trả 400 "Thuế VAT phải là số từ 0 đến 100."','Admin','Pass','Tín','20/7/2026',''],
  ['9','Cập nhật VAT âm','Cài đặt hệ thống','Đăng nhập Admin','1. PUT /api/settings với VAT=-5','thueVatMacDinh="-5"','Trả 400 "Thuế VAT phải là số từ 0 đến 100."','Admin','Pass','Tín','20/7/2026',''],
  ['10','Cập nhật VAT không phải số','Cài đặt hệ thống','Đăng nhập Admin','1. PUT /api/settings với VAT="abc"','thueVatMacDinh="abc"','Trả 400 "Thuế VAT phải là số từ 0 đến 100."','Admin','Pass','Tín','20/7/2026',''],
  ['11','Cập nhật phí dịch vụ không phải số','Cài đặt hệ thống','Đăng nhập Admin','1. PUT /api/settings với phiDichVu="xyz"','phiDichVu="xyz"','Trả 400 "Phí dịch vụ phải là số hợp lệ."','Admin','Pass','Tín','20/7/2026',''],
  ['12','Cập nhật tỷ lệ tích điểm không phải số','Cài đặt hệ thống','Đăng nhập Admin','1. PUT /api/settings với tyLeTichDiem="abc"','tyLeTichDiem="abc"','Trả 400 "Tỷ lệ tích điểm phải là số hợp lệ."','Admin','Pass','Tín','20/7/2026',''],
  ['13','Cập nhật VAT = 0 (biên dưới hợp lệ)','Cài đặt hệ thống','Đăng nhập Admin','1. PUT /api/settings với VAT="0"','thueVatMacDinh="0"','Trả 204, chấp nhận VAT=0','Admin','Pass','Tín','20/7/2026',''],
  ['14','Cập nhật phí dịch vụ ÂM','Cài đặt hệ thống','Đăng nhập Admin','1. PUT /api/settings với phiDichVu="-1000"','phiDichVu="-1000"','Trả 400 chặn phí dịch vụ âm','Admin','Fail','Tín','20/7/2026','BE chỉ kiểm decimal.TryParse (là số), KHÔNG chặn số âm → phí -1000đ lọt vào DB (HTTP 204). Cần thêm điều kiện >= 0 trong SettingService.UpdateAsync.'],
  ['15','Cập nhật tỷ lệ tích điểm ÂM','Cài đặt hệ thống','Đăng nhập Admin','1. PUT /api/settings với tyLeTichDiem="-5"','tyLeTichDiem="-5"','Trả 400 chặn tỷ lệ tích điểm âm','Admin','Fail','Tín','20/7/2026','BE chỉ kiểm decimal.TryParse, KHÔNG chặn số âm → tỷ lệ -5 lọt vào DB (HTTP 204). Tích điểm âm làm sai lệch điểm khách. Cần thêm điều kiện > 0.'],
];

const header = ['Test Case ID','Title','Types','Preconditions','Test Steps','Input Data','Expected Results','Actor','Execution Status','Tester','Date Test','Fixed'];
const data = [header, ...rows];
const ws = XLSX.utils.aoa_to_sheet(data);
ws['!cols'] = [{wch:6},{wch:32},{wch:18},{wch:28},{wch:42},{wch:30},{wch:48},{wch:12},{wch:15},{wch:8},{wch:11},{wch:60}];
const wb = XLSX.utils.book_new();
XLSX.utils.book_append_sheet(wb, ws, 'CaiDatHeThong');
XLSX.writeFile(wb, 'TestCase_CaiDatHeThong.xlsx');
const pass = rows.filter(r=>r[8]==='Pass').length;
const fail = rows.filter(r=>r[8]==='Fail').length;
console.log(`Da tao TestCase_CaiDatHeThong.xlsx voi ${rows.length} test case`);
console.log(`Pass: ${pass} | Fail: ${fail}`);
