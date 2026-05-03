import { Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { filter } from 'rxjs';
import { OrderService } from '../../core/services/order.service';
import { DeliveryService } from '../../core/services/delivery.service';
import { RealtimeService } from '../../core/services/realtime.service';
import { OrderListItem, OrderStatus } from '../../core/models/order.models';

const REFRESH_EVENTS = new Set(['OrderCreated', 'OrderStatusChanged', 'DeliveryRegistered']);

@Component({
  selector: 'app-deliveries',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './deliveries.component.html',
  styleUrl: './deliveries.component.scss'
})
export class DeliveriesComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly ordersApi = inject(OrderService);
  private readonly api = inject(DeliveryService);
  private readonly realtime = inject(RealtimeService);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly orders = signal<OrderListItem[]>([]);
  protected readonly creating = signal(false);
  protected readonly success = signal<string | null>(null);
  protected readonly error = signal<string | null>(null);

  protected readonly statusLabel: Record<OrderStatus, string> = {
    Created: 'Criado',
    Processing: 'Processando',
    Shipped: 'Enviado',
    Delivered: 'Entregue',
    Cancelled: 'Cancelado'
  };

  protected readonly form = this.fb.nonNullable.group({
    orderId: ['', Validators.required],
    deliveredAt: [new Date().toISOString().slice(0, 16), Validators.required],
    notes: ['']
  });

  ngOnInit(): void {
    this.loadShippedOrders();
    this.realtime.events$
      .pipe(
        filter(e => REFRESH_EVENTS.has(e.name)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe(() => this.loadShippedOrders());
  }

  private loadShippedOrders(): void {
    this.ordersApi.list({ status: 'Shipped' }).subscribe(list => this.orders.set(list));
  }

  submit(): void {
    if (this.form.invalid) return;
    this.creating.set(true);
    this.error.set(null);
    this.success.set(null);
    const value = this.form.getRawValue();
    this.api.create({
      orderId: value.orderId,
      deliveredAt: new Date(value.deliveredAt).toISOString(),
      notes: value.notes
    }).subscribe({
      next: () => {
        this.creating.set(false);
        this.success.set('Entrega registrada.');
        this.form.patchValue({ orderId: '', notes: '' });
        this.loadShippedOrders();
      },
      error: err => {
        this.creating.set(false);
        this.error.set(err?.error?.message ?? 'Falha ao registrar entrega');
      }
    });
  }
}
