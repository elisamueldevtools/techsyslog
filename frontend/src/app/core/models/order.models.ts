export type OrderStatus = 'Created' | 'Processing' | 'Shipped' | 'Delivered' | 'Cancelled';

export interface CreateOrderRequest {
  orderNumber: string;
  description: string;
  value: number;
  cep: string;
  number: string;
  complement?: string;
  observation?: string;
}

export interface CreateOrderResponse {
  id: string;
  status: OrderStatus;
}

export interface OrderListItem {
  id: string;
  orderNumber: string;
  status: OrderStatus;
  value: number;
  createdAt: string;
}

export interface UpdateOrderStatusRequest {
  status: OrderStatus;
}
