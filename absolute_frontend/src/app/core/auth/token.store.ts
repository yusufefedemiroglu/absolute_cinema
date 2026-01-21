import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class TokenStore {
  private accessToken: string | null = null;

  get(): string | null {
    return this.accessToken;
  }

  set(token: string): void {
    this.accessToken = token;
  }

  clear(): void {
    this.accessToken = null;
  }
}