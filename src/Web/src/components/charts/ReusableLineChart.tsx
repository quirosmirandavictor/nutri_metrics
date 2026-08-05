import React, { useMemo, useId } from "react";
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
  TooltipProps,
} from "recharts";

/* ------------------------------------------------------------------ */
/*  Types                                                              */
/* ------------------------------------------------------------------ */

export interface DataPoint {
  /** Date in ISO format (YYYY-MM-DD) or timestamp string */
  date: string;
  value: number;
}

export interface ChartSeries {
  id: string;
  name: string;
  data: DataPoint[];
  /** Optional color; falls back to the default palette if omitted */
  color?: string;
}

export interface DateRange {
  start: Date;
  end: Date;
}

export interface ReusableLineChartProps {
  series?: ChartSeries[];
  dateRange?: DateRange;
  title?: string;
  subtitle?: string;
  height?: number;
  /** X-axis label formatter, e.g. (date) => date.toLocaleDateString() */
  formatXAxis?: (date: string) => string;
  formatYAxis?: (value: number) => string;
  formatTooltipValue?: (value: number) => string;
  /** Message shown when there is no data to plot */
  emptyMessage?: string;
}

/* ------------------------------------------------------------------ */
/*  Default palette (modern, high-contrast, dark-theme friendly)       */
/* ------------------------------------------------------------------ */

const DEFAULT_PALETTE = [
  "#6366F1", // indigo
  "#22C55E", // green
  "#F59E0B", // amber
  "#EC4899", // pink
  "#06B6D4", // cyan
  "#EF4444", // red
];

/* ------------------------------------------------------------------ */
/*  Utility: filters a single series' points by date range             */
/* ------------------------------------------------------------------ */

function filterByRange(data: DataPoint[] = [], range?: DateRange): DataPoint[] {
  if (!Array.isArray(data)) return [];
  if (!range) return data;
  const start = range.start.getTime();
  const end = range.end.getTime();
  return data.filter((d) => {
    const t = new Date(d.date).getTime();
    return t >= start && t <= end;
  });
}

/* ------------------------------------------------------------------ */
/*  Utility: merges all series into a single dataset keyed by date     */
/*  (recharts needs an array of objects { date, seriesA, seriesB... }) */
/* ------------------------------------------------------------------ */

function mergeSeries(series: ChartSeries[] = [], range?: DateRange) {
  const map = new Map<string, Record<string, number | string>>();

  if (!Array.isArray(series)) return [];

  series.forEach((s) => {
    if (!s || !Array.isArray(s.data)) return;
    const filtered = filterByRange(s.data, range);
    filtered.forEach((point) => {
      const existing = map.get(point.date) ?? { date: point.date };
      existing[s.id] = point.value;
      map.set(point.date, existing);
    });
  });

  return Array.from(map.values()).sort(
    (a, b) => new Date(a.date as string).getTime() - new Date(b.date as string).getTime()
  );
}

/* ------------------------------------------------------------------ */
/*  Custom tooltip                                                     */
/* ------------------------------------------------------------------ */

function CustomTooltip({
  active,
  payload,
  label,
  formatTooltipValue,
}: TooltipProps<number, string> & { formatTooltipValue?: (v: number) => string }) {
  if (!active || !payload || payload.length === 0) return null;

  return (
    <div
      style={{
        background: "rgba(24, 24, 27, 0.92)",
        backdropFilter: "blur(6px)",
        borderRadius: 12,
        padding: "10px 14px",
        boxShadow: "0 8px 24px rgba(0,0,0,0.18)",
        border: "1px solid rgba(255,255,255,0.08)",
        minWidth: 140,
      }}
    >
      <div style={{ color: "#A1A1AA", fontSize: 12, marginBottom: 6 }}>
        {label as string}
      </div>
      {payload.map((entry) => (
        <div
          key={entry.dataKey as string}
          style={{
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
            gap: 12,
            fontSize: 13,
            color: "#FAFAFA",
            marginTop: 2,
          }}
        >
          <span style={{ display: "flex", alignItems: "center", gap: 6 }}>
            <span
              style={{
                width: 8,
                height: 8,
                borderRadius: "50%",
                background: entry.color,
                display: "inline-block",
              }}
            />
            {entry.name}
          </span>
          <strong>
            {formatTooltipValue ? formatTooltipValue(entry.value as number) : entry.value}
          </strong>
        </div>
      ))}
    </div>
  );
}

