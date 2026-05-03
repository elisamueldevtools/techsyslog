import { OrderStatus } from './order.models';

export interface DashboardOrderItem {
  id: string;
  orderNumber: string;
  value: number;
  createdAt: string;
}

export interface DashboardResponse {
  month: number;
  year: number;
  counters: Record<OrderStatus, number>;
  grids: Record<OrderStatus, DashboardOrderItem[]>;
}
