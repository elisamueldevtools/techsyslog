import { Component, DestroyRef, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { filter } from 'rxjs';
import { DashboardService } from '../../core/services/dashboard.service';
import { RealtimeService } from '../../core/services/realtime.service';
import { DashboardOrderItem, DashboardResponse } from '../../core/models/dashboard.models';
import { OrderStatus } from '../../core/models/order.models';
import { OrderDetailsDialogComponent } from './order-details-dialog/order-details-dialog.component';

type StatusFilter = OrderStatus | 'All';

const REFRESH_EVENTS = new Set(['OrderCreated', 'OrderStatusChanged', 'DeliveryRegistered']);

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, OrderDetailsDialogComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  private readonly api = inject(DashboardService);
  private readonly realtime = inject(RealtimeService);
  private readonly destroyRef = inject(DestroyRef);

  private readonly now = new Date();
  protected readonly month = signal<number>(this.now.getMonth() + 1);
  protected readonly year = signal<number>(this.now.getFullYear());
  protected readonly loading = signal(false);
  protected readonly data = signal<DashboardResponse | null>(null);

  protected readonly selectedStatus = signal<StatusFilter>('All');
  protected readonly detailsTargetId = signal<string | null>(null);

  protected readonly statusOrder: OrderStatus[] =
    ['Created', 'Processing', 'Shipped', 'Delivered', 'Cancelled'];

  protected readonly statusLabel: Record<OrderStatus, string> = {
    Created: 'Criado',
    Processing: 'Processando',
    Shipped: 'Enviado',
    Delivered: 'Entregue',
    Cancelled: 'Cancelado'
  };

  protected readonly statusClass: Record<OrderStatus, string> = {
    Created: 'created',
    Processing: 'processing',
    Shipped: 'shipped',
    Delivered: 'delivered',
    Cancelled: 'cancelled'
  };

  protected readonly months = [
    { value: 1, label: 'Janeiro' }, { value: 2, label: 'Fevereiro' },
    { value: 3, label: 'Março' }, { value: 4, label: 'Abril' },
    { value: 5, label: 'Maio' }, { value: 6, label: 'Junho' },
    { value: 7, label: 'Julho' }, { value: 8, label: 'Agosto' },
    { value: 9, label: 'Setembro' }, { value: 10, label: 'Outubro' },
    { value: 11, label: 'Novembro' }, { value: 12, label: 'Dezembro' }
  ];

  protected readonly years = computed(() => {
    const cur = new Date().getFullYear();
    return Array.from({ length: 6 }, (_, i) => cur - 4 + i);
  });

  protected readonly visibleItems = computed<DashboardOrderItem[]>(() => {
    const d = this.data();
    if (!d) return [];
    const sel = this.selectedStatus();
    if (sel === 'All') {
      return this.statusOrder
        .flatMap(s => d.grids[s])
        .sort((a, b) => b.createdAt.localeCompare(a.createdAt));
    }
    return d.grids[sel] ?? [];
  });

  ngOnInit(): void {
    this.load();
    this.realtime.events$
      .pipe(
        filter(e => REFRESH_EVENTS.has(e.name)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe(() => this.load());
  }

  protected onPeriodChange(): void { this.load(); }

  protected onCardClick(status: OrderStatus): void {
    this.selectedStatus.set(status);
  }

  protected onStatusFilterChange(value: StatusFilter): void {
    this.selectedStatus.set(value);
  }

  protected load(): void {
    this.loading.set(true);
    this.api.get(this.month(), this.year()).subscribe({
      next: res => { this.data.set(res); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  protected counter(status: OrderStatus): number {
    return this.data()?.counters[status] ?? 0;
  }

  protected isSelected(status: StatusFilter): boolean {
    return this.selectedStatus() === status;
  }

  protected statusOf(id: string): OrderStatus | null {
    const d = this.data();
    if (!d) return null;
    for (const s of this.statusOrder) {
      if (d.grids[s].some(item => item.id === id)) return s;
    }
    return null;
  }

  protected openDetails(id: string): void {
    this.detailsTargetId.set(id);
  }

  protected closeDetails(): void {
    this.detailsTargetId.set(null);
  }
}
