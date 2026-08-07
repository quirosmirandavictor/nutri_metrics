import { useCallback, useRef, useState } from "react";
import axios from "axios";
import {
  getFoodItemsByDateRange,
  type FoodItemHistoryEntry
} from "../api/getFoodItemsByDateRange";
import type { DataPoint } from "../../../components/charts";

function toDayKey(isoDate: string): string {
  // Keeps just the calendar day (YYYY-MM-DD) so multiple food items on the
  // same day collapse into a single point on the chart.
  return isoDate.slice(0, 10);
}

function aggregateByDay(entries: FoodItemHistoryEntry[]): DataPoint[] {
  const totals = new Map<string, number>();

  entries.forEach((entry) => {
    const day = toDayKey(entry.createdAt);
    totals.set(day, (totals.get(day) ?? 0) + entry.calories);
  });

  return Array.from(totals.entries())
    .map(([date, value]) => ({ date, value: Math.round(value) }))
    .sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime());
}

export function useCalorieHistory() {
  const [dataPoints, setDataPoints] = useState<DataPoint[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const abortRef = useRef<AbortController | null>(null);

  const fetchRange = useCallback(async (startDate: string, endDate: string) => {
    if (!startDate || !endDate) {
      setError("Seleccioná ambas fechas.");
      return;
    }
    if (new Date(startDate) > new Date(endDate)) {
      setError("La fecha inicial no puede ser posterior a la final.");
      return;
    }

    // Cancel any in-flight request before starting a new one.
    abortRef.current?.abort();
    const controller = new AbortController();
    abortRef.current = controller;

    setLoading(true);
    setError(null);

    try {
      const entries = await getFoodItemsByDateRange(startDate, endDate, controller.signal);
      setDataPoints(aggregateByDay(entries));
      if (entries.length === 0) {
        setError("No hay alimentos registrados en ese rango de fechas.");
      }
    } catch (err) {
      if (axios.isCancel(err)) return;
      setError("Ocurrió un error al consultar el historial. Intentá de nuevo.");
      setDataPoints([]);
    } finally {
      setLoading(false);
    }
  }, []);

  return { dataPoints, loading, error, fetchRange };
}
