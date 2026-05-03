import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../core/services/notification.service';
import { RealtimeService } from '../../core/services/realtime.service';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './shell.component.html',
  styleUrl: './shell.component.scss'
})
export class ShellComponent implements OnInit, OnDestroy {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly realtime = inject(RealtimeService);
  protected readonly notifications = inject(NotificationService);

  protected readonly user = this.auth.currentUser;
  protected readonly status = this.realtime.status;

  async ngOnInit(): Promise<void> {
    this.notifications.load().subscribe();
    await this.realtime.start();
  }

  async ngOnDestroy(): Promise<void> {
    await this.realtime.stop();
  }

  logout(): void {
    this.auth.logoutServerSide().subscribe({
      next: () => this.finalizeLogout(),
      error: () => this.finalizeLogout()
    });
  }

  goToNotifications(): void {
    this.router.navigateByUrl('/notifications');
  }

  private finalizeLogout(): void {
    this.auth.logout();
    this.router.navigateByUrl('/login');
  }
}
