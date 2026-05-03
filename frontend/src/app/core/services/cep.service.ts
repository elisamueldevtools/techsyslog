import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LookupCepResponse } from '../models/cep.models';

@Injectable({ providedIn: 'root' })
export class CepService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/cep`;

  lookup(cep: string): Observable<LookupCepResponse> {
    return this.http.get<LookupCepResponse>(`${this.base}/${cep}`);
  }
}
