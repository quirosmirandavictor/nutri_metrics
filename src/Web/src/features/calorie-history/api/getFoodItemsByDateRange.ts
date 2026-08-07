import { httpClient } from "../../../lib/httpClient";

/**
 * Shape returned by GetFoodItemsByDateRangeQuery, as consumed by the
 * chart aggregation below. Only `calories` and `createdAt` are required
 * to build the "total calories per day" series -- if your actual
 * FoodItemResponse DTO uses different field names, adjust this interface
 * (and nothing else needs to change).
 */
export interface FoodItemHistoryEntry {
  id: string;
  name: string;
  calories: number;
  createdAt: string;
}

/**
 * Calls GET /api/Food/by-date-range?startDate=&endDate= through the shared
 * httpClient. The Authorization header is injected automatically (see
 * lib/httpClient.ts), and the backend derives the current user from the JWT.
 */
export async function getFoodItemsByDateRange(
  startDate: string,
  endDate: string,
  signal?: AbortSignal
): Promise<FoodItemHistoryEntry[]> {
  const { data } = await httpClient.get<FoodItemHistoryEntry[]>("/Food/by-date-range", {
    params: { startDate, endDate },
    signal
  });

  return Array.isArray(data) ? data : [];
}
