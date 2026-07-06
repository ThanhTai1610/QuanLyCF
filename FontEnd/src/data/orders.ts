export type OrderStatus = "pending" | "preparing" | "ready" | "done" | "cancelled";

export interface OrderItem {
  name: string;
  qty: number;
  price: number;
  note?: string;
  /** Trạng thái pha chế tại bếp */
  done?: boolean;
  /** Nhân viên được phân công làm món */
  assignee?: string;
  /** Báo hết nguyên liệu */
  outOfStock?: boolean;
}

export interface Order {
  id: string;
  table: string;
  items: OrderItem[];
  total: number;
  status: OrderStatus;
  /** Giờ tạo dạng hiển thị "HH:MM" */
  createdAt: string;
  /** Mốc thời gian (ms) để tính bộ đếm tại bếp */
  createdTs: number;
  customer?: string;
  /** Đã thanh toán chưa */
  paid?: boolean;
  /** Phương thức thanh toán */
  paymentMethod?: string;
  /** Đơn ưu tiên / Khẩn cấp */
  isPriority?: boolean;
}

/** Mốc thời gian gốc để seed dữ liệu mẫu (cố định theo lúc khởi tạo app) */
const seedNow = Date.now();
const minsAgo = (m: number) => seedNow - m * 60 * 1000;

export const mockOrders: Order[] = [
  {
    id: "DH-2041",
    table: "Bàn 5",
    items: [
      { name: "Cappuccino", qty: 2, price: 45000 },
      { name: "Croissant bơ", qty: 1, price: 32000 },
    ],
    total: 122000,
    status: "pending",
    createdAt: "10:24",
    createdTs: minsAgo(8),
  },
  {
    id: "DH-2040",
    table: "Bàn 12",
    items: [
      { name: "Matcha đá xay", qty: 1, price: 55000, assignee: "Lan", done: true },
      { name: "Bánh Tiramisu", qty: 1, price: 48000, assignee: "Huy" },
    ],
    total: 103000,
    status: "preparing",
    createdAt: "10:18",
    createdTs: minsAgo(14),
  },
  {
    id: "DH-2039",
    table: "Bàn 3",
    items: [
      { name: "Cà phê sữa đá", qty: 3, price: 35000, assignee: "Minh" },
    ],
    total: 105000,
    status: "preparing",
    createdAt: "10:12",
    createdTs: minsAgo(20),
  },
  {
    id: "DH-2038",
    table: "Mang về - #038",
    items: [
      { name: "Trà đào cam sả", qty: 2, price: 45000 },
      { name: "Cheesecake dâu", qty: 1, price: 45000 },
    ],
    total: 135000,
    status: "done",
    createdAt: "10:05",
    createdTs: minsAgo(27),
  },
  {
    id: "DH-2037",
    table: "Mang về - #037",
    items: [
      { name: "Espresso", qty: 1, price: 35000 },
    ],
    total: 35000,
    status: "done",
    createdAt: "09:58",
    createdTs: minsAgo(34),
  },
  {
    id: "DH-2036",
    table: "Bàn 7",
    items: [
      { name: "Cookies & Cream", qty: 2, price: 55000 },
    ],
    total: 110000,
    status: "cancelled",
    createdAt: "09:42",
    createdTs: minsAgo(50),
  },
  {
    id: "DH-2035",
    table: "Bàn 4",
    items: [
      { name: "Latte nghệ thuật", qty: 1, price: 49000 },
      { name: "Croissant bơ", qty: 2, price: 32000 },
    ],
    total: 113000,
    status: "done",
    createdAt: "09:30",
    createdTs: minsAgo(62),
  },
];

export const statusMeta: Record<OrderStatus, { label: string; className: string }> = {
  pending:    { label: "Chờ xác nhận", className: "bg-warning/15 text-warning border border-warning/30" },
  preparing:  { label: "Đang pha chế", className: "bg-caramel/15 text-caramel border border-caramel/30" },
  ready:      { label: "Sẵn sàng (Chờ lấy)", className: "bg-success/15 text-success border border-success/30 animate-pulse" },
  done:       { label: "Hoàn thành",   className: "bg-success/15 text-success border border-success/30" },
  cancelled:  { label: "Đã hủy",       className: "bg-destructive/15 text-destructive border border-destructive/30" },
};
