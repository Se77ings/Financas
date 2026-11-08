const MONTH_ABBREV_PT = ['jan', 'fev', 'mar', 'abr', 'mai', 'jun', 'jul', 'ago', 'set', 'out', 'nov', 'dez'] as const;

/** Mês no contrato da API (CalendarMonth: Jan=1 … Dec=12). */
export function formatCalendarMonthAbbrev(month: number, locale = 'pt-BR'): string {
    if (month < 1 || month > 12) return String(month);
    if (locale.toLowerCase().startsWith('pt')) {
        return MONTH_ABBREV_PT[month - 1];
    }
    const date = new Date(2000, month - 1, 1);
    return date.toLocaleString(locale, { month: 'short' }).replace(/\.$/, '');
}

export function formatEffectiveDateLabel(month: number, year: number, locale = 'pt-BR'): string {
    return `${formatCalendarMonthAbbrev(month, locale)}-${year}`;
}

/** Anos com pelo menos uma competência, em ordem crescente. */
export function yearsOf(effectiveDates: { year: number }[]): number[] {
    return [...new Set(effectiveDates.map(d => d.year))].sort((a, b) => a - b);
}

/** O ano atual quando tem competência; senão, o último com competência. Nulo sem nenhuma. */
export function defaultYear(years: number[]): number | null {
    const current = new Date().getFullYear();
    return years.includes(current) ? current : years[years.length - 1] ?? null;
}
