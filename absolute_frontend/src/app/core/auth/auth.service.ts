import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import {environment} from "../../environments/environment";
import { AuthResponseDto,LoginRequestDto } from "./auth.models";
import { tap } from "rxjs";
@Injectable({providedIn: 'root'})
export class AuthService {
    private readonly baseUrl = `${environment.apiUrl}/auth`;

    //access token stored in memory only
    private accessToken: string | null = null; 

    constructor(private http: HttpClient) {}

    getaccessToken(): string | null {
        return this.accessToken;
    }
    
    setaccessToken(token: string | null){
        this.accessToken = token;
    }

    login(dto: LoginRequestDto) {
        return this.http.post<AuthResponseDto>(`${this.baseUrl}/login`, dto,{
            withCredentials: true
        }).pipe(
            tap(({ accessToken })=> {
                this.setaccessToken(accessToken);
            })
        );
    }

    refresh(accessToken: string) {
        const token = this.getaccessToken();
        if(!token){
            throw new Error("No access token  in memory to refresh with");
        }

        return this.http.post<AuthResponseDto>(`${this.baseUrl}/refresh`, {},{
            withCredentials: true,
            headers: {
                Authorization: `Bearer ${accessToken}`
            }
        }).pipe(
            tap(res => {
                this.setaccessToken(res.accessToken);
            })
        );
    }
    revoke() {
        return this.http.post<void>(`${this.baseUrl}/revoke`, {},{
            withCredentials: true
        }).pipe(
            tap(() => {
                this.setaccessToken(null);
            })
        );
    }

}