/* ------------------------------------------------------------------ */
/*  Main component                                                     */
/* ------------------------------------------------------------------ */

export default function ReusableLineChart({
  series = [],
  dateRange,
  title,
  subtitle,
  height = 340,
  formatXAxis,
  formatYAxis,
  formatTooltipValue,
  emptyMessage = "No data available for the selected range.",
}: ReusableLineChartProps) {
  const gradientId = useId();

  // Guards against series being undefined/null while data is still
  // loading upstream (e.g. an async fetch that hasn't resolved yet).
  const safeSeries = Array.isArray(series) ? series : [];

  const chartData = useMemo(
    () => mergeSeries(safeSeries, dateRange),
    [safeSeries, dateRange]
  );

  const hasData = safeSeries.length > 0 && chartData.length > 0;

  return (
    <div
      style={{
        background: "#0B0B0D",
        borderRadius: 20,
        padding: "24px 20px",
        fontFamily:
          "'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif",
      }}
    >
      {(title || subtitle) && (
        <div style={{ marginBottom: 18 }}>
          {title && (
            <h3 style={{ color: "#FAFAFA", fontSize: 16, fontWeight: 600, margin: 0 }}>
              {title}
            </h3>
          )}
          {subtitle && (
            <p style={{ color: "#71717A", fontSize: 13, margin: "4px 0 0" }}>{subtitle}</p>
          )}
        </div>
      )}

      {!hasData && (
        <div
          style={{
            height,
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            color: "#52525B",
            fontSize: 13,
            border: "1px dashed rgba(255,255,255,0.08)",
            borderRadius: 12,
          }}
        >
          {emptyMessage}
        </div>
      )}

      {hasData && (
      <ResponsiveContainer width="100%" height={height}>
        <LineChart data={chartData} margin={{ top: 8, right: 12, left: 0, bottom: 0 }}>
          <defs>
            {safeSeries.map((s, i) => (
              <linearGradient
                key={s.id}
                id={`${gradientId}-${s.id}`}
                x1="0"
                y1="0"
                x2="0"
                y2="1"
              >
                <stop
                  offset="0%"
                  stopColor={s.color ?? DEFAULT_PALETTE[i % DEFAULT_PALETTE.length]}
                  stopOpacity={0.35}
                />
                <stop
                  offset="100%"
                  stopColor={s.color ?? DEFAULT_PALETTE[i % DEFAULT_PALETTE.length]}
                  stopOpacity={0}
                />
              </linearGradient>
            ))}
          </defs>

          <CartesianGrid
            strokeDasharray="3 3"
            vertical={false}
            stroke="rgba(255,255,255,0.06)"
          />

          <XAxis
            dataKey="date"
            tickFormatter={formatXAxis}
            tick={{ fill: "#71717A", fontSize: 12 }}
            axisLine={{ stroke: "rgba(255,255,255,0.08)" }}
            tickLine={false}
          />

          <YAxis
            tickFormatter={formatYAxis}
            tick={{ fill: "#71717A", fontSize: 12 }}
            axisLine={false}
            tickLine={false}
            width={44}
          />

          <Tooltip
            content={<CustomTooltip formatTooltipValue={formatTooltipValue} />}
            cursor={{ stroke: "rgba(255,255,255,0.15)", strokeWidth: 1 }}
          />

          {safeSeries.length > 1 && (
            <Legend
              iconType="circle"
              wrapperStyle={{ fontSize: 12, color: "#A1A1AA", paddingTop: 12 }}
            />
          )}

          {safeSeries.map((s, i) => {
            const color = s.color ?? DEFAULT_PALETTE[i % DEFAULT_PALETTE.length];
            return (
              <Line
                key={s.id}
                type="monotone"
                dataKey={s.id}
                name={s.name}
                stroke={color}
                strokeWidth={2.5}
                dot={false}
                activeDot={{ r: 5, strokeWidth: 0 }}
                connectNulls
                animationDuration={600}
              />
            );
          })}
        </LineChart>
      </ResponsiveContainer>
      )}
    </div>
  );
}