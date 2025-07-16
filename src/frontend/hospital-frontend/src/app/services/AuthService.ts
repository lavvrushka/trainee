import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private loggedIn = new BehaviorSubject<boolean>(false);
  isLoggedIn$: Observable<boolean> = this.loggedIn.asObservable();

  signIn(username: string, password: string): void {
    // TODO: логика вызова API и обработки ошибок
    // после успешного входа:
    this.loggedIn.next(true);
  }

  logout(): void {
    // TODO: очистить токены, удалить куки и т.п.
    this.loggedIn.next(false);
  }
}
