import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import {environment} from "../../environments/environment";
import { AuthResponseDto,LoginRequestDto } from "./auth.models";
@Injectable({providedIn: 'root'})
export class AuthService {
    private readonly baseUrl = `${environment.apiUrl}/auth`;
    constructor(private http: HttpClient) {}

    login(dto: LoginRequestDto) {
        return this.http.post<AuthResponseDto>(`${this.baseUrl}/login`, dto,{
            withCredentials: true
        });
    }

    refresh(accessToken: string) {
        return this.http.post<AuthResponseDto>(`${this.baseUrl}/refresh`, {},{
            withCredentials: true,
            headers: {
                Authorization: `Bearer ${accessToken}`
            }
        });
    }
    
    revoke() {
        return this.http.post<void>(`${this.baseUrl}/revoke`, {},{
            withCredentials: true
        });
    }

}
