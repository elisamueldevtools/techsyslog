import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateDeliveryRequest } from '../models/delivery.models';

@Injectable({ providedIn: 'root' })
export class DeliveryService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/deliveries`;

  create(req: CreateDeliveryRequest): Observable<{ success: boolean }> {
    return this.http.post<{ success: boolean }>(this.base, req);
  }
}
