import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { NotificationService } from '../../core/services/notification.service';
import { NotificationItem } from '../../core/models/notification.models';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [CommonModule, DatePipe],
  templateUrl: './notifications.component.html',
  styleUrl: './notifications.component.scss'
})
export class NotificationsComponent implements OnInit {
  protected readonly api = inject(NotificationService);

  ngOnInit(): void {
    this.api.load().subscribe();
  }

  refresh(): void { this.api.load().subscribe(); }

  markRead(n: NotificationItem): void {
    if (n.read) return;
    this.api.markAsRead(n.id).subscribe();
  }
}
