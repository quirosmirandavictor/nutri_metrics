import { httpClient } from "../../../lib/httpClient";
import type {
  LoginRequest,
  LoginResponse,
  RegisterRequest,
  RegisterResponse
} from "../types";

export async function login(payload: LoginRequest): Promise<LoginResponse> {
  const { data } = await httpClient.post<LoginResponse>("/Auth/login", payload);
  return data;
}

export async function register(payload: RegisterRequest): Promise<RegisterResponse> {
  const { data } = await httpClient.post<RegisterResponse>("/Auth/register", payload);
  return data;
}
