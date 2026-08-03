import axios from "axios";

/**
 * The HTTP client cannot read React context (it lives outside the component
 * tree), so it exposes a small "bridge": the AuthProvider injects functions
 * to read the current token and to force logout on a 401. This avoids having
 * to pass the token manually on every request and keeps it out of props/
 * circular imports.
 */
type TokenGetter = () => string | null;
type UnauthorizedHandler = () => void;

let getToken: TokenGetter = () => null;
let onUnauthorized: UnauthorizedHandler = () => {};

export function registerAuthBridge(bridge: {
  getToken: TokenGetter;
  onUnauthorized: UnauthorizedHandler;
}) {
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

httpClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      onUnauthorized();
    }
    return Promise.reject(error);
  }
);
