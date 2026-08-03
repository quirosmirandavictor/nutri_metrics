import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { isAxiosError } from "axios";
import { useAuth } from "../hooks/useAuth";
import { loginSchema } from "../schemas/loginSchema";
export function LoginPage() {
    const { login } = useAuth();
    const navigate = useNavigate();
    const location = useLocation();
    const [serverError, setServerError] = useState(null);
    const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm({ resolver: zodResolver(loginSchema) });
    const redirectTo = location.state?.from ?? "/dashboard";
    const onSubmit = async (values) => {
        setServerError(null);
        try {
            await login(values.email, values.password);
            navigate(redirectTo, { replace: true });
        }
        catch (error) {
            if (isAxiosError(error) && error.response?.status === 401) {
                setServerError("Email o contraseña incorrectos.");
            }
            else {
                setServerError("No se pudo iniciar sesión. Intentá de nuevo.");
            }
        }
    };
    return (_jsx("main", { className: "app-shell", children: _jsxs("section", { className: "card", children: [_jsx("h1", { children: "Iniciar sesi\u00F3n" }), _jsx("p", { className: "subtitle", children: "Acced\u00E9 a tu cuenta de NutriMetrics." }), _jsxs("form", { className: "login-form", onSubmit: handleSubmit(onSubmit), noValidate: true, children: [_jsx("label", { htmlFor: "email", children: "Email" }), _jsx("input", { id: "email", type: "email", autoComplete: "email", ...register("email"), "aria-invalid": !!errors.email }), errors.email && _jsx("span", { className: "field-error", children: errors.email.message }), _jsx("label", { htmlFor: "password", children: "Contrase\u00F1a" }), _jsx("input", { id: "password", type: "password", autoComplete: "current-password", ...register("password"), "aria-invalid": !!errors.password }), errors.password && (_jsx("span", { className: "field-error", children: errors.password.message })), serverError && _jsx("div", { className: "form-error", children: serverError }), _jsx("button", { type: "submit", disabled: isSubmitting, children: isSubmitting ? "Ingresando..." : "Ingresar" })] }), _jsxs("p", { className: "switch-link", children: ["\u00BFNo ten\u00E9s cuenta? ", _jsx(Link, { to: "/register", children: "Registrate" })] })] }) }));
}
