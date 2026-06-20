// Goi API cho Module Catalog (SanPham, DanhMuc). JWT token tu dong duoc them boi api.ts.
import { api } from './api'

export interface SizeDto {
  tenKichCo: string
  giaCongThem: number
  trangThaiHoatDong: boolean
}

export interface ProductListItem {
  maSanPham: number
  tenSanPham: string
  maDanhMuc: number | null
  tenDanhMuc: string | null
  giaBan: number
  giaVonDuKien: number | null
  hinhAnh: string | null
  kieuMon: string
  laMonNoiBat: boolean
  trangThaiBan: boolean
}

export interface ProductDetail {
  maSanPham: number
  tenSanPham: string
  maDanhMuc: number | null
  maVach_SKU?: string
  giaBan: number
  giaVonDuKien?: number
  hinhAnh?: string
  moTa?: string
  luongCalo?: number
  thoiGianChuanBiPhut?: number
  laMonNoiBat: boolean
  kieuMon: string
  trangThaiBan: boolean
  kichCos: SizeDto[]
}

export interface CreateProductRequest {
  tenSanPham: string
  maDanhMuc: number | null
  maVach_SKU: string | null
  giaBan: number
  giaVonDuKien: number | null
  hinhAnh: string | null
  moTa: string | null
  luongCalo: number | null
  thoiGianChuanBiPhut: number | null
  laMonNoiBat: boolean
  kieuMon: string
  trangThaiBan: boolean
  kichCos: SizeDto[]
}

export interface UpdateProductRequest extends CreateProductRequest {}

export interface CategoryItem {
  maDanhMuc: number
  tenDanhMuc: string
  maDanhMucCha: number | null
  hinhAnh: string | null
  thuTuHienThi: number
  moTa: string | null
  trangThaiHoatDong: boolean
  soLuongSanPham: number
}

export const productsApi = {
  list: (params?: { q?: string; maDanhMuc?: number; dangBan?: boolean }) => {
    const query = new URLSearchParams()
    if (params?.q) query.append('q', params.q)
    if (params?.maDanhMuc) query.append('maDanhMuc', params.maDanhMuc.toString())
    if (params?.dangBan !== undefined) query.append('dangBan', params.dangBan.toString())
    return api.get<ProductListItem[]>(`/api/products?${query.toString()}`)
  },
  get: (id: number) => api.get<ProductDetail>(`/api/products/${id}`),
  create: (body: CreateProductRequest) => api.post<ProductDetail>('/api/products', body),
  update: (id: number, body: UpdateProductRequest) => api.put<void>(`/api/products/${id}`, body),
  delete: (id: number) => api.del<void>(`/api/products/${id}`),

  listCategories: (params?: { hienThi?: boolean }) => {
    const query = new URLSearchParams()
    if (params?.hienThi !== undefined) query.append('hienThi', params.hienThi.toString())
    return api.get<CategoryItem[]>(`/api/categories?${query.toString()}`)
  }
}
