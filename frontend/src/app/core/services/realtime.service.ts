import { Injectable, inject, signal } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr';
import { Observable, Subject } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';
import { NotificationService } from './notification.service';
import { NotificationItem } from '../models/notification.models';

export interface RealtimeEvent {
  name: string;
  payload: unknown;
}

@Injectable({ providedIn: 'root' })
export class RealtimeService {
  private readonly auth = inject(AuthService);
  private readonly notifications = inject(NotificationService);
  private connection?: HubConnection;

  readonly status = signal<'disconnected' | 'connecting' | 'connected'>('disconnected');
  readonly lastEvent = signal<RealtimeEvent | null>(null);

  private readonly _events$ = new Subject<RealtimeEvent>();
  readonly events$: Observable<RealtimeEvent> = this._events$.asObservable();

  async start(): Promise<void> {
    if (this.connection && this.connection.state !== HubConnectionState.Disconnected) return;

    this.status.set('connecting');
    this.connection = new HubConnectionBuilder()
      .withUrl(environment.hubUrl, { accessTokenFactory: () => this.auth.getAccessToken() ?? '' })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build();

    this.connection.on('Notification', (payload: NotificationItem) => {
      this.notifications.pushFromRealtime(payload);
      this.emit('Notification', payload);
    });

    ['OrderCreated', 'OrderStatusChanged', 'DeliveryRegistered'].forEach(evt => {
      this.connection!.on(evt, (payload: unknown) => this.emit(evt, payload));
    });

    this.connection.onreconnected(() => this.status.set('connected'));
    this.connection.onreconnecting(() => this.status.set('connecting'));
    this.connection.onclose(() => this.status.set('disconnected'));

    try {
      await this.connection.start();
      this.status.set('connected');
    } catch {
      this.status.set('disconnected');
    }
  }

  async stop(): Promise<void> {
    await this.connection?.stop();
    this.status.set('disconnected');
  }

  private emit(name: string, payload: unknown): void {
    const event: RealtimeEvent = { name, payload };
    this.lastEvent.set(event);
    this._events$.next(event);
  }
}
