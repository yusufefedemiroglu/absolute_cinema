import { inject, Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import {environment} from "../../environments/environment";
import { AuthResponseDto,RegisterRequestDto,LoginRequestDto } from "./auth.models";
@Injectable({providedIn: 'root'})
export class AuthService {
    private readonly baseUrl = `${environment.apiUrl}/auth`;
    constructor(private http: HttpClient) {}

    register(dto: RegisterRequestDto) {
        return this.http.post<AuthResponseDto>(`${this.baseUrl}/register`, dto,{
            withCredentials: true
        });
    }

}
