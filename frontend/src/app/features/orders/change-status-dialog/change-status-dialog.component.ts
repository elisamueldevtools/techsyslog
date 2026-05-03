import { Component, EventEmitter, Input, Output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { OrderListItem, OrderStatus } from '../../../core/models/order.models';

@Component({
  selector: 'app-change-status-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './change-status-dialog.component.html',
  styleUrl: './change-status-dialog.component.scss'
})
export class ChangeStatusDialogComponent {
  @Input({ required: true }) order!: OrderListItem;
  @Output() confirm = new EventEmitter<OrderStatus>();
  @Output() cancel = new EventEmitter<void>();

  protected readonly selected = signal<OrderStatus | null>(null);

  protected readonly statusLabel: Record<OrderStatus, string> = {
    Created: 'Criado',
    Processing: 'Processando',
    Shipped: 'Enviado',
    Delivered: 'Entregue',
    Cancelled: 'Cancelado'
  };

  private readonly transitions: Record<OrderStatus, OrderStatus[]> = {
    Created:    ['Processing', 'Cancelled'],
    Processing: ['Shipped', 'Cancelled'],
    Shipped:    ['Delivered', 'Cancelled'],
    Delivered:  [],
    Cancelled:  []
  };

  protected nextStatuses(): OrderStatus[] {
    return this.transitions[this.order.status];
  }

  protected onConfirm(): void {
    const s = this.selected();
    if (s) this.confirm.emit(s);
  }

  protected onBackdropClick(): void {
    this.cancel.emit();
  }
}
