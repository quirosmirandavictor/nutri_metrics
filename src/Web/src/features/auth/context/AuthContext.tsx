import {
  createContext,
  useCallback,
  useEffect,
  useMemo,
  useRef,
  useState,
  type ReactNode
} from "react";
import { login as loginRequest, register as registerRequest } from "../api/authApi";
import { persistSession, restoreSession, clearSession } from "../utils/secureStorage";
import { registerAuthBridge } from "../../../lib/httpClient";
import type { AuthSession, AuthState, LoginResponse } from "../types";

const SESSION_DURATION_MS = 60 * 60 * 1000; // 1 hora, fija por requerimiento

interface AuthContextValue extends AuthState {
  login: (email: string, password: string) => Promise<void>;
  register: (email: string, password: string, passwordConfirm: string) => Promise<void>;
  logout: () => void;
}

export const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [state, setState] = useState<AuthState>({ status: "restoring", session: null });
  const expiryTimeout = useRef<ReturnType<typeof setTimeout> | null>(null);

  const clearScheduledExpiry = () => {
    if (expiryTimeout.current) {
      clearTimeout(expiryTimeout.current);
      expiryTimeout.current = null;
    }
  };

  const logout = useCallback(() => {
    clearScheduledExpiry();
    clearSession();
    setState({ status: "unauthenticated", session: null });
  }, []);

  const scheduleExpiry = useCallback(
    (session: AuthSession) => {
      clearScheduledExpiry();
      const remainingMs = session.expiresAt - Date.now();
      // setTimeout con delays absurdamente grandes desborda a 32 bits; con
      // un máximo de 1h esto nunca pasa, pero se deja el guard por si
      // SESSION_DURATION_MS cambia en el futuro.
      const safeDelay = Math.max(0, Math.min(remainingMs, 2 ** 31 - 1));
      expiryTimeout.current = setTimeout(logout, safeDelay);
    },
    [logout]
  );

  // Lógica compartida entre login y register: ambos terminan con el mismo
  // shape de respuesta (token + userId), así que arman y activan la sesión
  // de la misma forma. Evita duplicar persistSession/scheduleExpiry/setState
  // en dos lugares que después habría que mantener sincronizados.
  const activateSession = useCallback(
    async (response: LoginResponse) => {
      const session: AuthSession = {
        token: response.token,
        userId: response.userId,
        expiresAt: Date.now() + SESSION_DURATION_MS
      };

      await persistSession(session);
      setState({ status: "authenticated", session });
      scheduleExpiry(session);
    },
    [scheduleExpiry]
  );

  const login = useCallback(
    async (email: string, password: string) => {
      const response = await loginRequest({ email, password });
      await activateSession(response);
    },
    [activateSession]
  );

  const register = useCallback(
    async (email: string, password: string, passwordConfirm: string) => {
      const response = await registerRequest({ email, password, passwordConfirm });
      await activateSession(response);
    },
    [activateSession]
  );

  // Restaurar sesión al montar la app (refresh de página dentro de la misma
  // pestaña). Si no hay sesión válida o ya expiró, queda "unauthenticated".
  useEffect(() => {
    let cancelled = false;

    restoreSession().then((session) => {
      if (cancelled) return;
      if (session) {
        setState({ status: "authenticated", session });
        scheduleExpiry(session);
      } else {
        setState({ status: "unauthenticated", session: null });
      }
    });

    return () => {
      cancelled = true;
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  // Puente hacia el cliente HTTP: le da acceso de solo lectura al token
  // actual y le permite forzar logout ante un 401 del backend.
  useEffect(() => {
    registerAuthBridge({
      getToken: () => state.session?.token ?? null,
      onUnauthorized: logout
    });
  }, [state.session, logout]);

  useEffect(() => clearScheduledExpiry, []);

  const value = useMemo<AuthContextValue>(
    () => ({ ...state, login, register, logout }),
    [state, login, register, logout]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
