import AuthService from "./auth.service";

export default function authHeader() {
    const token = AuthService.getToken();
    return token ? { Authorization: `Bearer ${token}` } : {};
}