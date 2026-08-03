import { jsx as _jsx, Fragment as _Fragment } from "react/jsx-runtime";
import { Navigate, useLocation } from "react-router-dom";
import { useAuth } from "../features/auth/hooks/useAuth";
export function ProtectedRoute({ children }) {
    const { status } = useAuth();
    const location = useLocation();
    if (status === "restoring") {
        // Prevent a flash to /login while the encrypted session is being
        // restored from sessionStorage on page load.
        return null;
    }
    if (status !== "authenticated") {
        return _jsx(Navigate, { to: "/login", replace: true, state: { from: location.pathname } });
    }
    return _jsx(_Fragment, { children: children });
}
