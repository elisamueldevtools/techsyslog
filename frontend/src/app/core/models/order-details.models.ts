import { OrderStatus } from './order.models';

export interface AddressDetails {
  cep: string;
  street: string;
  number: string;
  complement: string | null;
  neighborhood: string;
  city: string;
  state: string;
}

export interface OrderDetails {
  id: string;
  orderNumber: string;
  description: string;
  value: number;
  status: OrderStatus;
  address: AddressDetails;
  observation: string | null;
  createdAt: string;
}

export interface DeliveryDetails {
  id: string;
  deliveredAt: string;
  notes: string;
}

export interface OrderDetailsResponse {
  order: OrderDetails;
  deliveries: DeliveryDetails[];
}
