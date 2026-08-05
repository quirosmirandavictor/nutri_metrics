import { defineConfig, loadEnv } from "vite";

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), "VITE_");

  return {
    server: {
      host: "0.0.0.0",
      port: 5173,
      proxy: {
        "/api": {
          target: env.VITE_PROXY_TARGET || "http://localhost:8080",
          changeOrigin: true
        }
      },
      watch: {
        // Bind mounts under Docker (especially on Windows/Mac Docker Desktop)
        // don't always propagate native fs change events to the container.
        // Polling guarantees the dev server picks up file changes.
        usePolling: true,
        interval: 300
      }
    },
    preview: {
      host: "0.0.0.0",
      port: 5173
    }
  };
});