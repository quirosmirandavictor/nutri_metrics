import { useState, type FormEvent } from "react";
import { useFoodSearch } from "../hooks/useFoodSearch";
import { useFoodSelection } from "../hooks/useFoodSelection";
import type { FoodItem } from "../api/searchFood";

const MACRO_COLORS = {
  protein: "#22c55e",
  carbs: "#f59e0b",
  fat: "#ec4899"
};

export default function FoodSearchCard() {
  const [query, setQuery] = useState("");
  const { results, loading, error, search } = useFoodSearch();
  const { selection, addItem, removeItem, saveItem, deleteItem, totals } = useFoodSelection();

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault();
    search(query);
  };

  return (
    <section className="card card--wide">
      <h2>Buscar alimento</h2>
      <p className="subtitle">Buscá un alimento y agregalo a tu selección.</p>

      <form className="food-search-form" onSubmit={handleSubmit}>
        <input
          type="text"
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          placeholder="Ej: manzana, pechuga de pollo..."
          aria-label="Término de búsqueda"
        />
        <button type="submit" disabled={loading}>
          {loading ? "Buscando..." : "Buscar"}
        </button>
      </form>

      {error && <p className="food-error">{error}</p>}

      {results.length > 0 && (
        <div className="food-results-grid">
          {results.map((item, i) => (
            <FoodResultCard key={`${item.name}-${i}`} item={item} onAdd={() => addItem(item)} />
          ))}
        </div>
      )}

      <div className="food-selection">
        <div className="food-selection-header">
          <h3>Selección actual</h3>
          <span>
            {selection.length} {selection.length === 1 ? "alimento" : "alimentos"}
          </span>
        </div>

        {selection.length === 0 ? (
          <p className="food-selection-empty">Todavía no agregaste ningún alimento.</p>
        ) : (
          <>
            <ul className="food-selection-list">
              {selection.map(({ id, item, status, backendId, error: itemError }) => (
                <li key={id} className="food-selection-item">
                  <div className="food-selection-item-info">
                    <span>
                      {capitalize(item.name)} · {Math.round(item.calories)} kcal
                    </span>
                    {status === "saved" && (
                      <span className="food-status-badge food-status-saved">Guardado</span>
                    )}
                    {itemError && <span className="food-status-error">{itemError}</span>}
                  </div>

                  <div className="food-selection-item-actions">
                    {status === "draft" && (
                      <>
                        <button
                          type="button"
                          className="food-save-button"
                          onClick={() => saveItem(id)}
                        >
                          Guardar
                        </button>
                        <button
                          type="button"
                          className="food-selection-item-remove"
                          onClick={() => removeItem(id)}
                          aria-label={`Quitar ${item.name}`}
                        >
                          Quitar
                        </button>
                      </>
                    )}

                    {status === "saving" && (
                      <button type="button" className="food-save-button" disabled>
                        Guardando...
                      </button>
                    )}

                    {status === "saved" && backendId && (
                      <button
                        type="button"
                        className="food-delete-button"
                        onClick={() => deleteItem(id)}
                        aria-label={`Eliminar ${item.name}`}
                      >
                        Eliminar
                      </button>
                    )}

                    {status === "deleting" && (
                      <button type="button" className="food-delete-button" disabled>
                        Eliminando...
                      </button>
                    )}

                    {status === "error" && (
                      <>
                        <button
                          type="button"
                          className="food-save-button"
                          onClick={() => (backendId ? deleteItem(id) : saveItem(id))}
                        >
                          Reintentar
                        </button>
                        <button
                          type="button"
                          className="food-selection-item-remove"
                          onClick={() => removeItem(id)}
                          aria-label={`Quitar ${item.name}`}
                        >
                          Quitar
                        </button>
                      </>
                    )}
                  </div>
                </li>
              ))}
            </ul>

            <div className="food-selection-totals">
              <span>
                <strong>{Math.round(totals.calories)}</strong> kcal
              </span>
              <span>
                <strong>{totals.proteinGrams.toFixed(1)}</strong> g proteína
              </span>
              <span>
                <strong>{totals.carbohydratesGrams.toFixed(1)}</strong> g carbos
              </span>
              <span>
                <strong>{totals.fatGrams.toFixed(1)}</strong> g grasa
              </span>
            </div>
          </>
        )}

        <p className="food-selection-note">
          Guardá cada alimento para registrarlo en tu historial de calorías. Los alimentos sin
          guardar se pierden al cerrar la pestaña.
        </p>
      </div>
    </section>
  );
}

function FoodResultCard({ item, onAdd }: { item: FoodItem; onAdd: () => void }) {
  return (
    <article className="food-result-card">
      <div className="food-result-header">
        <span className="food-result-name">{capitalize(item.name)}</span>
        <span className="food-calorie-badge">{Math.round(item.calories)} kcal</span>
      </div>

      <div className="food-macros">
        <span>
          <span className="food-macro-dot" style={{ background: MACRO_COLORS.protein }} />
          Proteína: {item.proteinGrams} g
        </span>
        <span>
          <span className="food-macro-dot" style={{ background: MACRO_COLORS.carbs }} />
          Carbohidratos: {item.carbohydratesGrams} g
        </span>
        <span>
          <span className="food-macro-dot" style={{ background: MACRO_COLORS.fat }} />
          Grasa: {item.fatGrams} g
        </span>
      </div>

      <p className="food-serving">Porción: {item.servingSizeGrams} g</p>

      <button type="button" className="food-add-button" onClick={onAdd}>
        + Agregar a la selección
      </button>
    </article>
  );
}

function capitalize(value: string) {
  return value.charAt(0).toUpperCase() + value.slice(1);
}
