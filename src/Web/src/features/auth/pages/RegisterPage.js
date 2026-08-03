import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Link, useNavigate } from "react-router-dom";
import { isAxiosError } from "axios";
import { useAuth } from "../hooks/useAuth";
import { registerSchema } from "../schemas/registerSchema";
export function RegisterPage() {
    const { register: registerUser } = useAuth();
    const navigate = useNavigate();
    const [serverError, setServerError] = useState(null);
    const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm({ resolver: zodResolver(registerSchema) });
    const onSubmit = async (values) => {
        setServerError(null);
        try {
            await registerUser(values.email, values.password, values.passwordConfirm);
            navigate("/dashboard", { replace: true });
        }
        catch (error) {
            if (isAxiosError(error) && error.response?.data?.message) {
                setServerError(error.response.data.message);
            }
            else {
                setServerError("No se pudo completar el registro. Intentá de nuevo.");
            }
        }
    };
    return (_jsx("main", { className: "app-shell", children: _jsxs("section", { className: "card", children: [_jsx("h1", { children: "Crear cuenta" }), _jsx("p", { className: "subtitle", children: "Registrate para empezar a usar NutriMetrics." }), _jsxs("form", { className: "login-form", onSubmit: handleSubmit(onSubmit), noValidate: true, children: [_jsx("label", { htmlFor: "email", children: "Email" }), _jsx("input", { id: "email", type: "email", autoComplete: "email", ...register("email"), "aria-invalid": !!errors.email }), errors.email && _jsx("span", { className: "field-error", children: errors.email.message }), _jsx("label", { htmlFor: "password", children: "Contrase\u00F1a" }), _jsx("input", { id: "password", type: "password", autoComplete: "new-password", ...register("password"), "aria-invalid": !!errors.password }), errors.password && (_jsx("span", { className: "field-error", children: errors.password.message })), _jsx("label", { htmlFor: "passwordConfirm", children: "Confirmar contrase\u00F1a" }), _jsx("input", { id: "passwordConfirm", type: "password", autoComplete: "new-password", ...register("passwordConfirm"), "aria-invalid": !!errors.passwordConfirm }), errors.passwordConfirm && (_jsx("span", { className: "field-error", children: errors.passwordConfirm.message })), serverError && _jsx("div", { className: "form-error", children: serverError }), _jsx("button", { type: "submit", disabled: isSubmitting, children: isSubmitting ? "Creando cuenta..." : "Registrarme" })] }), _jsxs("p", { className: "switch-link", children: ["\u00BFYa ten\u00E9s cuenta? ", _jsx(Link, { to: "/login", children: "Iniciar sesi\u00F3n" })] })] }) }));
}
