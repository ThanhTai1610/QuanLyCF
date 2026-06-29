import { api } from './api'

export interface CategoryItem {
  maDanhMuc: number
  tenDanhMuc: string
  maDanhMucCha: number | null
  hinhAnh: string | null
  thuTuHienThi: number
  moTa: string | null
  trangThaiHoatDong: boolean
  soSanPham: number
  loaiDanhMuc: string
  apDungKhungGio: boolean
  gioBatDau: string | null
  gioKetThuc: string | null
  toiThieuChon: number | null
  toiDaChon: number | null
  laNhomKichThuoc: boolean
}

export interface CategoryPayload {
  tenDanhMuc: string
  maDanhMucCha: number | null
  hinhAnh: string | null
  thuTuHienThi: number
  moTa: string | null
  trangThaiHoatDong: boolean
  loaiDanhMuc: string
  apDungKhungGio: boolean
  gioBatDau: string | null
  gioKetThuc: string | null
  toiThieuChon: number | null
  toiDaChon: number | null
  laNhomKichThuoc: boolean
}

export interface CategoryOrderRequest {
  maDanhMuc: number
  thuTuHienThi: number
}

export const categoriesApi = {
  list: async (hienThi?: boolean): Promise<CategoryItem[]> => {
    const query = new URLSearchParams()
    if (hienThi !== undefined) query.append('hienThi', hienThi.toString())
    return await api.get(`/api/categories?${query.toString()}`)
  },
  create: async (payload: CategoryPayload) => {
    return await api.post('/api/categories', payload)
  },
  update: async (id: number, payload: CategoryPayload) => {
    await api.put(`/api/categories/${id}`, payload)
  },
  delete: async (id: number) => {
    await api.del(`/api/categories/${id}`)
  },
  updateOrder: async (requests: CategoryOrderRequest[]) => {
    await api.put('/api/categories/reorder', requests)
  },
  getAllProducts: async () => {
    return await api.get('/api/categories/products')
  },
  getCategoryProducts: async (id: number) => {
    return await api.get(`/api/categories/${id}/products`)
  },
  assignProducts: async (id: number, productIds: number[]) => {
    await api.post(`/api/categories/${id}/assign-products`, { productIds })
  }
}
