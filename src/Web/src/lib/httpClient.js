import axios from "axios";
let getToken = () => null;
let onUnauthorized = () => { };
export function registerAuthBridge(bridge) {
    getToken = bridge.getToken;
    onUnauthorized = bridge.onUnauthorized;
}
export const httpClient = axios.create({
    baseURL: "/api",
    headers: {
        "Content-Type": "application/json"
    }
});
httpClient.interceptors.request.use((config) => {
    const token = getToken();
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});
httpClient.interceptors.response.use((response) => response, (error) => {
    if (error.response?.status === 401) {
        onUnauthorized();
    }
    return Promise.reject(error);
});
