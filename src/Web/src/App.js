import { jsx as _jsx } from "react/jsx-runtime";
import { AuthProvider } from "./features/auth/context/AuthContext";
import { AppRouter } from "./routes/AppRouter";
const App = () => {
    return (_jsx(AuthProvider, { children: _jsx(AppRouter, {}) }));
};
export default App;
