import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { isAxiosError } from "axios";
import { useAuth } from "../hooks/useAuth";
import { loginSchema, type LoginFormValues } from "../schemas/loginSchema";

export function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [serverError, setServerError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting }
  } = useForm<LoginFormValues>({ resolver: zodResolver(loginSchema) });

  const redirectTo = (location.state as { from?: string })?.from ?? "/dashboard";

  const onSubmit = async (values: LoginFormValues) => {
    setServerError(null);
    try {
      await login(values.email, values.password);
      navigate(redirectTo, { replace: true });
    } catch (error) {
      if (isAxiosError(error) && error.response?.status === 401) {
        setServerError("Email o contraseña incorrectos.");
      } else {
        setServerError("No se pudo iniciar sesión. Intentá de nuevo.");
      }
    }
  };

  return (
    <main className="app-shell">
      <section className="card">
        <h1>Iniciar sesión</h1>
        <p className="subtitle">Accedé a tu cuenta de NutriMetrics.</p>

        <form className="login-form" onSubmit={handleSubmit(onSubmit)} noValidate>
          <label htmlFor="email">Email</label>
          <input
            id="email"
            type="email"
            autoComplete="email"
            {...register("email")}
            aria-invalid={!!errors.email}
          />
          {errors.email && <span className="field-error">{errors.email.message}</span>}

          <label htmlFor="password">Contraseña</label>
          <input
            id="password"
            type="password"
            autoComplete="current-password"
            {...register("password")}
            aria-invalid={!!errors.password}
          />
          {errors.password && (
            <span className="field-error">{errors.password.message}</span>
          )}

          {serverError && <div className="form-error">{serverError}</div>}

          <button type="submit" disabled={isSubmitting}>
            {isSubmitting ? "Ingresando..." : "Ingresar"}
          </button>
        </form>

        <p className="switch-link">
          ¿No tenés cuenta? <Link to="/register">Registrate</Link>
        </p>
      </section>
    </main>
  );
}
