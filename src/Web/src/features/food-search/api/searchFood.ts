import { httpClient } from "../../../lib/httpClient";

// Field names match the backend's SearchFoodQuery response shape.
export interface FoodItem {
  name: string;
  calories: number;
  proteinGrams: number;
  fatGrams: number;
  carbohydratesGrams: number;
  servingSizeGrams: number;
}

/**
 * Calls GET /api/Food/search?query=... through the shared httpClient.
 * The Authorization header is injected automatically by the auth bridge
 * (see lib/httpClient.ts) -- no need to pass the token manually here.
 */
export async function searchFood(query: string, signal?: AbortSignal): Promise<FoodItem[]> {
  const { data } = await httpClient.get<FoodItem[]>("/Food/search", {
    params: { query },
    signal
  });

  return Array.isArray(data) ? data : [];
}
