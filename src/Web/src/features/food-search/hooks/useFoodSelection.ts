import { useCallback, useEffect, useState } from "react";
import type { FoodItem } from "../api/searchFood";
import { addFoodItem, deleteFoodItem } from "../api/foodItemsApi";

export type SelectionStatus = "draft" | "saving" | "saved" | "deleting" | "error";

export interface SelectedFoodItem {
  id: string;
  item: FoodItem;
  status: SelectionStatus;
  /** Id returned by the backend once the item has been saved (POST /Food). */
  backendId?: string;
  error?: string;
}

export interface SelectionTotals {
  calories: number;
  proteinGrams: number;
  fatGrams: number;
  carbohydratesGrams: number;
}

// Plain (non-encrypted) sessionStorage key. Unlike the auth token, this data
// isn't sensitive, so it doesn't need the AES-GCM handling used in
// secureStorage.ts. It's a *draft* the user builds while searching -- saving
// it to the database is triggered explicitly per item via the "Guardar"
// button, this hook only keeps the draft alive across the current session.
const STORAGE_KEY = "nm_selected_foods";

function readFromSession(): SelectedFoodItem[] {
  try {
    const raw = sessionStorage.getItem(STORAGE_KEY);
    if (!raw) return [];
    const parsed = JSON.parse(raw);
    return Array.isArray(parsed) ? parsed : [];
  } catch {
    return [];
  }
}

export function useFoodSelection() {
  const [selection, setSelection] = useState<SelectedFoodItem[]>(() => readFromSession());

  useEffect(() => {
    sessionStorage.setItem(STORAGE_KEY, JSON.stringify(selection));
  }, [selection]);

  const addItem = useCallback((item: FoodItem) => {
    const entry: SelectedFoodItem = { id: crypto.randomUUID(), item, status: "draft" };
    setSelection((prev) => [...prev, entry]);
  }, []);

  /** Removes a draft (never saved) entry locally, without touching the backend. */
  const removeItem = useCallback((id: string) => {
    setSelection((prev) => prev.filter((entry) => entry.id !== id));
  }, []);

  /** Persists a draft entry via POST /api/Food. */
  const saveItem = useCallback(
    async (id: string) => {
      const entry = selection.find((e) => e.id === id);
      if (!entry || entry.status === "saving") return;

      setSelection((prev) =>
        prev.map((e) => (e.id === id ? { ...e, status: "saving", error: undefined } : e))
      );

      try {
        const backendId = await addFoodItem(entry.item);
        setSelection((prev) =>
          prev.map((e) => (e.id === id ? { ...e, status: "saved", backendId } : e))
        );
      } catch {
        setSelection((prev) =>
          prev.map((e) =>
            e.id === id ? { ...e, status: "error", error: "No se pudo guardar el alimento." } : e
          )
        );
      }
    },
    [selection]
  );

  /** Deletes a previously saved entry via DELETE /api/Food/{id}. */
  const deleteItem = useCallback(
    async (id: string) => {
      const entry = selection.find((e) => e.id === id);
      if (!entry?.backendId || entry.status === "deleting") return;

      setSelection((prev) =>
        prev.map((e) => (e.id === id ? { ...e, status: "deleting", error: undefined } : e))
      );

      try {
        await deleteFoodItem(entry.backendId);
        setSelection((prev) => prev.filter((e) => e.id !== id));
      } catch {
        setSelection((prev) =>
          prev.map((e) =>
            e.id === id ? { ...e, status: "error", error: "No se pudo eliminar el alimento." } : e
          )
        );
      }
    },
    [selection]
  );

  const clear = useCallback(() => setSelection([]), []);

  const totals: SelectionTotals = selection.reduce(
    (acc, { item }) => ({
      calories: acc.calories + item.calories,
      proteinGrams: acc.proteinGrams + item.proteinGrams,
      fatGrams: acc.fatGrams + item.fatGrams,
      carbohydratesGrams: acc.carbohydratesGrams + item.carbohydratesGrams
    }),
    { calories: 0, proteinGrams: 0, fatGrams: 0, carbohydratesGrams: 0 }
  );

  return { selection, addItem, removeItem, saveItem, deleteItem, clear, totals };
}
