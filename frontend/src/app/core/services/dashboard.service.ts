import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { DashboardResponse } from '../models/dashboard.models';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/dashboard`;

  get(month: number, year: number): Observable<DashboardResponse> {
    const params = new HttpParams().set('month', month).set('year', year);
    return this.http.get<DashboardResponse>(this.base, { params });
  }
}
