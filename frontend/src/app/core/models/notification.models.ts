export type NotificationType = 'OrderCreated' | 'OrderStatusChanged' | 'DeliveryRegistered';

export interface NotificationItem {
  id: string;
  type: NotificationType;
  message: string;
  read: boolean;
  createdAt: string;
}
