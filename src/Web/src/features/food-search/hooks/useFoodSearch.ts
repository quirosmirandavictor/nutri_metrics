import { useCallback, useRef, useState } from "react";
import axios from "axios";
import { searchFood, type FoodItem } from "../api/searchFood";

export function useFoodSearch() {
  const [results, setResults] = useState<FoodItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const abortRef = useRef<AbortController | null>(null);

  const search = useCallback(async (query: string) => {
    const trimmed = query.trim();
    if (!trimmed) {
      setError("Escribí un término de búsqueda.");
      return;
    }

    // Cancel any in-flight request before starting a new one.
    abortRef.current?.abort();
    const controller = new AbortController();
    abortRef.current = controller;

    setLoading(true);
    setError(null);

    try {
      const items = await searchFood(trimmed, controller.signal);
      setResults(items);
      if (items.length === 0) {
        setError("No se encontraron resultados.");
      }
    } catch (err) {
      if (axios.isCancel(err)) return;
      setError("Ocurrió un error al buscar. Intentá de nuevo.");
      setResults([]);
    } finally {
      setLoading(false);
    }
  }, []);

  return { results, loading, error, search };
}
