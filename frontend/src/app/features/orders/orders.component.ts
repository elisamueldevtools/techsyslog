import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { OrderService } from '../../core/services/order.service';
import { CepService } from '../../core/services/cep.service';
import { OrderListItem, OrderStatus } from '../../core/models/order.models';
import { LookupCepResponse } from '../../core/models/cep.models';
import { ChangeStatusDialogComponent } from './change-status-dialog/change-status-dialog.component';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ChangeStatusDialogComponent],
  templateUrl: './orders.component.html',
  styleUrl: './orders.component.scss'
})
export class OrdersComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(OrderService);
  private readonly cep = inject(CepService);

  protected readonly orders = signal<OrderListItem[]>([]);
  protected readonly loading = signal(false);
  protected readonly creating = signal(false);
  protected readonly error = signal<string | null>(null);

  protected readonly addressPreview = signal<LookupCepResponse | null>(null);
  protected readonly cepLookupError = signal<string | null>(null);
  protected readonly cepLoading = signal(false);

  protected readonly dialogTarget = signal<OrderListItem | null>(null);

  protected readonly statuses: OrderStatus[] = ['Created', 'Processing', 'Shipped', 'Delivered', 'Cancelled'];

  protected readonly statusLabel: Record<OrderStatus, string> = {
    Created: 'Criado',
    Processing: 'Processando',
    Shipped: 'Enviado',
    Delivered: 'Entregue',
    Cancelled: 'Cancelado'
  };

  protected readonly form = this.fb.nonNullable.group({
    orderNumber: ['', [Validators.required, Validators.pattern(/^\d+$/), Validators.maxLength(20)]],
    description: ['', Validators.required],
    value: [0, [Validators.required, Validators.min(0.01)]],
    cep: ['', [Validators.required, Validators.pattern(/^\d{8}$/)]],
    number: ['', Validators.required],
    complement: ['', [Validators.maxLength(100)]],
    observation: ['', [Validators.maxLength(500)]]
  });

  constructor() {
    this.form.controls.cep.valueChanges
      .pipe(debounceTime(400), distinctUntilChanged(), takeUntilDestroyed())
      .subscribe(cep => this.tryLookupCep(cep));
  }

  ngOnInit(): void { this.refresh(); }

  refresh(): void {
    this.loading.set(true);
    this.api.list().subscribe({
      next: list => { this.orders.set(list); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  private tryLookupCep(cep: string): void {
    if (!/^\d{8}$/.test(cep)) {
      this.addressPreview.set(null);
      this.cepLookupError.set(null);
      return;
    }
    this.cepLoading.set(true);
    this.cepLookupError.set(null);
    this.cep.lookup(cep).subscribe({
      next: res => {
        this.addressPreview.set(res);
        this.cepLoading.set(false);
      },
      error: () => {
        this.addressPreview.set(null);
        this.cepLookupError.set('CEP não encontrado');
        this.cepLoading.set(false);
      }
    });
  }

  submit(): void {
    if (this.form.invalid) return;
    this.creating.set(true);
    this.error.set(null);
    const raw = this.form.getRawValue();
    const payload = {
      orderNumber: raw.orderNumber,
      description: raw.description,
      value: raw.value,
      cep: raw.cep,
      number: raw.number,
      complement: raw.complement.trim() ? raw.complement : undefined,
      observation: raw.observation.trim() ? raw.observation : undefined
    };
    this.api.create(payload).subscribe({
      next: () => {
        this.creating.set(false);
        this.form.reset({
          orderNumber: '', description: '', value: 0, cep: '', number: '',
          complement: '', observation: ''
        });
        this.addressPreview.set(null);
        this.cepLookupError.set(null);
        this.refresh();
      },
      error: err => {
        this.creating.set(false);
        this.error.set(err?.error?.message ?? 'Falha ao criar pedido');
      }
    });
  }

  canChangeStatus(o: OrderListItem): boolean {
    return o.status !== 'Delivered' && o.status !== 'Cancelled';
  }

  openStatusDialog(o: OrderListItem): void {
    if (this.canChangeStatus(o)) this.dialogTarget.set(o);
  }

  cancelStatus(): void {
    this.dialogTarget.set(null);
  }

  confirmStatus(next: OrderStatus): void {
    const target = this.dialogTarget();
    if (!target) return;
    this.api.updateStatus(target.id, { status: next }).subscribe({
      next: () => {
        this.dialogTarget.set(null);
        this.refresh();
      },
      error: err => {
        this.dialogTarget.set(null);
        this.error.set(err?.error?.message ?? 'Falha ao mudar status');
      }
    });
  }
}
