import { httpClient } from "../../../lib/httpClient";
import type { FoodItem } from "./searchFood";

/**
 * Matches the backend's AddFoodItemRequest / AddFoodItemCommand shape:
 * (Name, Calories, Protein, Carbs, Fat, CreatedAt) -> camelCase over JSON.
 */
export interface AddFoodItemRequest {
  name: string;
  calories: number;
  protein: number;
  carbs: number;
  fat: number;
  createdAt: string;
}

interface AddFoodItemResponse {
  id: string;
}

interface DeleteFoodItemResponse {
  id: string;
}

/**
 * Persists a food item (coming from a search result) to the database via
 * POST /api/Food. Returns the backend-generated id, used later to delete it.
 */
export async function addFoodItem(
  item: FoodItem,
  createdAt: string = new Date().toISOString()
): Promise<string> {
  const request: AddFoodItemRequest = {
    name: item.name,
    calories: item.calories,
    protein: item.proteinGrams,
    carbs: item.carbohydratesGrams,
    fat: item.fatGrams,
    createdAt
  };

  const { data } = await httpClient.post<AddFoodItemResponse>("/Food", request);
  return data.id;
}

/**
 * Deletes a previously saved food item via DELETE /api/Food/{id}.
 * `id` must be the backend id returned by addFoodItem, not the local
 * client-side selection id.
 */
export async function deleteFoodItem(id: string): Promise<string> {
  const { data } = await httpClient.delete<DeleteFoodItemResponse>(`/Food/${id}`);
  return data.id;
}
