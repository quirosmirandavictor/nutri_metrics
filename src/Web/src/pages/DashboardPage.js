import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { useAuth } from "../features/auth/hooks/useAuth";
export function DashboardPage() {
    const { session, logout } = useAuth();
    return (_jsx("main", { className: "app-shell", children: _jsxs("section", { className: "card", children: [_jsx("h1", { children: "NutriMetrics Web" }), _jsxs("p", { children: ["Sesi\u00F3n activa para el usuario ", session?.userId, "."] }), _jsx("button", { type: "button", onClick: logout, children: "Cerrar sesi\u00F3n" })] }) }));
}
