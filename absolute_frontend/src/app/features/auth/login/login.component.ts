import {
  Component,
  ElementRef,
  Renderer2,
  ViewChild,
  HostListener,
  AfterViewInit,
  OnDestroy,
} from '@angular/core';

import {
  FormBuilder,
  Validators,
  FormControl,
  FormGroup,
  AbstractControl,
  ReactiveFormsModule,
} from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/auth/auth.service';
import { TokenStore } from '../../../core/auth/token.store';
import { LoginRequestDto } from '../../../core/auth/auth.models';

@Component({
  selector: 'app-login',

  // standalone cause this component is not declared in any module
  standalone: true,

  // we need ReactiveFormsModule for reactive forms
  imports: [ReactiveFormsModule, CommonModule],

  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements AfterViewInit, OnDestroy {
    //template reference to the card element
    @ViewChild('card', { static: false })
  cardRef!: ElementRef<HTMLElement>;

  // UI states
  isLoading = false;
  showSuccess = false;
  showPassword = false;

  // renderer2 listeners to remove on destroy
  private removeListeners: Array<() => void> = [];

  // Reactive form object
  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private renderer: Renderer2,
    private host: ElementRef<HTMLElement>,
    private authService: AuthService,
    private tokenStore: TokenStore,
  ) {
    // form initialization on constructor
    this.form = this.fb.group({
      userNameOrEmail: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      remember: [false],
    });
  }

  ngAfterViewInit(): void {
    if (!this.cardRef?.nativeElement) return;

    const card = this.cardRef.nativeElement;

    // mouse tilt effect(which does not work somehow)
    const moveUnlisten = this.renderer.listen(card, 'mousemove', (e: MouseEvent) => {
      const rect = card.getBoundingClientRect();
      const centerX = rect.left + rect.width / 2;
      const centerY = rect.top + rect.height / 2;

      const mouseX = e.clientX - centerX;
      const mouseY = e.clientY - centerY;

      const rotateX = (mouseY / rect.height) * -10;
      const rotateY = (mouseX / rect.width) * 10;

      card.style.transform = `perspective(1000px) rotateX(${rotateX}deg) rotateY(${rotateY}deg)`;
    });

    // when mouse leaves, reset transform
    const leaveUnlisten = this.renderer.listen(card, 'mouseleave', () => {
      card.style.transform = '';
    });

    // hiding listeners on destroy
    this.removeListeners.push(moveUnlisten, leaveUnlisten);
  }

  // when component is destroyed, remove event listeners
  ngOnDestroy(): void {
    this.removeListeners.forEach((fn) => fn());
  }

  // listen to document mousemove for future use
  @HostListener('document:mousemove', ['$event'])
  onMouseMove(e: MouseEvent) {
    // for future use
    void e;
  }

  // show/hide password
  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }
 

 

  // Form submit
  onSubmit(): void {
    if (this.isLoading) return;


    // make all fields touched to trigger validation
    this.form.markAllAsTouched();
    if (this.form.invalid) return;

    this.isLoading = true;


    const dto: LoginRequestDto = {
      userNameOrEmail: this.form.get('userNameOrEmail')?.value ?? '' ,
      password: this.form.get('password')?.value ?? '',
    };

    this.authService.login(dto).subscribe({
      next: (res) => {
        this.tokenStore.set(res.accessToken);
        console.log('TOKEN STORED:', this.tokenStore.get());
        this.isLoading = false;
        this.showSuccess = true;
        // setTimeout(() => {
        //   this.router.navigatebyUrl('/dashboard');
        // }, 400);
      },error: (err) => {
        console.error('Login error:', err);
        this.isLoading = false;
        this.showSuccess = false;
      }

    });
    // // Fake login (UI test)
    // setTimeout(() => {
    //   this.isLoading = false;
    //   this.showSuccess = true;
    // }, 1200);

  }

  // Template push error 
  isInvalid(controlName: 'userNameOrEmail' | 'password'): boolean {
    const c = this.form.get(controlName);
    return !!c && c.invalid && (c.touched || c.dirty);
  }

  // Template error message
  getErrorMessage(controlName: 'userNameOrEmail' | 'password'): string {
    const c = this.form.get(controlName);
    if (!c) return '';

    if (c.hasError('required')) {
      return controlName === 'password'
        ? 'Your password is required'
        : 'Your email is required';
    }

    if (c.hasError('minlength')) {
      return 'Password needs at least 6 characters long';
    }

    return 'Invalid value';
  }

  private ctrl(name: 'userNameOrEmail' | 'password' | 'remember'): AbstractControl | null {
    return this.form.get(name);
  }
}