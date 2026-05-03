import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CreateOrderRequest,
  CreateOrderResponse,
  OrderListItem,
  OrderStatus,
  UpdateOrderStatusRequest
} from '../models/order.models';
import { OrderDetailsResponse } from '../models/order-details.models';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/orders`;

  list(filter?: { status?: OrderStatus }): Observable<OrderListItem[]> {
    let params = new HttpParams();
    if (filter?.status) params = params.set('status', filter.status);
    return this.http.get<OrderListItem[]>(this.base, { params });
  }

  create(req: CreateOrderRequest): Observable<CreateOrderResponse> {
    return this.http.post<CreateOrderResponse>(this.base, req);
  }

  updateStatus(id: string, req: UpdateOrderStatusRequest): Observable<{ success: boolean }> {
    return this.http.put<{ success: boolean }>(`${this.base}/${id}/status`, req);
  }

  getDetails(id: string): Observable<OrderDetailsResponse> {
    return this.http.get<OrderDetailsResponse>(`${this.base}/${id}/details`);
  }
}
