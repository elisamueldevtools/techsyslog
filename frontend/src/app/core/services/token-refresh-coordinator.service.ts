import { Injectable, inject } from '@angular/core';
import { Observable, ReplaySubject, filter, map, take } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class TokenRefreshCoordinatorService {
  private readonly auth = inject(AuthService);
  private inflight$: ReplaySubject<string> | null = null;

  coordinatedRefresh(): Observable<string> {
    if (this.inflight$) {
      return this.inflight$.asObservable().pipe(take(1));
    }

    const subject = new ReplaySubject<string>(1);
    this.inflight$ = subject;

    this.auth.refresh().subscribe({
      next: res => {
        subject.next(res.accessToken);
        subject.complete();
        this.inflight$ = null;
      },
      error: err => {
        subject.error(err);
        this.inflight$ = null;
      }
    });

    return subject.asObservable().pipe(take(1));
  }

  abort(): void {
    if (this.inflight$) {
      this.inflight$.error(new Error('refresh aborted'));
      this.inflight$ = null;
    }
  }
}
