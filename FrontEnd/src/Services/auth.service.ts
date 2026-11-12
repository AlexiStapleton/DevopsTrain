import axios from 'axios';
import type { RegisterRequest } from '../Model/RegisterRequest';

const apiUrl = "http://localhost:8080/auth/";
const STORAGE_KEY = "token";

class AuthService {
    login(email: string, password: string) {
        return axios.post(`${apiUrl}login`, {email, password})
            .then(response => {
                this.saveToken(response.data);
                return response.data;
            })
    }
    
    saveToken(data: unknown) {
        const token =
            typeof data === "string"
                ? data
                : (data as any)?.accessToken ?? (data as any)?.token;
        if (token) localStorage.setItem(STORAGE_KEY, token);
    }
    logout() {
        localStorage.removeItem(STORAGE_KEY);
    }
    
    register(registerRequest:  RegisterRequest) {
        return axios.post(`${apiUrl}register`, registerRequest)
            .then(response => {
                this.saveToken(response.data);
                return response.data;
            })
    }

    getToken(): string | null {
        return localStorage.getItem(STORAGE_KEY);
    }

    getCurrentUser() {
        const token = this.getToken();
        if (!token) return null;
        try {
            const payload = token.split(".")[1];
            const json = atob(payload.replace(/-/g, "+").replace(/_/g, "/"));
            return JSON.parse(json);
        } catch {
            return null;
        }
    }
    
}

export default new AuthService();