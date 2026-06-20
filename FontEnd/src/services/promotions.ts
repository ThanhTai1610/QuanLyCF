// Goi API khuyen mai / voucher. Token JWT da duoc api.ts tu gan.
import { api } from './api'

export type LoaiGiamGia = 'PhanTram' | 'TienMat'

export interface Promotion {
  maKhuyenMai: number
  maGiamGia: string | null
  tenChuongTrinh: string
  loaiGiamGia: LoaiGiamGia
  giaTriGiam: number
  giamToiDa: number | null
  donToiThieu: number | null
  soLuongGioiHan: number | null
  soLuongDaDung: number
  ngayBatDau: string | null
  ngayKetThuc: string | null
  moTa: string | null
  trangThaiHoatDong: boolean
}

export interface SavePromotionBody {
  maGiamGia: string | null
  tenChuongTrinh: string
  loaiGiamGia: LoaiGiamGia
  giaTriGiam: number
  giamToiDa: number | null
  donToiThieu: number | null
  soLuongGioiHan: number | null
  ngayBatDau: string | null
  ngayKetThuc: string | null
  moTa: string | null
  trangThaiHoatDong: boolean
}

export interface ApplyResult {
  maKhuyenMai: number
  tenChuongTrinh: string
  tienGiam: number
  thanhTienSau: number
}

export const promotionsApi = {
  list: () => api.get<Promotion[]>('/api/promotions'),
  active: () => api.get<Promotion[]>('/api/promotions/active'),
  create: (b: SavePromotionBody) => api.post<{ maKhuyenMai: number }>('/api/promotions', b),
  update: (id: number, b: SavePromotionBody) => api.put<void>(`/api/promotions/${id}`, b),
  remove: (id: number) => api.del<void>(`/api/promotions/${id}`),
  // Xem trước giảm giá: theo mã nhập tay HOẶC chọn chương trình
  preview: (tongTien: number, opts: { maKhuyenMai?: number; code?: string }) =>
    api.post<ApplyResult>('/api/promotions/preview', {
      maKhuyenMai: opts.maKhuyenMai ?? null, code: opts.code ?? null, tongTien,
    }),
}
