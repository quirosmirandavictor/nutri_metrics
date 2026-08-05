import { useCallback, useEffect, useState } from "react";
import type { FoodItem } from "../api/searchFood";

export interface SelectedFoodItem {
  id: string;
  item: FoodItem;
}

export interface SelectionTotals {
  calories: number;
  proteinGrams: number;
  fatGrams: number;
  carbohydratesGrams: number;
}

// Plain (non-encrypted) sessionStorage key. Unlike the auth token, this data
// isn't sensitive, so it doesn't need the AES-GCM handling used in
// secureStorage.ts. It's a *draft* the user builds while searching --
// persisting it to the database is a separate feature triggered explicitly
// later, this hook only keeps it alive across the current tab/session.
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
    const entry: SelectedFoodItem = { id: crypto.randomUUID(), item };
    setSelection((prev) => [...prev, entry]);
  }, []);

  const removeItem = useCallback((id: string) => {
    setSelection((prev) => prev.filter((entry) => entry.id !== id));
  }, []);

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

  return { selection, addItem, removeItem, clear, totals };
}
