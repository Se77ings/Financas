import { formatCalendarMonthAbbrev } from "@/utils/calendarMonth";

export const CHART_COLORS = {
    spending: '#ef4444',
    average: '#f59e0b',
    ideal: '#22c55e',
    income: '#60a5fa',
    previous: '#9ca3af',
    grid: 'rgba(255,255,255,0.06)',
    axis: '#9ca3af',
};

export const SERIES_PALETTE = ['#60a5fa', '#f472b6', '#a78bfa', '#34d399', '#fbbf24', '#fb7185', '#38bdf8', '#c084fc', '#4ade80', '#f97316'];

export const UNCATEGORIZED_COLOR = '#4b5563';

export const TOOLTIP_STYLE = {
    contentStyle: { backgroundColor: '#1a1917', border: '1px solid rgba(255,255,255,0.1)', borderRadius: 8, fontSize: 12 },
    labelStyle: { color: '#e5e7eb', fontWeight: 600 },
    itemStyle: { padding: 0 },
};

export const AXIS_TICK = { fill: CHART_COLORS.axis, fontSize: 11 };

export function formatBRL(value: number): string {
    return value.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
}

export function formatCompactBRL(value: number): string {
    return value.toLocaleString('pt-BR', { notation: 'compact', maximumFractionDigits: 1 });
}

export function formatMonthLabel(month: number, year: number): string {
    return `${formatCalendarMonthAbbrev(month)}/${String(year).slice(-2)}`;
}

/** Recebe yyyy-mm-dd e devolve dd/mm. */
export function formatDayMonth(date: string): string {
    const [, month, day] = date.split('-');
    return `${day}/${month}`;
}
