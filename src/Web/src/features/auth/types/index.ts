export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  success: boolean;
  message: string;
  token: string;
  userId: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  passwordConfirm: string;
}

/** El backend devuelve exactamente el mismo shape que el login (autologin). */
export type RegisterResponse = LoginResponse;

/** Lo que efectivamente vive en memoria/estado de la app. */
export interface AuthSession {
  token: string;
  userId: string;
  /** Timestamp absoluto (ms epoch) en el que la sesión expira. */
  expiresAt: number;
}

export interface AuthState {
  status: "idle" | "restoring" | "authenticated" | "unauthenticated";
  session: AuthSession | null;
}
