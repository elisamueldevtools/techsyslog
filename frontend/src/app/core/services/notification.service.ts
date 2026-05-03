import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { NotificationItem } from '../models/notification.models';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/notifications`;

  readonly items = signal<NotificationItem[]>([]);
  readonly unreadCount = signal(0);

  load(): Observable<NotificationItem[]> {
    return this.http.get<NotificationItem[]>(this.base).pipe(
      tap(list => {
        this.items.set(list);
        this.unreadCount.set(list.filter(n => !n.read).length);
      })
    );
  }

  markAsRead(id: string): Observable<{ success: boolean }> {
    return this.http.post<{ success: boolean }>(`${this.base}/${id}/read`, {}).pipe(
      tap(() => {
        const updated = this.items().map(n => (n.id === id ? { ...n, read: true } : n));
        this.items.set(updated);
        this.unreadCount.set(updated.filter(n => !n.read).length);
      })
    );
  }

  pushFromRealtime(item: NotificationItem): void {
    const next = [item, ...this.items().filter(n => n.id !== item.id)];
    this.items.set(next);
    this.unreadCount.set(next.filter(n => !n.read).length);
  }
}
