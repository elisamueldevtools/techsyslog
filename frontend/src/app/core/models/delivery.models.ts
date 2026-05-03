export interface CreateDeliveryRequest {
  orderId: string;
  deliveredAt: string;
  notes?: string;
}
