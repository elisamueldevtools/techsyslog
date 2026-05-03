import { Component, EventEmitter, Input, OnInit, Output, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrderService } from '../../../core/services/order.service';
import { OrderDetailsResponse } from '../../../core/models/order-details.models';
import { OrderStatus } from '../../../core/models/order.models';

@Component({
  selector: 'app-order-details-dialog',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './order-details-dialog.component.html',
  styleUrl: './order-details-dialog.component.scss'
})
export class OrderDetailsDialogComponent implements OnInit {
  @Input({ required: true }) orderId!: string;
  @Output() close = new EventEmitter<void>();

  private readonly api = inject(OrderService);

  protected readonly loading = signal(true);
  protected readonly error = signal<string | null>(null);
  protected readonly data = signal<OrderDetailsResponse | null>(null);

  protected readonly statusLabel: Record<OrderStatus, string> = {
    Created: 'Criado',
    Processing: 'Processando',
    Shipped: 'Enviado',
    Delivered: 'Entregue',
    Cancelled: 'Cancelado'
  };

  ngOnInit(): void {
    this.api.getDetails(this.orderId).subscribe({
      next: res => { this.data.set(res); this.loading.set(false); },
      error: err => {
        this.error.set(err?.error?.message ?? 'Falha ao carregar detalhes.');
        this.loading.set(false);
      }
    });
  }

  protected onBackdropClick(): void { this.close.emit(); }
}
