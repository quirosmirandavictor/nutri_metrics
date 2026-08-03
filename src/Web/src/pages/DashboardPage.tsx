import { useAuth } from "../features/auth/hooks/useAuth";

export function DashboardPage() {
  const { session, logout } = useAuth();

  return (
    <main className="app-shell">
      <section className="card">
        <h1>NutriMetrics Web</h1>
        <p>Sesión activa para el usuario {session?.userId}.</p>
        <button type="button" onClick={logout}>
          Cerrar sesión
        </button>
      </section>
    </main>
  );
}
