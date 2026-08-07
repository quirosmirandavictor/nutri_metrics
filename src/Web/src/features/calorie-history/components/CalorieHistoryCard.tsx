import { useState, type FormEvent } from "react";
import { useCalorieHistory } from "../hooks/useCalorieHistory";
import { ReusableLineChart, type ChartSeries } from "../../../components/charts";

function defaultStartDate(): string {
  const d = new Date();
  d.setDate(d.getDate() - 6);
  return d.toISOString().slice(0, 10);
}

function defaultEndDate(): string {
  return new Date().toISOString().slice(0, 10);
}

export default function CalorieHistoryCard() {
  const [startDate, setStartDate] = useState(defaultStartDate());
  const [endDate, setEndDate] = useState(defaultEndDate());
  const { dataPoints, loading, error, fetchRange } = useCalorieHistory();

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault();
    fetchRange(startDate, endDate);
  };

  const series: ChartSeries[] = [
    {
      id: "calories",
      name: "Calorías totales",
      data: dataPoints,
      color: "#4338ca"
    }
  ];

  return (
    <section className="card card--wide">
      <h2>Historial de calorías</h2>
      <p className="subtitle">
        Consultá el total de calorías registradas por día dentro de un rango de fechas.
      </p>

      <form className="date-range-form" onSubmit={handleSubmit}>
        <div className="date-range-field">
          <label htmlFor="startDate">Desde</label>
          <input
            id="startDate"
            type="date"
            value={startDate}
            max={endDate}
            onChange={(e) => setStartDate(e.target.value)}
          />
        </div>

        <div className="date-range-field">
          <label htmlFor="endDate">Hasta</label>
          <input
            id="endDate"
            type="date"
            value={endDate}
            min={startDate}
            onChange={(e) => setEndDate(e.target.value)}
          />
        </div>

        <button type="submit" disabled={loading}>
          {loading ? "Consultando..." : "Consultar"}
        </button>
      </form>

      {error && <p className="food-error">{error}</p>}

      <ReusableLineChart
        series={series}
        title="Calorías totales por día"
        formatXAxis={(date) =>
          new Date(date).toLocaleDateString("es-ES", { day: "2-digit", month: "short" })
        }
        formatTooltipValue={(value) => `${value} kcal`}
        emptyMessage="Elegí un rango de fechas y presioná Consultar."
      />
    </section>
  );
}
