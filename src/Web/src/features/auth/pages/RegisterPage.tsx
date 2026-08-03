import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Link, useNavigate } from "react-router-dom";
import { isAxiosError } from "axios";
import { useAuth } from "../hooks/useAuth";
import { registerSchema, type RegisterFormValues } from "../schemas/registerSchema";

export function RegisterPage() {
  const { register: registerUser } = useAuth();
  const navigate = useNavigate();
  const [serverError, setServerError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting }
  } = useForm<RegisterFormValues>({ resolver: zodResolver(registerSchema) });

  const onSubmit = async (values: RegisterFormValues) => {
    setServerError(null);
    try {
      await registerUser(values.email, values.password, values.passwordConfirm);
      navigate("/dashboard", { replace: true });
    } catch (error) {
      if (isAxiosError(error) && error.response?.data?.message) {
        setServerError(error.response.data.message as string);
      } else {
        setServerError("No se pudo completar el registro. Intentá de nuevo.");
      }
    }
  };

  return (
    <main className="app-shell">
      <section className="card">
        <h1>Crear cuenta</h1>
        <p className="subtitle">Registrate para empezar a usar NutriMetrics.</p>

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
            autoComplete="new-password"
            {...register("password")}
            aria-invalid={!!errors.password}
          />
          {errors.password && (
            <span className="field-error">{errors.password.message}</span>
          )}

          <label htmlFor="passwordConfirm">Confirmar contraseña</label>
          <input
            id="passwordConfirm"
            type="password"
            autoComplete="new-password"
            {...register("passwordConfirm")}
            aria-invalid={!!errors.passwordConfirm}
          />
          {errors.passwordConfirm && (
            <span className="field-error">{errors.passwordConfirm.message}</span>
          )}

          {serverError && <div className="form-error">{serverError}</div>}

          <button type="submit" disabled={isSubmitting}>
            {isSubmitting ? "Creando cuenta..." : "Registrarme"}
          </button>
        </form>

        <p className="switch-link">
          ¿Ya tenés cuenta? <Link to="/login">Iniciar sesión</Link>
        </p>
      </section>
    </main>
  );
}
